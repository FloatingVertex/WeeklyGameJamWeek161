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
        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator DestroyCrate()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            var crate = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("SimpleEnvironment/Crate"),Vector3.zero,Quaternion.identity);
            var bullet = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Bullets/Bullet"), Vector3.left*2, Quaternion.identity);
            bullet.GetComponent<Bullet>().velocity = Vector2.right * 800;
            yield return new WaitForFixedUpdate();
            yield return null;
            Assert.IsTrue(bullet == null);
            Assert.IsTrue(crate == null);
        }
    }
}
