using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

[RequireComponent(typeof(XRGrabInteractable))]
public class Pistola : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] Transform canon;
    [SerializeField] ParticleSystem fogonazo;
    [SerializeField] AudioSource audioDisparo;
    [SerializeField] AudioSource audioRecarga;
    [SerializeField] GameObject prefabImpacto;

    [Header("Balistica")]
    [SerializeField] float alcance = 40f;
    [SerializeField] float dano = 25f;
    [SerializeField] float cadencia = 0.25f;
    [SerializeField] int municionMaxima = 12;
    [SerializeField] LayerMask capasImpacto = ~0;

    [Header("Recarga")]
    [SerializeField] bool recargarAlSoltar = true;

    XRGrabInteractable agarre;
    float proximoDisparo;
    int municion;

    public int MunicionActual => municion;

    void Awake()
    {
        agarre = GetComponent<XRGrabInteractable>();
        municion = municionMaxima;
    }

    void OnEnable()
    {
        agarre.activated.AddListener(AlActivar);
        agarre.selectExited.AddListener(AlSoltar);
    }

    void OnDisable()
    {
        agarre.activated.RemoveListener(AlActivar);
        agarre.selectExited.RemoveListener(AlSoltar);
    }

    void AlActivar(ActivateEventArgs args) => Disparar(args.interactorObject);

    void AlSoltar(SelectExitEventArgs args)
    {
        if (recargarAlSoltar) Recargar();
    }

    void Disparar(IXRActivateInteractor interactor)
    {
        if (Time.time < proximoDisparo || municion <= 0) return;
        proximoDisparo = Time.time + cadencia;
        municion--;

        if (fogonazo)
        {
            fogonazo.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            fogonazo.Play();
        }

        if (audioDisparo) audioDisparo.Play();

        if (Physics.Raycast(canon.position, canon.forward,
                            out RaycastHit hit, alcance, capasImpacto))
        {
            hit.collider.GetComponentInParent<IRecibeDano>()
               ?.RecibirDano(dano, hit.point, canon.forward);

            if (prefabImpacto)
                Destroy(Instantiate(prefabImpacto, hit.point,
                        Quaternion.LookRotation(hit.normal)), 2f);
        }

        var haptico = (interactor as Component)?.GetComponentInParent<HapticImpulsePlayer>();
        haptico?.SendHapticImpulse(0.6f, 0.08f);
    }

    public void Recargar()
    {
        if (municion == municionMaxima) return;

        municion = municionMaxima;
        proximoDisparo = 0f;
        if (audioRecarga) audioRecarga.Play();
    }
}