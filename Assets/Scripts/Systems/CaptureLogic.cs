
using System.Collections.Generic;
using System.Linq;

public enum PlayType
{
    DirectCapture,  
    BuildCapture,   
    SimplePlay      
}

public class PossiblePlay
{
    public PlayType   Type;
    public Card       HandCard;
    public List<Card> TableCards;
    public int Priority => Type switch
    {
        PlayType.DirectCapture => 1000,
        PlayType.BuildCapture  =>  500,
        _                      =>  100
    };
}

public class PlayResult
{
    public bool       Success;
    public string     Message;
    public List<Card> CapturedCards = new List<Card>();
    public bool       WasCapture;
}

public static class CaptureLogic
{
    public static List<PossiblePlay> FindPlays(Card handCard, CardStack table, Player player = null)
    {
        var result    = new List<PossiblePlay>();
        var tableList = table.GetAll().ToList();
        int myAngle   = handCard.AngleDegrees;

      
        foreach (var tc in tableList)
        {
            if (tc.AngleDegrees == myAngle)
                result.Add(new PossiblePlay {
                    Type       = PlayType.DirectCapture,
                    HandCard   = handCard,
                    TableCards = new List<Card> { tc }
                });
        }

      
        
        if (player != null)
        {
            foreach (var tc in tableList)
            {
                if (tc.IsVirtual)  continue;              
                if (tc.Suit != handCard.Suit) continue;   

                int sumAngle = (myAngle + tc.AngleDegrees) % 360;

                
                bool haveResult = player.Hand.GetAll()
                    .Any(c => !ReferenceEquals(c, handCard) && c.AngleDegrees == sumAngle);

                if (haveResult)
                    result.Add(new PossiblePlay {
                        Type       = PlayType.BuildCapture,
                        HandCard   = handCard,
                        TableCards = new List<Card> { tc }
                    });
            }
        }

     
        result.Add(new PossiblePlay {
            Type       = PlayType.SimplePlay,
            HandCard   = handCard,
            TableCards = new List<Card>()
        });

        return result;
    }

    public static bool MustCapture(Card handCard, CardStack table)
        => table.GetAll().Any(tc => tc.AngleDegrees == handCard.AngleDegrees);

    public static PlayResult Execute(PossiblePlay play, MatchState state, Player player)
    {
        var res = new PlayResult();

        if (!player.Hand.Remove(play.HandCard))
        {
            res.Success = false;
            res.Message = $"No tenes {play.HandCard?.ShortName}";
            return res;
        }

        switch (play.Type)
        {
       
            case PlayType.DirectCapture:
            {
                var tableCard = play.TableCards[0];

               
                var build = state.GetBuildForVirtualCard(tableCard);

                List<Card> realCards;
                if (build != null)
                {
                    
                    realCards = new List<Card>(build.SourceCards);
                    state.Table.Remove(tableCard);
                    state.RemoveBuild(build);
                }
                else
                {
                    
                    realCards = new List<Card> { tableCard };
                    state.Table.Remove(tableCard);
                }

                realCards.Add(play.HandCard);
                player.CaptureCards(realCards);
                state.LastCapturer = player;

                res.Success       = true;
                res.WasCapture    = true;
                res.CapturedCards = realCards;
                res.Message = $"{player.Name} captura {realCards.Count} carta(s)!";
                break;
            }

            
            case PlayType.BuildCapture:
            {
                var tableCard = play.TableCards[0];
                int sumAngle  = (play.HandCard.AngleDegrees + tableCard.AngleDegrees) % 360;

               
                var sourceCards = new List<Card> { play.HandCard, tableCard };

               
                state.Table.Remove(tableCard);
               
                var virtualCard = new Card(sumAngle, play.HandCard.Suit);
                state.Table.Add(virtualCard);
                state.RegisterBuild(virtualCard, sourceCards, sumAngle);

                res.Success    = true;
                res.WasCapture = false;
                res.Message    =
                    $"Construccion: {play.HandCard.ShortName} + {tableCard.ShortName}" +
                    $" = {sumAngle}deg  (capturala con una carta de {sumAngle}deg)";
                break;
            }

            
            case PlayType.SimplePlay:
                state.Table.Add(play.HandCard);
                res.Success    = true;
                res.WasCapture = false;
                res.Message    = $"{player.Name} coloca {play.HandCard.ShortName} en mesa";
                break;
        }

        return res;
    }
}
