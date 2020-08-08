using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    [TestFixture]
    public class DamageTests
    {
        [UnityTest]
        public IEnumerator DestroyCrate()
        {
            // Spawns a crate and a bullet that gets shot at the crate.
            var crate = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("SimpleEnvironment/Crate"),Vector3.zero,Quaternion.identity);
            var bullet = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Bullets/Bullet"), Vector3.left*2, Quaternion.identity);
            bullet.GetComponent<Bullet>().velocity = Vector2.right * 800;

            // Make sure both are gone after 2 frames
            yield return new WaitForFixedUpdate();
            yield return null;
            Assert.IsTrue(bullet == null);
            Assert.IsTrue(crate == null);
        }
    }
}
