using System;
using UnityEngine;

public class Shader : MonoBehaviour
{
    [SerializeField] private float dissolveSpeed;
    [SerializeField] private Renderer renderer;
    [SerializeField] private string cutOff = "_Cut_off";
    private float dissolveAmount;
    private bool dissolving;

    internal static int PropertyToID(string v)
    {
        throw new NotImplementedException();
    }

    private void Start()
    {
        dissolveAmount = 1f;
        renderer.material.SetFloat(cutOff, dissolveAmount);

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            dissolving = true;
        }

        if (dissolving)
        {
            dissolveAmount = Mathf.MoveTowards(dissolveAmount, 0f, dissolveSpeed * Time.deltaTime);
            renderer.material.SetFloat(cutOff, dissolveAmount);
        }
    }
}
