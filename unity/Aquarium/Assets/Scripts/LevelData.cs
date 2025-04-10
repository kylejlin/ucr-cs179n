using UnityEngine;
using System.Collections.Generic; //list and dictionary definition


public class LevelData
{
    public int xpCap;
    public List<int> algaesUnlock = new List<int>(); 
    public List<int> trilobitesUnlock = new List<int>(); 
    public List<int> decorationsUnlock = new List<int>(); 
    public LevelData(int xpCap, List<int> algaesUnlock, List<int> trilobitesUnlock, List<int> decorationsUnlock)
    {
        this.xpCap = xpCap;
        this.algaesUnlock = algaesUnlock;
        this.trilobitesUnlock = trilobitesUnlock;
        this.decorationsUnlock = decorationsUnlock;
    }
    public LevelData(int xpCap)
    {
        this.xpCap = xpCap;
        this.algaesUnlock = new List<int>();
        this.trilobitesUnlock = new List<int>();
        this.decorationsUnlock = new List<int>();
    }
}