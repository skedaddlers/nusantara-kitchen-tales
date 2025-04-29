using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PulauDatabase", menuName = "NusantaraKitchen/Pulau Database")]
public class PulauDatabase : ScriptableObject
{
    public List<PulauDataSO> semuaPulau;

    public PulauDataSO GetPulau(string nama)
    {
        return semuaPulau.Find(p => p.namaPulau == nama);
    }
}
