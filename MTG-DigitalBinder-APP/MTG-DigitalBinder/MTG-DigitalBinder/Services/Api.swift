// MARK: - CardModel for Decoding

import Foundation

// MARK: - API Response Model
struct ApiResponse: Codable {
    let reference: Int
    let data: [ServerCard]

    // Map JSON keys to Swift property names
    enum CodingKeys: String, CodingKey {
        case reference = "Reference"
        case data = "Data"
    }
}

// MARK: - Card Data Model
struct ServerCard: Codable {
    let id: Int
    let name: String
    let imageUri: String?
    let colorIdentity: String?
    let isCommander: Bool
    let cmc: Int
    let typeLine: String

    // Map JSON keys to Swift property names
    enum CodingKeys: String, CodingKey {
        case id = "Id"
        case name = "Name"
        case imageUri = "ImageUri"
        case colorIdentity = "ColorIdentity"
        case isCommander = "IsCommander"
        case cmc = "CMC"
        case typeLine = "TypeLine"
    }
}

// MARK: - Network Manager Singleton
class NetworkManager {
    static let shared = NetworkManager()
    private init() {}

    func requestLocalNetworkAccess() {

    }

    // Endpoint para busca por nome
    func searchCards(
        name: String,
        isCommander: Bool,
        colorIdentity: String? = nil,
        completion: @escaping (Result<[ServerCard], Error>) -> Void
    ) {
        var components = URLComponents(
            string: "http://143.198.101.34:7770/api/Card/GetCardsWithPagination"
        )

        // Inicializar o array de queryItems
        var queryItems = [
            URLQueryItem(name: "reference", value: "1"),
            URLQueryItem(name: "pageSize", value: "10000"),
        ]

        // Criar o filtro JSON
        let filterDict =
            [
                "name": name,
                "isCommander": isCommander,
                "color_identity": colorIdentity,
            ] as [String: Any]

        print("Search Fiter: \(filterDict)")

        if let jsonData = try? JSONSerialization.data( 
            withJSONObject: filterDict),
            let jsonString = String(data: jsonData, encoding: .utf8)
        {
            // Adicionar o filtro aos queryItems existentes
            queryItems.append(URLQueryItem(name: "filters", value: jsonString))
        }

        // Atribuir todos os queryItems de uma vez
        components?.queryItems = queryItems

        guard let url = components?.url else {
            completion(
                .failure(NSError(domain: "Invalid URL", code: 0, userInfo: nil))
            )
            return
        }

        print("Search URL: \(url)")

        var request = URLRequest(url: url)
        request.httpMethod = "GET"
        request.setValue("application/json", forHTTPHeaderField: "Content-Type")
        request.timeoutInterval = 30

        URLSession.shared.dataTask(with: request) { data, response, error in
            if let error = error {
                completion(.failure(error))
                return
            }

            guard let httpResponse = response as? HTTPURLResponse,
                (200...299).contains(httpResponse.statusCode)
            else {
                let statusError = NSError(
                    domain: "HTTP Error",
                    code: (response as? HTTPURLResponse)?.statusCode ?? 0,
                    userInfo: nil
                )
                completion(.failure(statusError))
                return
            }

            guard let data = data else {
                completion(
                    .failure(
                        NSError(
                            domain: "No data received", code: 0, userInfo: nil))
                )
                return
            }

            do {
                let apiResponse = try JSONDecoder().decode(
                    ApiResponse.self, from: data)
                completion(.success(apiResponse.data))
            } catch {
                print("Decode error: \(error)")  // Para debug
                completion(.failure(error))
            }
        }.resume()
    }

    func getCardsWithPagination(
        reference: Int,
        pageSize: Int,
        completion: @escaping (Result<[ServerCard], Error>) -> Void
    ) {
        var components = URLComponents(
            string: "http://143.198.101.34:7770/api/Card/GetCardsWithPagination"
        )

        components?.queryItems = [
            URLQueryItem(name: "reference", value: "\(reference)"),
            URLQueryItem(name: "pageSize", value: "\(pageSize)"),
        ]

        guard let url = components?.url else {
            completion(
                .failure(NSError(domain: "Invalid URL", code: 0, userInfo: nil))
            )
            return
        }

        print("Chamou essa porra")

        var request = URLRequest(url: url)
        request.httpMethod = "GET"
        request.setValue("application/json", forHTTPHeaderField: "Content-Type")
        request.timeoutInterval = 30

        URLSession.shared.dataTask(with: request) { data, response, error in
            if let error = error {
                completion(.failure(error))
                return
            }

            guard let httpResponse = response as? HTTPURLResponse,
                (200...299).contains(httpResponse.statusCode)
            else {
                let statusError = NSError(
                    domain: "HTTP Error",
                    code: (response as? HTTPURLResponse)?.statusCode ?? 0,
                    userInfo: nil
                )
                completion(.failure(statusError))
                return
            }

            guard let data = data else {
                completion(
                    .failure(
                        NSError(
                            domain: "No data received", code: 0, userInfo: nil))
                )
                return
            }

            do {
                let apiResponse = try JSONDecoder().decode(
                    ApiResponse.self, from: data)
                completion(.success(apiResponse.data))
            } catch {
                print("Decode error: \(error)")
                completion(.failure(error))
            }
        }.resume()
    }
}
