//
//  FallbackImageView.swift
//  MTG-DigitalBinder
//
//  Created by Vinicius Rossado on 27.12.2024.
//

import SwiftUI

struct FallbackImageView: View {
    var body: some View {
        Image(systemName: "photo")
            .resizable()
            .scaledToFit()
            .frame(height: 300)
            .foregroundColor(.red)
            .background(Color.gray.opacity(0.3))
            .cornerRadius(10)
    }
}

#Preview {
    FallbackImageView()
}
