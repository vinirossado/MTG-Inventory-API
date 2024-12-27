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
                SearchBarView(viewModel: viewModel)

                CardGridContent(
                    gridItems: gridItems,
                    cards: viewModel.cards,
                    spacing: spacing,
                    isLoading: viewModel.isLoading
                )
            }
            .alert(isPresented: $viewModel.showingAlert) {
                Alert(
                    title: Text("Error"),
                    message: Text(viewModel.alertMessage ?? "Unknown error"),
                    dismissButton: .default(Text("OK"))
                )
            }
        }
    }
}

#Preview {
    CardGridView()
}
