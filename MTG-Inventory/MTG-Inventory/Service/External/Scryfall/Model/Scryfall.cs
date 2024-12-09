using System.Text.Json.Serialization;

namespace MTG_Card_Checker.Repository.External.Scryfall.Model;

public class Scryfall
{
    [JsonPropertyName("object")]
    public string Object { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("oracle_id")]
    public string OracleId { get; set; }

    [JsonPropertyName("multiverse_ids")]
    public List<int> MultiverseIds { get; set; }

    [JsonPropertyName("mtgo_id")]
    public int MtgoId { get; set; }

    [JsonPropertyName("arena_id")]
    public int ArenaId { get; set; }

    [JsonPropertyName("tcgplayer_id")]
    public int TcgplayerId { get; set; }

    [JsonPropertyName("cardmarket_id")]
    public int CardmarketId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("lang")]
    public string Lang { get; set; }

    [JsonPropertyName("released_at")]
    public string ReleasedAt { get; set; }

    [JsonPropertyName("uri")]
    public string Uri { get; set; }

    [JsonPropertyName("scryfall_uri")]
    public string ScryfallUri { get; set; }

    [JsonPropertyName("layout")]
    public string Layout { get; set; }

    [JsonPropertyName("highres_image")]
    public bool HighresImage { get; set; }

    [JsonPropertyName("image_status")]
    public string ImageStatus { get; set; }
    
    [JsonPropertyName("card_faces")]
    public List<CardFace> CardFaces { get; set; }

    [JsonPropertyName("image_uris")]
    public ImageUris ImageUris { get; set; }

    [JsonPropertyName("mana_cost")]
    public string ManaCost { get; set; }

    [JsonPropertyName("cmc")]
    public double Cmc { get; set; }

    [JsonPropertyName("type_line")]
    public string TypeLine { get; set; }

    [JsonPropertyName("oracle_text")]
    public string OracleText { get; set; }

    [JsonPropertyName("colors")]
    public List<object> Colors { get; set; }

    [JsonPropertyName("color_identity")]
    public string[] ColorIdentity { get; set; }

    [JsonPropertyName("keywords")]
    public List<string> Keywords { get; set; }

    [JsonPropertyName("legalities")]
    public Legalities Legalities { get; set; }

    [JsonPropertyName("games")]
    public List<string> Games { get; set; }

    [JsonPropertyName("reserved")]
    public bool Reserved { get; set; }

    [JsonPropertyName("foil")]
    public bool Foil { get; set; }

    [JsonPropertyName("nonfoil")]
    public bool Nonfoil { get; set; }

    [JsonPropertyName("finishes")]
    public List<string> Finishes { get; set; }

    [JsonPropertyName("oversized")]
    public bool Oversized { get; set; }

    [JsonPropertyName("promo")]
    public bool Promo { get; set; }

    [JsonPropertyName("reprint")]
    public bool Reprint { get; set; }

    [JsonPropertyName("variation")]
    public bool Variation { get; set; }

    [JsonPropertyName("set_id")]
    public string SetId { get; set; }

    [JsonPropertyName("set")]
    public string Set { get; set; }

    [JsonPropertyName("set_name")]
    public string SetName { get; set; }

    [JsonPropertyName("set_type")]
    public string SetType { get; set; }

    [JsonPropertyName("set_uri")]
    public string SetUri { get; set; }

    [JsonPropertyName("set_search_uri")]
    public string SetSearchUri { get; set; }

    [JsonPropertyName("scryfall_set_uri")]
    public string ScryfallSetUri { get; set; }

    [JsonPropertyName("rulings_uri")]
    public string RulingsUri { get; set; }

    [JsonPropertyName("prints_search_uri")]
    public string PrintsSearchUri { get; set; }

    [JsonPropertyName("collector_number")]
    public string CollectorNumber { get; set; }

    [JsonPropertyName("digital")]
    public bool Digital { get; set; }

    [JsonPropertyName("rarity")]
    public string Rarity { get; set; }

    [JsonPropertyName("card_back_id")]
    public string CardBackId { get; set; }

    [JsonPropertyName("artist")]
    public string Artist { get; set; }

    [JsonPropertyName("artist_ids")]
    public List<string> ArtistIds { get; set; }

    [JsonPropertyName("illustration_id")]
    public string IllustrationId { get; set; }

    [JsonPropertyName("border_color")]
    public string BorderColor { get; set; }

    [JsonPropertyName("frame")]
    public string Frame { get; set; }

    [JsonPropertyName("security_stamp")]
    public string SecurityStamp { get; set; }

    [JsonPropertyName("full_art")]
    public bool FullArt { get; set; }

    [JsonPropertyName("textless")]
    public bool Textless { get; set; }

    [JsonPropertyName("booster")]
    public bool Booster { get; set; }

    [JsonPropertyName("story_spotlight")]
    public bool StorySpotlight { get; set; }

    [JsonPropertyName("edhrec_rank")]
    public int EdhrecRank { get; set; }

    [JsonPropertyName("preview")]
    public Preview Preview { get; set; }

    [JsonPropertyName("prices")]
    public Prices Prices { get; set; }

    [JsonPropertyName("related_uris")]
    public RelatedUris RelatedUris { get; set; }

    [JsonPropertyName("purchase_uris")]
    public PurchaseUris PurchaseUris { get; set; }
}

