using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///comportamineto de este enemigo
///
/// Estado Patrol: el enemigo eleige un nodo al azar y se moverá llendo de nodo  a nodo por el camino mas corto a dicho punto
/// Estado Alerta: el personaje ha sido descubierto por el enemigo por lo que el enemigo está disparando al personaje desde el punto en el que esté y no dejará de estar en este estadoo
/// hasta que el personaje no muera quede fuera del campo de visión del enemigo.
/// Estado busqueda: el enemigo ha perdido de vista al personaje y durante cierto tiempo intentará buscar al personaje
/// Estado distraido: El enemigo destruirá el señuelo pasando totalmente del personaje hasta que lo haya destruido
///




public class EnemyShooter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
