using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using OmniQuery.Test;
using NamespaceTest;

[assembly: Assembly("i.am.assembly", 666, true, InnerField="inner field", InnerProperty="inner property")]
[module: Module]

namespace NamespaceTest
{
    public class OuterGeneric<O>
    {
        public class InnerGeneric<I>
        {
        }

        public class InnerNonGeneric
        {
        }
    }

    public class OuterNonGeneric
    {
        public class InnerGeneric<T>
        {
        }

        public class InnerNonGeneric
        {
        }
    }
}

class TestClass
{
    public OuterGeneric<int>.InnerGeneric<int> Method1()
    {
        return null;
    }

    public OuterGeneric<int>.InnerNonGeneric Method2()
    {
        return null;
    }

    public OuterNonGeneric.InnerGeneric<int> Method3()
    {
        return null;
    }

    public OuterNonGeneric.InnerNonGeneric Method4()
    {
        return null;
    }
}

namespace OmniQuery.Test
{
    namespace _SampleNamespace
    {
        [Class]
        public class SampleClass<[GenericParameter] T> : SampleInterface
        {
            [DllImport("user32.dll", CharSet = CharSet.Unicode)]
            public static extern int MessageBox(IntPtr hWnd, String text, String caption, uint type);

            [Delegate]
            [return: ReturnValue]
            public delegate void SampleDelegate(int x);

            [Event]
            public event SampleDelegate SampleEvent;

            [Field]
            public int SampleField;

            [Property]
            public int SampleProperty
            {
                get { return SampleField; }
            }

            [Constructor]
            public SampleClass()
            {
            }

            [Method]
            [return: ReturnValue]
            public void SampleGenericMethod<[GenericParameter] P>([Parameter] P x)
            {
                SampleEvent = new SampleDelegate(SampleInterfaceMethod);
                SampleInterfaceMethod();
            }

            public void SampleInterfaceMethod([Parameter] int x = 5)
            {
            }

            private void SampleInterfaceMethod(bool y, System.Random random, System.Reflection.Assembly assembly, CodeAnalytics.CodeAnalysisDb info, SampleClass<Microsoft.SqlServer.Server.SqlContext> context)
            {
            }

            internal class Internal_Class<IC>
            {
                internal T DoT(T t)
                {
                    return t;
                }

                internal IC DoIC(IC ic)
                {
                    return ic;
                }
            }

            private class Private_Class
            {
                public delegate void Internal_Delegate_123<Z>();
                public delegate void Another_Delegate();

                private Internal_Class<Mono.Collections.Generic.Collection<System.Collections.Generic.KeyValuePair<int, Internal_Delegate_123<DateTime>>>>
                    ComplexMethod<O, M, G>(IEnumerable<SampleClass<IList<SampleDelegate>>> blah, System.Reflection.Assembly assembly, SampleClass<System.Random> random, O o, M m, G g)
                {
                    return null;
                }

                private unsafe KeyValuePair<int*[,][][,,][][,,,], Another_Delegate> UnsafeMethod(int*[,][][,,][][,,,] pointerArray)
                {
                    return new KeyValuePair<int*[,][][,,][][,,,], Another_Delegate>();
                }

                private class _Underscore<_under>
                {
                }

                private class МойКласс<класс>
                {
                }

                private class ŠŠŠŠŠ<ŽŽŽ>
                {
                }

                private class @class<@params>
                {
                }

                private _Underscore<МойКласс<ŠŠŠŠŠ<@class<int*[]>[]>[]>[]>[] ArrayMethod()
                {
                    return null;
                }
            }
        }
    }

    public class TwoParams<One, Two>
    {
    }

    public class InheritParms<Child> : TwoParams<Child, int>
    {
    }

    [Interface]
    public interface SampleInterface
    {
        [Method]
        void SampleInterfaceMethod([Parameter] int x);
    }

    [Enum]
    public enum SampleEnum
    {
        [Field]
        SampleEnumField1,
        [Field]
        SampleEnumField2
    }

    [Struct]
    public struct SampleStruct
    {
    }
}
