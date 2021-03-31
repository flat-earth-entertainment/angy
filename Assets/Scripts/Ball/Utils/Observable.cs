using System.Collections.Generic;

namespace Ball.Utils
{
    public class Observable<T>
    {
        public delegate void ChangeValue(T v);

        private T _v;

        public Observable(T value)
        {
            _v = value;
        }

        public T Val
        {
            get => _v;
            set
            {
                var previousValue = _v;

                if (!EqualityComparer<T>.Default.Equals(previousValue, value))
                {
                    _v = value;

                    if (PropertyUpdated != null)
                    {
                        PropertyUpdated(_v);
                    }
                }
            }
        }

        public event ChangeValue PropertyUpdated;
    }
}