using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleAndCreditController : MonoBehaviour
{
    public GameObject title;
    public GameObject credits;
    public AudioSource titleMusicSource;
    public AudioSource creditsMusicSource;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartAscent()
    {

    }

    public void showCredits()
    {
        credits.SetActive(true);
        creditsMusicSource.Play();
    }
}
