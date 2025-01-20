import Combine
import SwiftUI

// MARK: - Card Model
struct Card: Identifiable {
    let id: UUID
    let name: String
    let imageUrl: String
    var colorIdentity: String
}

// MARK: - Card ViewModel
class CardViewModel: ObservableObject {
    @Published var cards: [Card] = []
    @Published var searchQuery: String = ""
    @Published var isCommander: Bool = false
    @Published var colorIdentity: String = ""
    @Published var isLoading = false
    @Published var alertMessage: String?
    @Published var showingAlert = false

    @Published var colorsIdentity: [String] = []

    @Published private var filterState = FilterState()

    // MARK: - Filter State
    struct FilterState {
        var searchQuery = ""
        var isCommander : Bool = false
        var colorIdentity: String? = ""
        // Adicione outros filtros aqui
    }

    private var cancellables = Set<AnyCancellable>()

    init() {
        setupFilterSubscribers()
        fetchAllCards()
    }

    private func setupFilterSubscribers() {
        // Combine todos os publishers de filtro
        Publishers.CombineLatest4(
            $searchQuery.removeDuplicates(),
            $isCommander.removeDuplicates(),
            $colorIdentity.removeDuplicates(),
            // Adicione outros publishers aqui
            Just(())  // placeholder para o terceiro publisher
        )
        .debounce(for: .milliseconds(300), scheduler: RunLoop.main)
        .sink { [weak self] (query, isCommander, colorIdentity, _) in
            guard let self = self else { return }

            print(colorIdentity)
            self.filterState = FilterState(
                searchQuery: query,
                isCommander: isCommander,
                colorIdentity: colorIdentity
                    // Atualize outros filtros aqui
            )

            self.applyFilters()
        }
        .store(in: &cancellables)
    }

    private func applyFilters() {
        if filterState.searchQuery.isEmpty && !filterState.isCommander
            && filterState.colorIdentity == nil
        {
            fetchAllCards()
        } else {
            searchCards(
                query: filterState.searchQuery,
                isCommander: filterState.isCommander,
                colorIdentity: filterState.colorIdentity
                    // Adicione outros parametros de filtro aqui
            )
        }
    }

    func fetchAllCards() {
        guard !isLoading else { return }
        isLoading = true

        NetworkManager.shared.getCardsWithPagination(
            reference: 0, pageSize: 50
        ) { [weak self] result in
            DispatchQueue.main.async {
                guard let self = self else { return }
                self.handleFetchResult(result, setColorIdentity: true)
            }
        }
    }

    func searchCards(query: String, isCommander: Bool, colorIdentity: String? = nil) {
        guard !isLoading else { return }
        isLoading = true

        NetworkManager.shared.searchCards(
            name: query, isCommander: isCommander, colorIdentity: colorIdentity
        ) { [weak self] result in
            DispatchQueue.main.async {
                guard let self = self else { return }
                self.handleFetchResult(result)
            }
        }
    }

    private func handleFetchResult(
        _ result: Result<[ServerCard], Error>, setColorIdentity: Bool = false
    ) {
        switch result {
        case .success(let serverCards):
            self.cards = serverCards.map { serverCard in
                Card(
                    id: UUID(),
                    name: serverCard.name,
                    imageUrl: serverCard.imageUri
                        ?? "https://example.com/placeholder.jpg",
                    colorIdentity: serverCard.colorIdentity ?? ""
                )
            }

        case .failure(let error):
            self.alertMessage =
                "Error fetching cards: \(error.localizedDescription)"
            self.showingAlert = true
        }

        if setColorIdentity {
            // Compute distinct colors
            self.colorsIdentity = Array(
                Set(self.cards.map { $0.colorIdentity })
            ).sorted { (a, b) -> Bool in return a.count < b.count }
        }
        self.isLoading = false
    }

}

struct CardDetailsView: View {
    let card: Card
    @Environment(\.dismiss) var dismiss

    var body: some View {

        ScrollView {
            VStack(alignment: .leading, spacing: 16) {
                CardImage(url: card.imageUrl)
                    .padding(.horizontal)

                CardInformation(card: card)
                    .padding(.horizontal)
            }
        }
        .navigationTitle("Detalhes da Carta")
        .navigationBarTitleDisplayMode(.inline)
        .toolbar {
            ToolbarItem(placement: .navigationBarTrailing) {
                DismissButton(dismiss: dismiss)
            }

        }
    }
}

struct DismissButton: View {
    let dismiss: DismissAction

    var body: some View {
        Button("Fechar") {
            dismiss()
        }
    }
}

struct CardInformation: View {
    let card: Card

    var body: some View {
        VStack(alignment: .leading, spacing: 12) {
            //            InfoRow(title: "Nome", value: card.name)
        }
    }
}

//// MARK: - Shimmer Effect
struct ShimmerView: View {
    @State private var isAnimating = false

    var body: some View {
        RoundedRectangle(cornerRadius: 10)
            .fill(
                LinearGradient(
                    gradient: Gradient(colors: [
                        Color.gray.opacity(0.3), Color.gray.opacity(0.1),
                        Color.gray.opacity(0.3),
                    ]),
                    startPoint: .leading,
                    endPoint: .trailing
                )
            )
            .rotationEffect(.degrees(30))
            .offset(x: isAnimating ? 200 : -200)
            .onAppear {
                withAnimation(
                    Animation.linear(duration: 1.5).repeatForever(
                        autoreverses: false)
                ) {
                    isAnimating = true
                }
            }
    }
}

// MARK: - Preview
struct ContentView_Previews: PreviewProvider {
    static var previews: some View {
        CardGridView()
    }
}
