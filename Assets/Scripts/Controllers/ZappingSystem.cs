// using UnityEngine;
// using UnityEngine.UI;

// public class ZappingSystem : MonoBehaviour
// {
//     public float zapTime = 0.02f, zapDistance = 100f;
//     public Image zapProgress; public AudioClip zapSound;

//     GameObject current; float focus, lastCast;

//     void Update()
//     {
//         if (Time.time - lastCast < 0.2f) return;
//         lastCast = Time.time;
//         var cam = Camera.main;
//         if (Physics.SphereCast(
//              new Ray(cam.transform.position, cam.transform.forward),
//              1f, out var hit, zapDistance, LayerMask.GetMask("Debris")))
//         {
//             var d = hit.collider.GetComponent<Debris>();
//             if (d == null || !hit.collider.gameObject.activeSelf) { Reset(); return; }
//             if (hit.collider.gameObject == current)
//             {
//                 focus += Time.deltaTime;
//                 zapProgress.fillAmount = focus/zapTime;
//                 if (focus>=zapTime) Zap(current);
//             }
//             else
//             {
//                 Reset(); current = hit.collider.gameObject; d.SetTargeted(true);
//             }
//         }
//         else Reset();
//     }

//     void Zap(GameObject obj)
// {
//     // 1) Turn off target highlight
//     obj.GetComponent<Debris>().SetTargeted(false);

//     // 2) Spawn your effect at the camera
//     Vector3 origin = Camera.main.transform.position;
//     GameObject fx = ObjectPool.Instance.GetObject(origin, Quaternion.identity);

//     // 3) Initialize its flight toward the debris
//     var mover = fx.GetComponent<ZapEffectController>();
//     if (mover != null)
//         mover.Initialize(origin, obj.transform.position);

//     // 4) Play sound & notify manager
//     AudioSource.PlayClipAtPoint(zapSound, transform.position);
//     var dm = Object.FindFirstObjectByType<DebrisManager>();
//     if (dm != null) dm.DebrisZapped(obj);

//     // 5) Disable the debris
//     obj.SetActive(false);

//     // 6) Reset your targeting state
//     Reset();
// }


//     void Reset()
//     {
//         if (current) current.GetComponent<Debris>()?.SetTargeted(false);
//         current = null; focus = 0; zapProgress.fillAmount = 0;
//     }
// }
