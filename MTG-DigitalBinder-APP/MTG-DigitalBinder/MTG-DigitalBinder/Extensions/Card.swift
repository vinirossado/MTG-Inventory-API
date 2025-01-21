//
//  Card.swift
//  MTG-DigitalBinder
//
//  Created by Vinicius Rossado on 20.01.2025.
//

import Foundation

extension Card {
    init(from serverCard: ServerCard) {
        self.id = UUID()  // Generate new UUID since ServerCard uses Int
        self.name = serverCard.name
        self.imageUrl = serverCard.imageUri ?? ""  // Provide default empty string if nil
        self.colorIdentity = serverCard.colorIdentity ?? ""  // Provide default empty string if nil
    }
}

// If you need to convert an array of ServerCards
extension Array where Element == ServerCard {
    func toCards() -> [Card] {
        self.map { Card(from: $0) }
    }
}
