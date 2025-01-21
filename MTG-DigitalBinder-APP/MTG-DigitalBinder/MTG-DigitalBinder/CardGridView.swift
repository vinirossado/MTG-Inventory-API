//
//  CardGridView.swift
//  MTG-DigitalBinder
//
//  Created by Vinicius Rossado on 27.12.2024.
//

import SwiftUI

struct CardGridView: View {
    @StateObject private var viewModel = CardViewModel()
    private let numberOfColumns = 4
    private let spacing: CGFloat = 4
    @State private var isLoading = false
    @State private var currentPage = 1

    var body: some View {
        let gridItems = Array(
            repeating: GridItem(.flexible(), spacing: spacing),
            count: numberOfColumns
        )

        ZStack {
            Color(.darkGray)
                .ignoresSafeArea()
            VStack(spacing: spacing) {
                HeaderView(title: "Digital Binder")
                NavigationSplitView {
                   
                    FiltersView(viewModel: viewModel)
                        .padding(.bottom, 15)
                } detail: {
                    // Search Bar
                    TextField("Search cards by name...", text: $viewModel.searchQuery)
                        .textFieldStyle(RoundedBorderTextFieldStyle())
                        .padding(.horizontal)
                        .padding(.vertical, 8)
                    
                    CardGridContent(
                        gridItems: gridItems,
                        cards: viewModel.cards,
                        spacing: spacing,
                        isLoading: viewModel.isLoading,
                        onLoadMore: loadMoreCards
                    )
                    .onAppear {
                        if viewModel.cards.isEmpty {
                            loadMoreCards()
                        }
                    }
                }
                .alert(isPresented: $viewModel.showingAlert) {
                    Alert(
                        title: Text("Error"),
                        message: Text(
                            viewModel.alertMessage ?? "Unknown error"),
                        dismissButton: .default(Text("OK"))
                    )
                }
            }
        }
    }

    private func loadMoreCards() {
        guard !isLoading else { return }

        isLoading = true

        NetworkManager.shared.getCardsWithPagination(
            reference: self.currentPage, pageSize: 50
        ) { result in
            switch result {
            case .success(let newCards):
                DispatchQueue.main.async {
                    viewModel.cards.append(contentsOf: newCards.toCards())
                    currentPage += 1
                    isLoading = false
                }
            case .failure(let error):
                print("Error loading more cards: \(error)")
                DispatchQueue.main.async {
                    isLoading = false
                }
            }
        }
    }

}

#Preview {
    CardGridView()
}
