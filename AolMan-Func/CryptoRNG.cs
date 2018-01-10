using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AolManFunc
{
    public class CryptoRandom : Random, IDisposable
    {
        private RNGCryptoServiceProvider _rng = new RNGCryptoServiceProvider();
        private byte[] _uint32Buffer = new byte[4];

        public CryptoRandom() { }
        public CryptoRandom(int ignoredSeed) { }

        public override int Next()
        {
            _rng.GetBytes(_uint32Buffer);
            return BitConverter.ToInt32(_uint32Buffer, 0) & 0x7FFFFFFF;
        }

        public override int Next(int maxValue)
        {
            if (maxValue < 0)
                throw new ArgumentOutOfRangeException(nameof(maxValue));
            return Next(0, maxValue);
        }

        public override int Next(int minValue, int maxValue)
        {
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException(nameof(minValue));
            if (minValue == maxValue) return minValue;
            long diff = maxValue - minValue;
            while (true)
            {
                _rng.GetBytes(_uint32Buffer);
                uint rand = BitConverter.ToUInt32(_uint32Buffer, 0);

                long max = (1 + (long)uint.MaxValue);
                long remainder = max % diff;
                if (rand < max - remainder)
                {
                    return (int)(minValue + (rand % diff));
                }
            }
        }

        public override double NextDouble()
        {
            _rng.GetBytes(_uint32Buffer);
            uint rand = BitConverter.ToUInt32(_uint32Buffer, 0);
            return rand / (1.0 + uint.MaxValue);
        }

        public override void NextBytes(byte[] buffer)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            _rng.GetBytes(buffer);
        }

        #region IDisposable
        private bool _disposed = false; // To detect redundant calls

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_rng != null)
                    {
                        _rng.Dispose();
                    }
                }
            }

            _rng = null;
            _disposed = true;

        }
        #endregion
    }
}
