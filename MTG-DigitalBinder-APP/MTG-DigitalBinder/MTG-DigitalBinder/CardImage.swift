//
//  CardImage.swift
//  MTG-DigitalBinder
//
//  Created by Vinicius Rossado on 27.12.2024.
//

import SwiftUI

struct CardImage: View {
    let url: String
    @State private var reloadAttempts: Int = 0
    @State private var urlString: String
    private let maxRetires: Int = 3

    
    init(url: String){
        self.url = url
        _urlString = State(initialValue: url)
    }
    
    var body: some View {
        AsyncImage(url: URL(string: urlString)) { phase in
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
                if reloadAttempts < maxRetires {
                    ProgressView().frame(height: 300)
                        .task {
                            await retry()
                        }

                } else {
                    FallbackImageView()
                }
            @unknown default:
                EmptyView()
            }
        }
    }

    private func retry() async {
        try? await Task.sleep(nanoseconds: 1_000_000_000)
        reloadAttempts += 1
        urlString = "\(url)?retry=\(UUID().uuidString)"
    }
}
