using FixMath.NET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct FixVector3
{
    public Fix64 x;
    public Fix64 y;
    public Fix64 z;

    public FixVector3(Fix64 x, Fix64 y, Fix64 z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public FixVector3(FixVector3 v)
    {
        this.x = v.x;
        this.y = v.y;
        this.z = v.z;
    }

    public Fix64 this[int index]
    {
        get
        {
            if (index == 0)
                return x;
            else if (index == 1)
                return y;
            else
                return z;
        }
        set
        {
            if (index == 0)
                x = value;
            else if (index == 1)
                y = value;
            else
                y = value;
        }
    }

    public static FixVector3 Zero
    {
        get { return new FixVector3(Fix64.Zero, Fix64.Zero, Fix64.Zero); }
    }

    public static FixVector3 operator +(FixVector3 a, FixVector3 b)
    {
        Fix64 x = a.x + b.x;
        Fix64 y = a.y + b.y;
        Fix64 z = a.z + b.z;
        return new FixVector3(x, y, z);
    }

    public static FixVector3 operator -(FixVector3 a, FixVector3 b)
    {
        Fix64 x = a.x - b.x;
        Fix64 y = a.y - b.y;
        Fix64 z = a.z - b.z;
        return new FixVector3(x, y, z);
    }

    public static FixVector3 operator *(Fix64 d, FixVector3 a)
    {
        Fix64 x = a.x * d;
        Fix64 y = a.y * d;
        Fix64 z = a.z * d;
        return new FixVector3(x, y, z);
    }

    public static FixVector3 operator *(FixVector3 a, Fix64 d)
    {
        Fix64 x = a.x * d;
        Fix64 y = a.y * d;
        Fix64 z = a.z * d;
        return new FixVector3(x, y, z);
    }

    public static FixVector3 operator /(FixVector3 a, Fix64 d)
    {
        Fix64 x = a.x / d;
        Fix64 y = a.y / d;
        Fix64 z = a.z / d;
        return new FixVector3(x, y, z);
    }

    public static bool operator ==(FixVector3 lhs, FixVector3 rhs)
    {
        return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;
    }

    public static bool operator !=(FixVector3 lhs, FixVector3 rhs)
    {
        return lhs.x != rhs.x || lhs.y != rhs.y || lhs.z != rhs.z;
    }

    public static Fix64 SqrMagnitude(FixVector3 a)
    {
        return a.x * a.x + a.y * a.y + a.z * a.z;
    }

    public static Fix64 Distance(FixVector3 a, FixVector3 b)
    {
        return Magnitude(a - b);
    }

    public static Fix64 Magnitude(FixVector3 a)
    {
        return Fix64.Sqrt(FixVector3.SqrMagnitude(a));
    }

    public void Normalize()
    {
        Fix64 n = x * x + y * y + z * z;
        if (n == Fix64.Zero)
            return;

        n = Fix64.Sqrt(n);

        if (n < (Fix64)0.0001)
        {
            return;
        }

        n = 1 / n;
        x *= n;
        y *= n;
        z *= n;
    }

    public FixVector3 GetNormalized()
    {
        FixVector3 v = new FixVector3(this);
        v.Normalize();
        return v;
    }

    public override string ToString()
    {
        return string.Format("x:{0} y:{1} z:{2}", x, y, z);
    }

    public override bool Equals(object obj)
    {
        return obj is FixVector2 && ((FixVector3)obj) == this;
    }

    public override int GetHashCode()
    {
        return this.x.GetHashCode() + this.y.GetHashCode() + this.z.GetHashCode();
    }

    public static FixVector3 Lerp(FixVector3 from, FixVector3 to, Fix64 factor)
    {
        return from * (1 - factor) + to * factor;
    }
#if _CLIENTLOGIC_
    public UnityEngine.Vector3 ToVector3()
    {
        return new UnityEngine.Vector3((float)x, (float)y, (float)z);
    }
#endif
}

public struct FixVector2
{
    public Fix64 x;
    public Fix64 y;

    public FixVector2(Fix64 x, Fix64 y)
    {
        this.x = x;
        this.y = y;
    }
    public FixVector2(Fix64 x, int y)
    {
        this.x = x;
        this.y = (Fix64)y;
    }

    public FixVector2(int x, int y)
    {
        this.x = (Fix64)x;
        this.y = (Fix64)y;
    }
    public FixVector2(FixVector2 v)
    {
        this.x = v.x;
        this.y = v.y;
    }
    public static FixVector2 operator -(FixVector2 a, int b)
    {
        Fix64 x = a.x - b;
        Fix64 y = a.y - b;
        return new FixVector2(x, y);
    }

    public Fix64 this[int index]
    {
        get { return index == 0 ? x : y; }
        set
        {
            if (index == 0)
            {
                x = value;
            }
            else
            {
                y = value;
            }
        }
    }

    public static FixVector2 Zero
    {
        get { return new FixVector2(Fix64.Zero, Fix64.Zero); }
    }

    public static FixVector2 operator +(FixVector2 a, FixVector2 b)
    {
        Fix64 x = a.x + b.x;
        Fix64 y = a.y + b.y;
        return new FixVector2(x, y);
    }

    public static FixVector2 operator -(FixVector2 a, FixVector2 b)
    {
        Fix64 x = a.x - b.x;
        Fix64 y = a.y - b.y;
        return new FixVector2(x, y);
    }

    public static FixVector2 operator *(Fix64 d, FixVector2 a)
    {
        Fix64 x = a.x * d;
        Fix64 y = a.y * d;
        return new FixVector2(x, y);
    }

    public static FixVector2 operator *(FixVector2 a, Fix64 d)
    {
        Fix64 x = a.x * d;
        Fix64 y = a.y * d;
        return new FixVector2(x, y);
    }

    public static FixVector2 operator /(FixVector2 a, Fix64 d)
    {
        Fix64 x = a.x / d;
        Fix64 y = a.y / d;
        return new FixVector2(x, y);
    }

    public static bool operator ==(FixVector2 lhs, FixVector2 rhs)
    {
        return lhs.x == rhs.x && lhs.y == rhs.y;
    }

    public static bool operator !=(FixVector2 lhs, FixVector2 rhs)
    {
        return lhs.x != rhs.x || lhs.y != rhs.y;
    }

    public override bool Equals(object obj)
    {
        return obj is FixVector2 && ((FixVector2)obj) == this;
    }

    public override int GetHashCode()
    {
        return this.x.GetHashCode() + this.y.GetHashCode();
    }


    public static Fix64 SqrMagnitude(FixVector2 a)
    {
        return a.x * a.x + a.y * a.y;
    }

    public static Fix64 Distance(FixVector2 a, FixVector2 b)
    {
        return Magnitude(a - b);
    }

    public static Fix64 Magnitude(FixVector2 a)
    {
        return Fix64.Sqrt(FixVector2.SqrMagnitude(a));
    }

    public void Normalize()
    {
        Fix64 n = x * x + y * y;
        if (n == Fix64.Zero)
            return;

        n = Fix64.Sqrt(n);

        if (n < (Fix64)0.0001)
        {
            return;
        }

        n = 1 / n;
        x *= n;
        y *= n;
    }

    public FixVector2 GetNormalized()
    {
        FixVector2 v = new FixVector2(this);
        v.Normalize();
        return v;
    }

    public override string ToString()
    {
        return string.Format("x:{0} y:{1}", x, y);
    }

#if _CLIENTLOGIC_
    public UnityEngine.Vector2 ToVector2()
    {
        return new UnityEngine.Vector2((float)x, (float)y);
    }
#endif
}

public struct NormalVector2
{
    public float x;
    public float y;

    public NormalVector2(float x, float y)
    {
        this.x = x;
        this.y = y;
    }


    public NormalVector2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public NormalVector2(NormalVector2 v)
    {
        this.x = v.x;
        this.y = v.y;
    }
    public static NormalVector2 operator -(NormalVector2 a, int b)
    {
        float x = a.x - b;
        float y = a.y - b;
        return new NormalVector2(x, y);
    }

    public float this[int index]
    {
        get { return index == 0 ? x : y; }
        set
        {
            if (index == 0)
            {
                x = value;
            }
            else
            {
                y = value;
            }
        }
    }

    public static NormalVector2 Zero
    {
        get { return new NormalVector2(0, 0); }
    }

    public static NormalVector2 operator +(NormalVector2 a, NormalVector2 b)
    {
        float x = a.x + b.x;
        float y = a.y + b.y;
        return new NormalVector2(x, y);
    }

    public static NormalVector2 operator -(NormalVector2 a, NormalVector2 b)
    {
        float x = a.x - b.x;
        float y = a.y - b.y;
        return new NormalVector2(x, y);
    }

    public static NormalVector2 operator *(float d, NormalVector2 a)
    {
        float x = a.x * d;
        float y = a.y * d;
        return new NormalVector2(x, y);
    }

    public static NormalVector2 operator *(NormalVector2 a, float d)
    {
        float x = a.x * d;
        float y = a.y * d;
        return new NormalVector2(x, y);
    }

    public static NormalVector2 operator /(NormalVector2 a, float d)
    {
        float x = a.x / d;
        float y = a.y / d;
        return new NormalVector2(x, y);
    }

    public static bool operator ==(NormalVector2 lhs, NormalVector2 rhs)
    {
        return lhs.x == rhs.x && lhs.y == rhs.y;
    }

    public static bool operator !=(NormalVector2 lhs, NormalVector2 rhs)
    {
        return lhs.x != rhs.x || lhs.y != rhs.y;
    }

    public override bool Equals(object obj)
    {
        return obj is NormalVector2 && ((NormalVector2)obj) == this;
    }

    public override int GetHashCode()
    {
        return this.x.GetHashCode() + this.y.GetHashCode();
    }


    public static float SqrMagnitude(NormalVector2 a)
    {
        return a.x * a.x + a.y * a.y;
    }

    public static float Distance(NormalVector2 a, NormalVector2 b)
    {
        return Magnitude(a - b);
    }

    public static float Magnitude(NormalVector2 a)
    {
        return NormalVector2.SqrMagnitude(a);
    }

    public void Normalize()
    {
        float n = x * x + y * y;
        if (n == 0)
            return;

        //n = float.Sqrt(n);

        if (n < (float)0.0001)
        {
            return;
        }

        n = 1 / n;
        x *= n;
        y *= n;
    }

    public NormalVector2 GetNormalized()
    {
        NormalVector2 v = new NormalVector2(this);
        v.Normalize();
        return v;
    }

    public override string ToString()
    {
        return string.Format("x:{0} y:{1}", x, y);
    }

#if _CLIENTLOGIC_
    public UnityEngine.Vector2 ToVector2()
    {
        return new UnityEngine.Vector2((float)x, (float)y);
    }
#endif
}

public struct IntVector2
{
    public int x;
    public int y;



    public IntVector2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public IntVector2(IntVector2 v)
    {
        this.x = v.x;
        this.y = v.y;
    }
    public static IntVector2 operator -(IntVector2 a, int b)
    {
        int x = a.x - b;
        int y = a.y - b;
        return new IntVector2(x, y);
    }

    public int this[int index]
    {
        get { return index == 0 ? x : y; }
        set
        {
            if (index == 0)
            {
                x = value;
            }
            else
            {
                y = value;
            }
        }
    }

    public static IntVector2 Zero
    {
        get { return new IntVector2(0, 0); }
    }

    public static IntVector2 operator +(IntVector2 a, IntVector2 b)
    {
        int x = a.x + b.x;
        int y = a.y + b.y;
        return new IntVector2(x, y);
    }

    public static IntVector2 operator -(IntVector2 a, IntVector2 b)
    {
        int x = a.x - b.x;
        int y = a.y - b.y;
        return new IntVector2(x, y);
    }

    public static IntVector2 operator *(int d, IntVector2 a)
    {
        int x = a.x * d;
        int y = a.y * d;
        return new IntVector2(x, y);
    }

    public static IntVector2 operator *(IntVector2 a, int d)
    {
        int x = a.x * d;
        int y = a.y * d;
        return new IntVector2(x, y);
    }

    public static IntVector2 operator /(IntVector2 a, int d)
    {
        int x = a.x / d;
        int y = a.y / d;
        return new IntVector2(x, y);
    }

    public static bool operator ==(IntVector2 lhs, IntVector2 rhs)
    {
        return lhs.x == rhs.x && lhs.y == rhs.y;
    }

    public static bool operator !=(IntVector2 lhs, IntVector2 rhs)
    {
        return lhs.x != rhs.x || lhs.y != rhs.y;
    }

    public override bool Equals(object obj)
    {
        return obj is IntVector2 && ((IntVector2)obj) == this;
    }

    public override int GetHashCode()
    {
        return this.x.GetHashCode() + this.y.GetHashCode();
    }


    public static int SqrMagnitude(IntVector2 a)
    {
        return a.x * a.x + a.y * a.y;
    }

    public static int Distance(IntVector2 a, IntVector2 b)
    {
        return Magnitude(a - b);
    }

    public static int Magnitude(IntVector2 a)
    {
        return IntVector2.SqrMagnitude(a);
    }

    public void Normalize()
    {
        int n = x * x + y * y;
        if (n == 0)
            return;

        //n = int.Sqrt(n);

        if (n < (int)0.0001)
        {
            return;
        }

        n = 1 / n;
        x *= n;
        y *= n;
    }

    public IntVector2 GetNormalized()
    {
        IntVector2 v = new IntVector2(this);
        v.Normalize();
        return v;
    }

    public override string ToString()
    {
        return string.Format("x:{0} y:{1}", x, y);
    }

#if _CLIENTLOGIC_
    public UnityEngine.Vector2 ToVector2()
    {
        return new UnityEngine.Vector2((int)x, (int)y);
    }
#endif
}

public struct FixVector4
{
    public Fix64 x;
    public Fix64 y;
    public Fix64 z;
    public Fix64 w;

    public FixVector4(Fix64 x, Fix64 y, Fix64 z, Fix64 w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    public FixVector4(FixVector4 v)
    {
        this.x = v.x;
        this.y = v.y;
        this.z = v.z;
        this.w = v.w;
    }

    public Fix64 this[int index]
    {
        get
        {
            if (index == 0)
                return x;
            else if (index == 1)
                return y;
            else if (index == 2)
                return z;
            else
                return w;
        }
        set
        {
            if (index == 0)
                x = value;
            else if (index == 1)
                y = value;
            else if (index == 2)
                z = value;
            else
                w = value;
        }
    }

    public static FixVector4 Zero
    {
        get { return new FixVector4(Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.Zero); }
    }

    public static FixVector4 operator +(FixVector4 a, FixVector4 b)
    {
        Fix64 x = a.x + b.x;
        Fix64 y = a.y + b.y;
        Fix64 z = a.z + b.z;
        Fix64 w = a.w + b.w;
        return new FixVector4(x, y, z, w);
    }

    public static FixVector4 operator -(FixVector4 a, FixVector4 b)
    {
        Fix64 x = a.x - b.x;
        Fix64 y = a.y - b.y;
        Fix64 z = a.z - b.z;
        Fix64 w = a.w - b.w;
        return new FixVector4(x, y, z, w);
    }

    public static FixVector4 operator *(Fix64 d, FixVector4 a)
    {
        Fix64 x = a.x * d;
        Fix64 y = a.y * d;
        Fix64 z = a.z * d;
        Fix64 w = a.w * d;
        return new FixVector4(x, y, z, w);
    }

    public static FixVector4 operator *(FixVector4 a, Fix64 d)
    {
        Fix64 x = a.x * d;
        Fix64 y = a.y * d;
        Fix64 z = a.z * d;
        Fix64 w = a.w * d;
        return new FixVector4(x, y, z, w);
    }

    public static FixVector4 operator /(FixVector4 a, Fix64 d)
    {
        Fix64 x = a.x / d;
        Fix64 y = a.y / d;
        Fix64 z = a.z / d;
        Fix64 w = a.w / d;
        return new FixVector4(x, y, z, w);
    }

    public static bool operator ==(FixVector4 lhs, FixVector4 rhs)
    {
        return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z && lhs.w == rhs.w;
    }

    public static bool operator !=(FixVector4 lhs, FixVector4 rhs)
    {
        return lhs.x != rhs.x || lhs.y != rhs.y || lhs.z != rhs.z || lhs.w != rhs.w;
    }

    public override string ToString()
    {
        return string.Format("x:{0} y:{1} z:{2} w:{3}", x, y, z, w);
    }

    public override bool Equals(object obj)
    {
        return obj is FixVector3 && ((FixVector4)obj) == this;
    }

    public override int GetHashCode()
    {
        return this.x.GetHashCode() + this.y.GetHashCode() + this.z.GetHashCode() + this.w.GetHashCode();
    }

    public static FixVector4 Lerp(FixVector4 from, FixVector4 to, Fix64 factor)
    {
        return from * (1 - factor) + to * factor;
    }
#if _CLIENTLOGIC_
    public UnityEngine.Quaternion ToVector4()
    {
        return new UnityEngine.Quaternion((float)x, (float)y, (float)z, (float)w);
    }
#endif
}