using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace HenryDev
{
    public enum eLanguage
    {
        English,
        Simplified_Chinese
    }
    public enum eAxis
    {
        Origin = 1 << 0,
        X = 1 << 1,
        Y = 1 << 2,
        Z = 1 << 3
    }

    [System.Serializable]
    public class WeightedObject<T>
    {
        public T Value;
        public float Weight;
        public WeightedObject(T value, float weight)
        {
            this.Value = value;
            this.Weight = weight;
        }
        public WeightedObject()
        {

        }
    }

    public static class CommonExtension
    {
        // * Movement
        public static void Follow(this Transform current, Transform to = null, float speed = 1)
        {
            if (to == null)
                return;
            var currentPosition = current.position;
            currentPosition = Vector3.MoveTowards(currentPosition, to.position, speed * Time.deltaTime);
            current.position = currentPosition;
        }
        public static void Follow(this Rigidbody2D current, Transform to = null, float speed = 1)
        {
            if (to == null)
                return;
            Vector3 direction = to.position - current.transform.position;
            current.velocity = direction.normalized * speed;
        }
        public static void FollowAndRemainDistance(this Transform current, Transform to = null, float speed = 1, float recoverySpeed = 1, float stoppingDistance = 0f, float stoppingDistanceBuffer = 0f)
        {
            if (to == null)
                return;
            float backSpeed = -recoverySpeed;
            var currentPosition = current.position;
            if (current.transform.IsAtRange(to, stoppingDistance, stoppingDistanceBuffer))
                currentPosition = Vector3.MoveTowards(currentPosition, to.position, 0 * Time.deltaTime);
            else if (current.transform.IsNear(to, stoppingDistance, stoppingDistanceBuffer))
                currentPosition = Vector3.MoveTowards(currentPosition, to.position, backSpeed * Time.deltaTime);
            else
                currentPosition = Vector3.MoveTowards(currentPosition, to.position, speed * Time.deltaTime);

            current.position = currentPosition;
        }
        public static void FollowAndRemainDistance(this Rigidbody2D current, Transform to = null, float speed = 1, float recoverySpeed = 1, float stoppingDistance = 0f, float stoppingDistanceBuffer = 0f)
        {
            if (to == null)
                return;
            float backSpeed = -recoverySpeed;
            Vector3 direction = to.position - current.transform.position;
            if (current.transform.IsAtRange(to, stoppingDistance, stoppingDistanceBuffer))
                current.velocity = direction.normalized * 0;
            else if (current.transform.IsNear(to, stoppingDistance, stoppingDistanceBuffer))
                current.velocity = direction.normalized * backSpeed;
            else
                current.velocity = direction.normalized * speed;
        }
        public static void MoveToward(this Transform current, Vector2 direction, float speed = 1)
        {
            current.position = (Vector2)current.position + direction.normalized * speed * Time.deltaTime;
        }
        public static void MoveToward(this Rigidbody2D current, Vector2 direction, float speed = 1)
        {
            current.velocity = direction.normalized * speed;
        }
        public static void MoveToward(this Transform current, Vector3 direction, float speed = 1)
        {
            current.position += direction.normalized * speed * Time.deltaTime;
        }
        public static void MoveToward(this Rigidbody current, Vector3 direction, float speed = 1)
        {
            current.velocity = direction.normalized * speed;
        }
        public static void RotateToward(this Transform current, Transform target, float rotateSpeed, eAxis lockedAxis = eAxis.Origin)
        {
            Vector3 targetDirection = current.GetDirectionTo(target);
            var targetRotation = Quaternion.LookRotation(targetDirection);
            Quaternion nextRotation = Quaternion.Slerp(current.localRotation, targetRotation, rotateSpeed * Time.deltaTime);
            current.rotation = nextRotation;

            var angle = current.eulerAngles;
            if (lockedAxis.HasFlag(eAxis.X))
                angle.x = 0;
            if (lockedAxis.HasFlag(eAxis.Y))
                angle.y = 0;
            if (lockedAxis.HasFlag(eAxis.Z))
                angle.z = 0;

            current.eulerAngles = angle;
        }
        public static void RotateToward(this Transform current, Vector3 targetDirection, float rotateSpeed, eAxis lockedAxis = eAxis.Origin)
        {
            var targetRotation = Quaternion.LookRotation(targetDirection);
            Quaternion nextRotation = Quaternion.Slerp(current.localRotation, targetRotation, rotateSpeed * Time.deltaTime);
            current.rotation = nextRotation;

            var angle = current.eulerAngles;
            if (lockedAxis.HasFlag(eAxis.X))
                angle.x = 0;
            if (lockedAxis.HasFlag(eAxis.Y))
                angle.y = 0;
            if (lockedAxis.HasFlag(eAxis.Z))
                angle.z = 0;

            current.eulerAngles = angle;
        }
        public static void FlipHorizontallyToward(this Transform current, Transform to, float initFlipValue, bool isReversed = false)
        {
            if (current.IsAt(to))
                return;
            float flipValue = -1 * Mathf.Sign(initFlipValue);
            if (current.IsLeftOf(to))
                flipValue = 1 * Mathf.Sign(initFlipValue);
            if (isReversed)
                flipValue *= -1;
            float finalFlipValue = flipValue * initFlipValue;

            var targetScale = current.localScale;
            targetScale.x = finalFlipValue;

            current.localScale = targetScale;
        }
        public static void FlipHorizontallyToward(this Transform current, float xAxis, float initFlipValue, bool isReversed = false)
        {
            if (xAxis == 0)
                return;
            float flipValue = -1;
            if (xAxis > 0)
                flipValue = 1;
            if (isReversed)
                flipValue *= -1;
            float finalFlipValue = flipValue * initFlipValue;

            var targetScale = current.localScale;
            targetScale.x = finalFlipValue;

            current.localScale = targetScale;
        }




        // * Type
        public static T RandomItem<T>(this IEnumerable<T> source)
        {
            int count = source.Count();
            int pickedIdx = Random.Range(0, count);
            int counter = 0;
            foreach (var item in source)
            {
                if (counter == pickedIdx)
                    return item;
                counter++;
            }
            return source.First();
        }
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            System.Random random = new System.Random();
            List<T> list = new List<T>(source);

            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }
        public static List<T> RandomItemList<T>(this IEnumerable<T> source, int amount = -1, IEnumerable<T> excluded = null)
        {
            IEnumerable<T> filtered = null;
            if (excluded != null)
            {
                filtered = source.Except(excluded);
            }
            IEnumerable<T> newSource = filtered?.Shuffle() ?? source.Shuffle();
            int count = newSource.Count();
            amount = amount < 0 ? count : amount;
            amount = amount > count ? count : amount;
            return newSource.Take(amount).ToList();
        }
        public static List<T> TakeRandomList<T>(this IEnumerable<T> source, int pickAmount, IEnumerable<T> excluded = null)
        {
            IEnumerable<T> filtered = null;
            if (excluded != null)
            {
                filtered = source.Except(excluded);
            }
            IEnumerable<T> newSource = filtered?.Shuffle() ?? source.Shuffle();
            List<T> result = new List<T>();
            for (int i = 0; i < pickAmount; i++)
            {
                result.Add(newSource.RandomItem());
            }
            return result;
        }
        public static T RandomElementByWeight<T>(this IEnumerable<WeightedObject<T>> sequence)
        {
            float totalWeight = sequence.Sum(item => item.Weight);
            float itemWeightIndex = (float)new System.Random().NextDouble() * totalWeight;
            float currentWeightIndex = 0;
            var queriable = from weightedItem in sequence select new { Value = weightedItem, Weight = weightedItem.Weight };
            foreach (var item in queriable)
            {
                currentWeightIndex += item.Weight;
                if (currentWeightIndex >= itemWeightIndex)
                    return item.Value.Value;
            }
            return default;
        }




        // * Math
        public static Vector3 RandomPositionOnHemisphereSurface(this Transform current, float radius, bool isUpperPart = true, bool canGetPointAtBottomPlane = false)
        {
            float phi;
            float theta;

            if (canGetPointAtBottomPlane && UnityEngine.Random.Range(0f, 1f) < 0.5f)
            {
                phi = 0f;
                theta = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
            }
            else
            {
                phi = UnityEngine.Random.Range(0f, Mathf.PI / 2f);
                theta = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
            }

            float hemisphereHeight = isUpperPart ? radius : -radius;

            float x = current.position.x + radius * Mathf.Sin(phi) * Mathf.Cos(theta);
            float y = current.position.y + hemisphereHeight + radius * Mathf.Sin(phi) * Mathf.Sin(theta);
            float z = current.position.z + radius * Mathf.Cos(phi);

            return new Vector3(x, y, z);
        }
        public static Vector3 RandomPositionOnSphere(this Transform current, float radius)
        {
            float theta = Random.Range(0f, Mathf.PI * 2);
            float phi = Random.Range(0f, Mathf.PI);
            float x = current.position.x + radius * Mathf.Sin(phi) * Mathf.Cos(theta);
            float y = current.position.y + radius * Mathf.Sin(phi) * Mathf.Sin(theta);
            float z = current.position.z + radius * Mathf.Cos(phi);

            return new Vector3(x, y, z);
        }
        public static Vector3 RandomPositionOnHemisphereSurface(this Vector3 current, float radius, bool isUpperPart = true, bool canGetPointAtBottomPlane = false)
        {
            float phi;
            float theta;

            if (canGetPointAtBottomPlane && UnityEngine.Random.Range(0f, 1f) < 0.5f)
            {
                phi = 0f;
                theta = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
            }
            else
            {
                phi = UnityEngine.Random.Range(0f, Mathf.PI / 2f);
                theta = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
            }

            float hemisphereHeight = isUpperPart ? radius : -radius;

            float x = current.x + radius * Mathf.Sin(phi) * Mathf.Cos(theta);
            float y = current.y + hemisphereHeight + radius * Mathf.Sin(phi) * Mathf.Sin(theta);
            float z = current.z + radius * Mathf.Cos(phi);

            return new Vector3(x, y, z);
        }
        public static Vector3 RandomPositionOnSphere(this Vector3 current, float radius)
        {
            float theta = Random.Range(0f, Mathf.PI * 2);
            float phi = Random.Range(0f, Mathf.PI);
            float x = current.x + radius * Mathf.Sin(phi) * Mathf.Cos(theta);
            float y = current.y + radius * Mathf.Sin(phi) * Mathf.Sin(theta);
            float z = current.z + radius * Mathf.Cos(phi);

            return new Vector3(x, y, z);
        }
        public static Vector2 RandomPosition(this Vector2 from, Vector2 to)
        {
            return new Vector2(Random.Range(from.x, to.x), Random.Range(from.y, to.y));
        }
        public static float RandomFloating(this Vector2 randomNum)
        {
            return Random.Range(randomNum.x, randomNum.y);
        }
        public static Vector2 RandomPositionOnEdgeRectangle(this Transform transform, float width, float height)
        {
            int edgeIndex = Random.Range(0, 4);

            float randomX = 0f;
            float randomY = 0f;

            switch (edgeIndex)
            {
                case 0: // Top edge
                    randomX = Random.Range(transform.position.x - width / 2, transform.position.x + width / 2);
                    randomY = transform.position.y + height / 2;
                    break;
                case 1: // Bottom edge
                    randomX = Random.Range(transform.position.x - width / 2, transform.position.x + width / 2);
                    randomY = transform.position.y - height / 2;
                    break;
                case 2: // Left edge
                    randomX = transform.position.x - width / 2;
                    randomY = Random.Range(transform.position.y - height / 2, transform.position.y + height / 2);
                    break;
                case 3: // Right edge
                    randomX = transform.position.x + width / 2;
                    randomY = Random.Range(transform.position.y - height / 2, transform.position.y + height / 2);
                    break;
            }

            return new Vector2(randomX, randomY);
        }

        public static Vector2 RandomPositionInsideRectangle(this Transform transform, float width, float height)
        {
            float randomX = Random.Range(transform.position.x - width / 2, transform.position.x + width / 2);
            float randomY = Random.Range(transform.position.y - height / 2, transform.position.y + height / 2);
            return new Vector2(randomX, randomY);
        }

        public static Vector2 RandomPositionOutsideRectangle(this Transform transform, float width, float height, float outsideBuffer)
        {
            int edgeIndex = Random.Range(0, 4);

            float randomX = 0f;
            float randomY = 0f;

            switch (edgeIndex)
            {
                case 0: // Top edge
                    randomX = Random.Range(transform.position.x - width / 2, transform.position.x + width / 2);
                    randomY = Random.Range(transform.position.y + height / 2, transform.position.y + (height + outsideBuffer) / 2);
                    break;
                case 1: // Bottom edge
                    randomX = Random.Range(transform.position.x - width / 2, transform.position.x + width / 2);
                    randomY = Random.Range(transform.position.y - height / 2, transform.position.y - (height + outsideBuffer) / 2);
                    break;
                case 2: // Left edge
                    randomX = Random.Range(transform.position.x - width / 2, transform.position.x - (width + outsideBuffer) / 2);
                    randomY = Random.Range(transform.position.y - height / 2, transform.position.y + height / 2);
                    break;
                case 3: // Right edge
                    randomX = Random.Range(transform.position.x + width / 2, transform.position.x + (width + outsideBuffer) / 2);
                    randomY = Random.Range(transform.position.y - height / 2, transform.position.y + height / 2);
                    break;
            }

            return new Vector2(randomX, randomY);
        }

        public static Vector2 RandomPositionOnCircle(this Transform transform, float radius)
        {
            float angle = Random.Range(0, 2 * Mathf.PI);
            float randomX = transform.position.x + radius * Mathf.Cos(angle);
            float randomY = transform.position.y + radius * Mathf.Sin(angle);
            return new Vector2(randomX, randomY);
        }

        public static Vector2 RandomPositionInsideCircle(this Transform transform, float radius)
        {
            float angle = Random.Range(0, 2 * Mathf.PI);
            float randomRadius = Random.Range(0, radius);
            float randomX = transform.position.x + randomRadius * Mathf.Cos(angle);
            float randomY = transform.position.y + randomRadius * Mathf.Sin(angle);
            return new Vector2(randomX, randomY);
        }

        public static Vector3 RandomPositionOnPlaneInCircle(this Vector3 transform, float radius, Vector3 plane)
        {
            Vector3 randomInSphere = RandomPositionOnHemisphereSurface(transform, radius);
            return new Vector3(randomInSphere.x * plane.x, randomInSphere.y * plane.y, randomInSphere.z * plane.z);
        }

        public static Vector2 RandomPositionOutsideCircle(this Transform transform, float radius, float outsideBuffer)
        {
            float innerRadius = radius;
            float outerRadius = radius + outsideBuffer;
            float angle = Random.Range(0f, 2f * Mathf.PI);
            float randomRadius = Random.Range(innerRadius, outerRadius);
            float x = transform.position.x + randomRadius * Mathf.Cos(angle);
            float y = transform.position.y + randomRadius * Mathf.Sin(angle);

            return new Vector2(x, y);
        }
        public static bool IsNear(this Transform current, Transform target, float distance, float buffer = 0.1f)
        {
            float squareValue = Mathf.Pow(target.position.x - current.position.x, 2) + Mathf.Pow(target.position.y - current.position.y, 2) + Mathf.Pow(target.position.z - current.position.z, 2);
            return squareValue < distance * distance - buffer * buffer;
        }
        public static bool IsNear(this Transform current, Vector3 target, float distance, float buffer = 0.1f)
        {
            float squareValue = Mathf.Pow(target.x - current.position.x, 2) + Mathf.Pow(target.y - current.position.y, 2) + Mathf.Pow(target.z - current.position.z, 2);
            return squareValue < distance * distance - buffer * buffer;
        }
        public static bool IsAtRange(this Transform current, Transform target, float distance, float buffer = 0.1f)
        {
            float squareValue = Mathf.Pow(target.position.x - current.position.x, 2) + Mathf.Pow(target.position.y - current.position.y, 2) + Mathf.Pow(target.position.z - current.position.z, 2);
            return distance * distance - buffer * buffer <= squareValue && squareValue <= distance * distance + buffer * buffer;
        }
        public static bool IsFar(this Transform current, Transform target, float distance, float buffer = 0.1f)
        {
            float squareValue = Mathf.Pow(target.position.x - current.position.x, 2) + Mathf.Pow(target.position.y - current.position.y, 2) + Mathf.Pow(target.position.z - current.position.z, 2);
            return squareValue > distance * distance + buffer * buffer;
        }
        public static bool IsNearAtRange(this Transform current, Transform target, float distance, float buffer = 0.1f)
        {
            return current.IsNear(target, distance, buffer) && current.IsAtRange(target, distance, buffer);
        }
        public static bool IsFarAtRange(this Transform current, Transform target, float distance, float buffer = 0.1f)
        {
            return current.IsFar(target, distance, buffer) && current.IsAtRange(target, distance, buffer);
        }
        public static bool IsLeftOf(this Transform current, Transform target)
        {
            return current.position.x > target.position.x;
        }
        public static bool IsRightOf(this Transform current, Transform target)
        {
            return current.position.x < target.position.x;
        }
        public static bool IsAbove(this Transform current, Transform target)
        {
            return current.position.y > target.position.y;
        }
        public static bool IsBelow(this Transform current, Transform target)
        {
            return current.position.y < target.position.y;
        }
        public static bool IsInFrontOf(this Transform current, Transform target)
        {
            return current.position.z > target.position.z;
        }
        public static bool IsBehind(this Transform current, Transform target)
        {
            return current.position.z < target.position.z;
        }
        public static bool IsAt(this Transform current, Transform target)
        {
            return current.position.Equals(target.position);
        }
        public static Vector3 GetDirectionTo(this Transform current, Transform target)
        {
            Vector3 direction = target.position - current.position;
            return direction;
        }
        public static Vector3 GetNormalizedDirectionTo(this Transform current, Transform target)
        {
            return current.GetDirectionTo(target).normalized;
        }
        public static float GetDistance(this Transform current, Transform target)
        {
            return Vector3.Magnitude(current.GetDirectionTo(target));
        }



        // * Physics
        public static List<Collider2D> FindCollidersInRange(this Transform transform, float range, LayerMask layer)
        {
            Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, range, layer);
            List<Collider2D> result = new();
            foreach (var col in cols)
            {
                result.Add(col);
            }
            return result;
        }
        public static Collider2D FindFirstColliderInRange(this Transform transform, float range, LayerMask layer)
        {
            Collider2D col = Physics2D.OverlapCircle(transform.position, range, layer);
            return col;
        }



        // * Enum
        public static string ToISO(this eLanguage language) => language switch
        {
            eLanguage.English => "en",
            eLanguage.Simplified_Chinese => "zh",
            _ => "en"
        };
        public static eLanguage ToLanguage(this string iso) => iso switch
        {
            "en" => eLanguage.English,
            "zh" => eLanguage.Simplified_Chinese,
            _ => eLanguage.English
        };


        // * String
        public static string ChangeColorAtIndex(this string rawText, int index, Color color, List<string> characterList)
        {
            int count = rawText.Length;
            string colorString = "#" + ColorUtility.ToHtmlStringRGBA(color);
            characterList[index] = string.Format("<color={0}>{1}</color>", colorString, characterList[index]);
            string result = string.Empty;
            for (int i = 0; i < count; i++)
            {
                result += characterList[i];
            }
            return result;
        }
        public static List<string> ToCharacterList(this string text)
        {
            List<string> characterList = new();
            foreach (var c in text)
            {
                characterList.Add(c.ToString());
            }
            return characterList;
        }
    }

    public static class CommonUtilities
    {
        public static string GetTextFileContent(string path)
        {
            StreamReader reader = new StreamReader(path);
            string result = reader.ReadToEnd();
            reader.Close();
            return result;
        }
    }
}
