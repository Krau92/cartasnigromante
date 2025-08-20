using UnityEngine;

public class GameManager : MonoBehaviour
{
    Card zoomCard; //Referencia a la carta que se ha hecho zoom
    public static GameManager instance; //Instancia del GameManager para el patrón Singleton
    private GamePhases gamePhase; //Fase del juego actual
    public enum GamePhases
    {
        Combat,
        DeckBuilding
    }
    private void Awake()
    {
        // Asegurarse de que solo haya una instancia de GameManager
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //Suscripciones a eventos
    private void OnEnable()
    {
        EventManager.instance.OnCardClicked += LockCardZoom;
        EventManager.instance.OnCardHovered += SoftCardZoom;
        EventManager.instance.OnCardUnhovered += UnsoftCardZoom;

    }

    //Desuscripciones a eventos
    private void OnDisable()
    {
        EventManager.instance.OnCardClicked -= LockCardZoom;
        EventManager.instance.OnCardHovered -= SoftCardZoom;
        EventManager.instance.OnCardUnhovered -= UnsoftCardZoom;
    }

    void Update()
    {
        ResetZoom();
    }

    //Método para gestionar los reset de zooms de las cartas
    private void ResetZoom()
    {
        //Si se ha hecho zoom en una carta y se ha pulsado el botón derecho del ratón, se resetea el zoom
        if (Input.GetMouseButtonDown(1) && zoomCard != null)
        {
            UnzoomCard();
            EventManager.instance.DeselectCard(); //Llamar al evento de deselección de carta
        }
    }

    //Método para quitar el zoom de la carta
    public void UnzoomCard()
    {
        if (zoomCard == null)
            return; //Si no hay carta con zoom, no hacer nada
        zoomCard.UnzoomCard();
        zoomCard = null; //Limpiar la referencia a la carta que se ha hecho zoom
    }

    //Método para que una carta fije el zoom
    public void LockCardZoom(Card card)
    {
        if (zoomCard != null)
            return; //Si ya hay una carta con zoom, no hacer nada
        card.ZoomCard();
        zoomCard = card; //Guardar la referencia a la carta que se ha hecho zoom
    }

    //Método para hacer zoom a una carta cuando haya hover sobre ella
    public void SoftCardZoom(Card card)
    {
        if (zoomCard != null)
            return; //Si ya hay una carta con zoom y no es la misma, no hacer nada
        card.ZoomCard(); //Hacer zoom a la carta
    }

    //Método para quitar el zoom de una carta cuando se deja de hacer hover sobre ella
    public void UnsoftCardZoom(Card card)
    {
        if (zoomCard != null)
            return; //Si ya hay una carta con zoom y es la misma, no hacer nada
        card.UnzoomCard(); //Quitar el zoom de la carta
    }

    //Método para obtener la carta que tiene el zoom
    public Card GetZoomedCard()
    {
        return zoomCard; //Devolver la carta que tiene el zoom
    }


    //Método para cambiar la fase del juego
    public void ChangeGamePhase(GamePhases newPhase)
    {
        gamePhase = newPhase; //Cambiar la fase del juego
        Debug.Log("Game phase changed to: " + gamePhase);
        //! Como llamo al inicio del combate? Evento o llamada directa? Lo llama el botón de next combat
    }

}
