using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

internal class CharacterEvents
    {
    //character damaged and damage value
     public static UnityAction<GameObject, int> characterDamaged;
    //health 
     public static UnityAction<GameObject, int> characterHealed;
    }


