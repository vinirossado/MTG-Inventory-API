//
//  CardGridContent.swift
//  MTG-DigitalBinder
//
//  Created by Vinicius Rossado on 27.12.2024.
//

import SwiftUI

struct CardGridContent: View {
    let gridItems: [GridItem]
    let cards: [Card]
    let spacing: CGFloat
    let isLoading: Bool

    var body: some View {
        ScrollView {
            LazyVGrid(columns: gridItems, spacing: spacing) {
                ForEach(cards) { card in
                    CardGridItem(card: card)
                }
            }
            .padding(.horizontal, spacing)

            if isLoading {
                ProgressView()
                    .padding(.top)
            }
        }
    }
}
