//
//  SearchBarView.swift
//  MTG-DigitalBinder
//
//  Created by Vinicius Rossado on 27.12.2024.
//

import SwiftUI

struct SearchBarView: View {
    @ObservedObject var viewModel: CardViewModel

    var body: some View {
        HStack(spacing: 16) {
            TextField("Search cards by name...", text: $viewModel.searchQuery)
                .textFieldStyle(RoundedBorderTextFieldStyle())
                .frame(maxWidth: .infinity)

            Menu {
                ForEach(viewModel.colorsIdentity, id: \.self) { option in
                    Button(action: {
                        viewModel.colorIdentity = option
                    }) {
                        HStack(spacing: 8) {
                            // Display circles for each option in the list

                            //                            ColorCircles(for: option)
                            //                                .frame(height: 12)
                            //                                .fixedSize(horizontal: true, vertical: true)
                            Text(option.isEmpty ? "Colorless" : option)
                        }
                    }
                }
            } label: {
                HStack {
                    // Display selected filter's circles in the Menu title
                    ColorCircles(for: viewModel.colorIdentity)
                    Image(systemName: "chevron.down")
                        .foregroundColor(.gray)
                }
                .padding(8)
                .background(Color(.secondarySystemBackground))
                .cornerRadius(8)
            }

            Toggle("Is Commander", isOn: $viewModel.isCommander)
                .toggleStyle(SwitchToggleStyle(tint: .blue))
                .fixedSize()

            RefreshButton(action: viewModel.fetchAllCards)
        }
        .padding(.horizontal)
    }

    // Example helper functions for icons and colors
    private func iconName(for colorIdentity: String) -> String {
        switch colorIdentity {
        case "Red": return "circle.fill"
        case "Blue": return "circle.fill"
        case "Green": return "circle.fill"
        case "Black": return "circle.fill"
        case "White": return "circle.fill"
        default: return "circle"
        }
    }

    private func color(for colorIdentity: String) -> Color {
        switch colorIdentity {
        case "Red": return .red
        case "Blue": return .blue
        case "Green": return .green
        case "Black": return .black
        case "White": return .white
        default: return .gray
        }
    }

    private func description(for colorIdentity: String) -> String {
        switch colorIdentity {
        case "": return "Colorless"
        case "B": return "Black"
        case "G": return "Green"
        case "B,G": return "Black and Green"
        default: return colorIdentity  // Fallback to showing the raw string
        }
    }
}

struct ColorCircles: View {
    let colors: [Color]

    init(for colorIdentity: String) {
        colors = ColorCircles.colors(for: colorIdentity)
    }

    var body: some View {
        HStack(spacing: 4) {
            ForEach(colors, id: \.self) { color in
                Circle()
                    .fill(color)
                    .frame(width: 12, height: 12)  // Fixed dimensions for the circles
            }
        }
        .frame(minWidth: 50, alignment: .leading)  // Ensures visibility within Menu items
    }

    static func colors(for colorIdentity: String) -> [Color] {
        let mapping: [String: Color] = [
            "": .gray,  // Colorless
            "B": .black,  // Black
            "G": .green,  // Green
            "U": .blue,  // Blue
            "R": .red,  // Red
            "W": .white,  // White
        ]

        // Split the identity string (e.g., "B,G") into components and map to colors
        let components = colorIdentity.split(separator: ",").map { String($0) }
        return components.compactMap { mapping[$0] }
    }
}
