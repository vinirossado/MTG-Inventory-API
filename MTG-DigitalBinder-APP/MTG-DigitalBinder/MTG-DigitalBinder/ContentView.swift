import SwiftUI
import Combine

// MARK: - Card Model
struct Card: Identifiable {
    let id: UUID
    let imageUrl: String
}

class CardViewModel: ObservableObject {
    @Published var cards: [Card] = []
    @Published var isLoading = false
    @Published var alertMessage: String?
    @Published var showingAlert = false
    
    private var currentPage = 0
    
    func fetchCards() {
        guard !isLoading else { return }
        isLoading = true
        
        NetworkManager.shared.getCards(reference: currentPage, pageSize: 200) { [weak self] result in
            DispatchQueue.main.async {
                switch result {
                case .success(let serverCards):
                    let newCards = serverCards.map { serverCard in
                        Card(
                            id: UUID(), // Generate a unique identifier for each card
                            imageUrl: serverCard.imageUri ?? "https://example.com/placeholder.jpg"
                        )
                    }
                    self?.cards.append(contentsOf: newCards)
                    self?.currentPage += 1
                case .failure(let error):
                    self?.alertMessage = "Error fetching cards: \(error.localizedDescription)"
                    self?.showingAlert = true
                }
                self?.isLoading = false
            }
        }
    }
}


// MARK: - Card Grid View
struct CardGridView: View {
    @StateObject private var viewModel = CardViewModel()
    private let columns = Array(repeating: GridItem(.flexible()), count: 4)
    
    var body: some View {
        ZStack {
            Color(.black)
                .ignoresSafeArea()
        
            ScrollView {
                LazyVGrid(columns: columns, spacing: 5) {
                    ForEach(viewModel.cards) { card in
                        AsyncImage(url: URL(string: card.imageUrl)) { image in
                            image
                                .resizable()
                                .scaledToFill()
                                .cornerRadius(8)
                        } placeholder: {
                            Color.gray
                                .opacity(0.3)
                                .cornerRadius(8)
                        }
                    }
                    
                    if viewModel.isLoading {
                        ProgressView()
                    } else if viewModel.cards.isEmpty {
                        Text("No cards to display")
                            .font(.headline)
                            .foregroundColor(.gray)
                    } else {
                        ProgressView()
                            .onAppear {
                                viewModel.fetchCards()
                            }
                    }
                }
                .padding()
            }
            .navigationTitle("MTG Cards")
            .alert(isPresented: $viewModel.showingAlert) {
                Alert(title: Text("Error"), message: Text(viewModel.alertMessage ?? "Unknown error"), dismissButton: .default(Text("OK")))
            }
            .onAppear {
                viewModel.fetchCards()
            }
        }
    }
}

// MARK: - Preview
struct CardGridView_Previews: PreviewProvider {
    static var previews: some View {
        CardGridView()
    }
}
