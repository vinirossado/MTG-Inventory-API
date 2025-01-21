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
    let onLoadMore: () -> Void  // Nova propriedade para callback
    
    var body: some View {
        ScrollViewReader { proxy in
            ScrollView {
                LazyVGrid(columns: gridItems, spacing: spacing) {
                    ForEach(cards) { card in
                        CardGridItem(card: card)
                            .onAppear {
                                // Se for um dos últimos itens, carrega mais
                                if let index = cards.firstIndex(where: { $0.id == card.id }),
                                   index == cards.count - 4  // Trigger quando faltar 4 items para o final
                                {
                                    onLoadMore()
                                }
                            }
                    }
                }
                .padding(.horizontal, spacing)
                
                if isLoading {
                    ProgressView()
                        .padding(.top)
                }
            }
            .ignoresSafeArea(.keyboard)
        }
    }
}
