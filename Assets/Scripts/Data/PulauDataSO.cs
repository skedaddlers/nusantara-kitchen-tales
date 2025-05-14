using UnityEngine;

[CreateAssetMenu(fileName = "PulauData", menuName = "NusantaraKitchen/Pulau Data")]
public class PulauDataSO : ScriptableObject
{
    public string namaPulau;

    [TextArea]
    public string deskripsiPulau;
    public ResepDataSO[] resepList; 

}