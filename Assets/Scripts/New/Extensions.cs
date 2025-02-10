using Zenject;


    public static class Extensions
    {
        public static void BindWithInterfaces<T>(this DiContainer container)
        {
            container.BindInterfacesAndSelfTo<T>().AsSingle().NonLazy();
        }

        public static void BindWithInterfacesLazy<T>(this DiContainer container)
        {
            container.BindInterfacesAndSelfTo<T>().AsSingle();
        }
    }