public class CardFace
{
    [JsonPropertyName("image_uris")]
    public ImageUris ImageUris { get; set; }
}

public class ImageUris
{
    [JsonPropertyName("small")]
    public string Small { get; set; }

    [JsonPropertyName("normal")]
    public string Normal { get; set; }

    [JsonPropertyName("large")]
    public string Large { get; set; }

    [JsonPropertyName("png")]
    public string Png { get; set; }

    [JsonPropertyName("art_crop")]
    public string ArtCrop { get; set; }

    [JsonPropertyName("border_crop")]
    public string BorderCrop { get; set; }
}

public class Legalities
{
    [JsonPropertyName("standard")]
    public string Standard { get; set; }

    [JsonPropertyName("future")]
    public string Future { get; set; }

    [JsonPropertyName("historic")]
    public string Historic { get; set; }

    [JsonPropertyName("timeless")]
    public string Timeless { get; set; }

    [JsonPropertyName("gladiator")]
    public string Gladiator { get; set; }

    [JsonPropertyName("pioneer")]
    public string Pioneer { get; set; }

    [JsonPropertyName("explorer")]
    public string Explorer { get; set; }

    [JsonPropertyName("modern")]
    public string Modern { get; set; }

    [JsonPropertyName("legacy")]
    public string Legacy { get; set; }

    [JsonPropertyName("pauper")]
    public string Pauper { get; set; }

    [JsonPropertyName("vintage")]
    public string Vintage { get; set; }

    [JsonPropertyName("penny")]
    public string Penny { get; set; }

    [JsonPropertyName("commander")]
    public string Commander { get; set; }

    [JsonPropertyName("oathbreaker")]
    public string Oathbreaker { get; set; }

    [JsonPropertyName("standardbrawl")]
    public string Standardbrawl { get; set; }

    [JsonPropertyName("brawl")]
    public string Brawl { get; set; }

    [JsonPropertyName("alchemy")]
    public string Alchemy { get; set; }

    [JsonPropertyName("paupercommander")]
    public string Paupercommander { get; set; }

    [JsonPropertyName("duel")]
    public string Duel { get; set; }

    [JsonPropertyName("oldschool")]
    public string Oldschool { get; set; }

    [JsonPropertyName("premodern")]
    public string Premodern { get; set; }

    [JsonPropertyName("predh")]
    public string Predh { get; set; }
}

public class Preview
{
    [JsonPropertyName("source")]
    public string Source { get; set; }

    [JsonPropertyName("source_uri")]
    public string SourceUri { get; set; }

    [JsonPropertyName("previewed_at")]
    public string PreviewedAt { get; set; }
}

public class Prices
{
    [JsonPropertyName("usd")]
    public string Usd { get; set; }

    [JsonPropertyName("usd_foil")]
    public string UsdFoil { get; set; }

    [JsonPropertyName("usd_etched")]
    public object UsdEtched { get; set; }

    [JsonPropertyName("eur")]
    public string Eur { get; set; }

    [JsonPropertyName("eur_foil")]
    public string EurFoil { get; set; }

    [JsonPropertyName("tix")]
    public string Tix { get; set; }
}

public class PurchaseUris
{
    [JsonPropertyName("tcgplayer")]
    public string Tcgplayer { get; set; }

    [JsonPropertyName("cardmarket")]
    public string Cardmarket { get; set; }

    [JsonPropertyName("cardhoarder")]
    public string Cardhoarder { get; set; }
}

public class RelatedUris
{
    [JsonPropertyName("gatherer")]
    public string Gatherer { get; set; }

    [JsonPropertyName("tcgplayer_infinite_articles")]
    public string TcgplayerInfiniteArticles { get; set; }

    [JsonPropertyName("tcgplayer_infinite_decks")]
    public string TcgplayerInfiniteDecks { get; set; }

    [JsonPropertyName("edhrec")]
    public string Edhrec { get; set; }
}

public class Root
{
    [JsonPropertyName("object")]
    public string Object { get; set; }

    [JsonPropertyName("total_cards")]
    public int TotalCards { get; set; }

    [JsonPropertyName("has_more")]
    public bool HasMore { get; set; }

    [JsonPropertyName("data")]
    public List<Scryfall> Data { get; set; }
}