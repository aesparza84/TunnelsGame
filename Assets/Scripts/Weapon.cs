using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class Weapon 
{
    public string Name;
    public int Uses;
    public bool HandSide;
    public bool Broken;
    public int Stagger;
    public Weapon(string _name, int _uses, bool handSide, int stagger)
    {
        Name = _name;
        Uses = _uses;
        HandSide = handSide;
        Broken = false;
        Stagger = stagger;
    }

    public void UseWeapon()
    {
        if (Broken)
            return;

        Uses -= 1;
        if (Uses <= 0)
        {
            Broken = true;
        }
    }

    public int GetStagger()
    {
        return Stagger;
    }
}
