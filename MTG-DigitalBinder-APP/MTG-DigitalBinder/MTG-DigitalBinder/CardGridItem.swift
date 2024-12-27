//
//  CardGridItem.swift
//  MTG-DigitalBinder
//
//  Created by Vinicius Rossado on 27.12.2024.
//

import SwiftUI

struct CardGridItem: View {
    let card: Card
    @State private var showingDetails = false

    var body: some View {
        CardImage(url: card.imageUrl)
            .onLongPressGesture {
                showingDetails = true
            }
            .sheet(isPresented: $showingDetails) {
                CardDetailsView(card: card)
            }
    }
}
