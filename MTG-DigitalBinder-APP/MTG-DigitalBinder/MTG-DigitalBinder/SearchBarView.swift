//
//  FiltersView.swift
//  MTG-DigitalBinder
//
//  Created by Vinicius Rossado on 27.12.2024.
//
import SwiftUI

struct FiltersView: View {
    @ObservedObject var viewModel: CardViewModel

    var body: some View {
        VStack(spacing: 16) {
            

            // Filter Menu
            Menu {
                ForEach(viewModel.colorsIdentity, id: \.self) { option in
                    Button(action: {
                        viewModel.colorIdentity = option
                    }) {
                        HStack {
//                            ColorCircles(for: option)
//                                .frame(height: 12)
//                                .fixedSize(horizontal: true, vertical: true)
//                            Text(option.isEmpty ? "Colorless" : option)
                        }
                    }
                }
            } label: {
                HStack {
//                    ColorCircles(for: viewModel.colorIdentity)
//                    Text(viewModel.colorIdentity.isEmpty ? "Colorless" : viewModel.colorIdentity)
//                    Image(systemName: "chevron.down")
//                        .foregroundColor(.gray)
                }
                .padding(8)
                .frame(maxWidth: .infinity, alignment: .leading)
                .background(Color(.secondarySystemBackground))
                .cornerRadius(8)
                .padding(.horizontal)
            }

            // Toggle
            Toggle("Is Commander", isOn: $viewModel.isCommander)
                .toggleStyle(SwitchToggleStyle(tint: .blue))
                .padding(.horizontal)

            // Refresh Button
            RefreshButton(action: viewModel.fetchAllCards)
                .padding(.horizontal)
        }
        .background(Color(.systemGroupedBackground))
        .frame(maxWidth: .infinity, alignment: .leading)
        .navigationTitle("Search Filters")
    }
}
