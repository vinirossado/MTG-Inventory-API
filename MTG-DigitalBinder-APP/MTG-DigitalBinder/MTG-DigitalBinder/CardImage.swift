//
//  CardImage.swift
//  MTG-DigitalBinder
//
//  Created by Vinicius Rossado on 27.12.2024.
//

import SwiftUI

struct CardImage: View {
    let url: String

    var body: some View {
        AsyncImage(url: URL(string: url)) { phase in
            switch phase {
            case .empty:
                ShimmerView()
                    .frame(height: 300)
                    .cornerRadius(10)
            case .success(let image):
                image
                    .resizable()
                    .scaledToFit()
                    .cornerRadius(12)
                    .shadow(
                        color: .black.opacity(0.3),
                        radius: 4, x: 0, y: 2
                    )
            case .failure:
                FallbackImageView()
            @unknown default:
                EmptyView()
            }
        }
    }
}
