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
    
    func getCards(reference: Int, pageSize: Int, completion: @escaping (Result<[ServerCard], Error>) -> Void) {
        guard let url = URL(string: "http://143.198.101.34:7770/api/Card/GetCardsWithPagination?reference=\(reference)&pageSize=\(pageSize)") else {
            completion(.failure(NSError(domain: "Invalid URL", code: 0, userInfo: nil)))
            return
        }
        
//        guard let url = URL(string: "http://localhost:5822/api/Card/GetCardsWithPagination?reference=\(reference)&pageSize=\(pageSize)") else {
//            completion(.failure(NSError(domain: "Invalid URL", code: 0, userInfo: nil)))
//            return
//        }
        
        var request = URLRequest(url: url)
        request.httpMethod = "GET"
        request.setValue("application/json", forHTTPHeaderField: "Content-Type")
        request.timeoutInterval = 30 // Adjust timeout as needed
        
        URLSession.shared.dataTask(with: request) { data, response, error in
            if let error = error {
                completion(.failure(error))
                return
            }
            
            guard let httpResponse = response as? HTTPURLResponse, (200...299).contains(httpResponse.statusCode) else {
                let statusError = NSError(domain: "HTTP Error", code: (response as? HTTPURLResponse)?.statusCode ?? 0, userInfo: nil)
                completion(.failure(statusError))
                return
            }
            
            guard let data = data else {
                completion(.failure(NSError(domain: "No data received", code: 0, userInfo: nil)))
                return
            }
            
            do {
                let apiResponse = try JSONDecoder().decode(ApiResponse.self, from: data)
//                print(apiResponse.data.first(where: <#T##(ServerCard) throws -> Bool#>))
                completion(.success(apiResponse.data))
            } catch {
                completion(.failure(error))
            }
        }.resume()
    }
}
