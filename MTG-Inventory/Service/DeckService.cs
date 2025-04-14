
using MTG_Inventory.Models;
using MTG_Inventory.Repository;

namespace MTG_Inventory.Service;

public class DeckService(DeckRepository deckRepository, CardDeckRepository cardDeckRepository)
{
    public async Task UpdateDeck(Deck deck)
    {
        await deckRepository.UpdateDeck(deck);
    }

    public async Task AddDeck(Deck deck)
    {
        await deckRepository.AddDeck(deck);
    }
    
    public async Task<IList<Deck>> GetDecks()
    {
        return await deckRepository.GetDecks();
    }

    public async Task<Deck?> GetDeckById(int id)
    {
        return await deckRepository.GetDeckById(id);
    }
    
    public async Task UploadCards(List<CardDeck> cards)
    {
        
        await cardDeckRepository.AddCardsToDeck(cards);
    }
}