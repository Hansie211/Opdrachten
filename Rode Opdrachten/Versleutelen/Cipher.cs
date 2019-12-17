using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opdracht3 {

    public static class CRC32 {

        private static uint[] crctable;

        public static uint Checksum( byte[] bytes ) {

            uint result = 0xFFFFFFFF;

            for ( int i = 0; i < bytes.Length; i++ ) {

                byte index = (byte)( ((result) & 0xFF) ^ bytes[i]) ;
                result = (uint)( ( result >> 8 ) ^ crctable[index] );
            }

            return ~result;
        }

        public static uint Checksum( string bytes ) {

            return Checksum( Encoding.ASCII.GetBytes(bytes) );
        }

        public static uint Checksum( uint bytes ) {

            return Checksum( new byte[] { 
                (byte)((bytes >> 24) & 0xFF),
                (byte)((bytes >> 16) & 0xFF),
                (byte)((bytes >> 8) & 0xFF),
                (byte)((bytes >> 0) & 0xFF) 
            } );
        }

        public static void init() {
            uint polynomial = 0xEDB88320;

            crctable = new uint[256];

            // Init the table
            for ( uint i = 0; i < crctable.Length; i++ ) {

                uint temp = i;
                for ( int j = 8; j > 0; j-- ) {

                    if ( (temp & 1) == 1 ) {

                        temp = (uint)( ( temp >> 1 ) ^ polynomial );
                    } else {

                        temp >>= 1;
                    }
                }

                crctable[i] = temp;
            }
        }
    }

    static class CipherManager {

        static public Cipher[] ciphers;


        static public void init() {

            CRC32.init();
            ciphers = new Cipher[3];

            ciphers[0] = new CipherCaesar();
            ciphers[1] = new CipherXOR();
            ciphers[2] = new CipherSP();
        }

        static public Boolean encrypt( byte bitmode, int cipherIndex, string password, string fileName, out byte[] data ) {

            return ciphers[cipherIndex].encrypt( bitmode, password, fileName, out data );
        }
        static public Boolean decrypt( byte bitmode, int cipherIndex, string password, string fileName, out byte[] data ) {

            return ciphers[cipherIndex].decrypt( bitmode, password, fileName, out data );
        }

    }

    abstract class Cipher {

        protected string name;
        protected byte paddingSize = 1;
        protected Boolean usePadding = false;

        public string getName() {
            return name;
        }

        protected void addChecksum( ref byte[] data, uint crc32 ) {

            Array.Resize( ref data, data.Length + 4);
            setBlock( ref data, crc32, data.Length - 4, data.Length, 4);
        }

        protected uint getChecksum( byte[] data ) {

            return getBlock( data, data.Length - 4, data.Length, 4);
        }

        protected uint getBlock( byte[] data, long idx, long max, byte size ) {

            uint result = 0;

            for( long i = 0; i < size; i++ ) {

                if ( i + idx > max - 1 ) { // prevent overflow
                    break;
                }

                result |= (uint)(data[i + idx] << (byte)(i * 8));
            }

            return result;
        }

        protected void setBlock( ref byte[] data, uint block, long idx, long max, byte size ) {

            for ( long i = 0; i < size; i++ ) { // prevent overflow

                if ( i + idx > max - 1 ) {
                    return;
                }

                data[i+idx] = (byte)(( block >> (byte)(i*8) ) & 0xFF );
            }

        }

        protected void getBitmode( byte bitmode, out byte bufsize, out long blocksize ) {

            switch ( bitmode ) {
                default:
                case 8:
                    bufsize = 1;
                    blocksize = (long)0xFF;
                    return;

                case 16:
                    bufsize = 2;
                    blocksize = (long)0xFFFF;
                    return;

                case 24:
                    bufsize = 3;
                    blocksize = (long)0xFFFFFF;
                    return;

                case 32:
                    bufsize = 4;
                    blocksize = (long)0xFFFFFFFF;
                    return;
            }

        }

        private Boolean loadFile( string fileName, out byte[] fileData  ) {

            FileStream stream;
            try {
                stream = new FileStream( fileName, FileMode.Open, FileAccess.Read );
            }
            catch ( Exception e ) {

                Console.WriteLine( e.Message );

                fileData = new byte[0];
                return false;
            }

            // Read all data to memory
            fileData = new byte[stream.Length];
            stream.Read( fileData, 0, (int)stream.Length );

            // Close the file
            stream.Close();

            return true;
        }

        protected void applyPadding(ref byte[] data) {

            byte psize = (byte)(paddingSize - (data.Length % paddingSize));
            if ( psize == 0 ) {
                psize = paddingSize;
            }

            // Resize to array ( include the padding )
            Array.Resize( ref data, data.Length + psize);
            data[ data.Length - 1] = psize; // store the padding size in the last byte
        }

        protected void stripPadding(ref byte[] data ) {

            // padding size is in the last byte
            byte psize = data[data.Length-1];
            Array.Resize( ref data, data.Length - psize );
        }

        protected Boolean preparePlainFile( string filename, out byte[] fileData, out byte[] bufferData, out uint crc32 ){

            crc32 = 0;

            if ( !loadFile( filename, out fileData ) ) {

                bufferData = new byte[0];
                return false;
            }

            // Apply the padding to the file
            if ( usePadding ) { 
                applyPadding( ref fileData );
            }

            bufferData = new byte[ fileData.Length ];
            // Get the CRC32
            crc32 = CRC32.Checksum( fileData );


            return true;
        }

        protected Boolean prepareEncryptedFile( string filename, out byte[] fileData, out byte[] bufferData, out uint crc32 ) {
            
            if ( !loadFile( filename, out fileData ) ) {

                crc32       = 0;
                bufferData  = new byte[0];
                return false;
            }

            // Read the CRC32 from the file
            crc32 = getChecksum( fileData );

            // Strip the checksum
            Array.Resize( ref fileData, fileData.Length - 4);

            // Allocate the output buffer
            bufferData = new byte[ fileData.Length ];

            return true;
        }

        protected abstract long encryptBlock( uint block, uint password, long blocksize, byte bitmode );
        protected abstract long decryptBlock( uint block, uint password, long blocksize, byte bitmode );

        protected uint createPassword( string password ) {

            return CRC32.Checksum( password );
        }

        public virtual Boolean encrypt( byte bitmode, string password, string filename, out byte[] encodedData ) {
            
            byte[] fileData;
            uint crc32;

            if ( !preparePlainFile( filename, out fileData, out encodedData, out crc32 ) ) {
                return false;
            }

            uint _password = createPassword(password);

            byte bufsize;
            long blocksize;

            getBitmode( bitmode, out bufsize, out blocksize );
            _password &= (uint)blocksize;

            // Encrypt
            for ( long i = 0; i < fileData.Length; i += bufsize ) {

                // Read data
                uint block  = getBlock( fileData, i, fileData.Length, bufsize );

                // Encrypt
                long result = encryptBlock( block, _password, blocksize, bitmode);
                // Squeeze into blocksize
                result &= (long)blocksize;

                // Write data
                setBlock( ref encodedData, (uint)result, i, fileData.Length, bufsize );
            }

            // Write the checksum
            addChecksum( ref encodedData, crc32 );

            return true;
        }

        public virtual Boolean decrypt( byte bitmode, string password, string filename, out byte[] decodeData ) {
            
            byte[] fileData;
            uint crc32;

            if ( !prepareEncryptedFile( filename, out fileData, out decodeData, out crc32 ) ) {
                return false;
            }

            uint _password = createPassword(password);

            byte bufsize;
            long blocksize;

            getBitmode( bitmode, out bufsize, out blocksize );
            _password &= (uint)blocksize;


            // Decrypt
            for ( long i = 0; i < fileData.Length; i += bufsize ) {

                // Read data
                uint block  = getBlock( fileData, i, fileData.Length, bufsize );

                // Decrypt
                long result = decryptBlock( block, _password, blocksize, bitmode );
                // Squeeze into blocksize
                result &= (long)blocksize;

                // Write data
                setBlock( ref decodeData, (uint)result, i, fileData.Length, bufsize );
            }

            if ( CRC32.Checksum( decodeData ) != crc32 ){
                return false;
            }

            // Remove the padding
            if ( usePadding ) {
                stripPadding( ref decodeData );            
            }

            return true;
        }

        public Cipher( string name, Boolean usePadding ) {
            this.name = name;
            this.usePadding = usePadding;
        }

    }

    class CipherCaesar : Cipher {

        public CipherCaesar() : base( "Caesarian Shift", false ) {

        }

        protected override long encryptBlock( uint block, uint password, long blocksize, byte bitmode ) {
            
            return (long)(block + password);
        }

        protected override long decryptBlock( uint block, uint password, long blocksize, byte bitmode ) {
            
            return (long)(block - password) + (long)( blocksize + 1 );
        }
    }

    class CipherXOR : Cipher {

        public CipherXOR() : base( "Exclusive OR", false ) {

        }

        protected override long encryptBlock( uint block, uint password, long blocksize, byte bitmode ) {

            return (long)( block ^ password );
        }

        protected override long decryptBlock( uint block, uint password, long blocksize, byte bitmode ) {

            return (long)( block ^ password );
        }

    }

    class CipherSP : Cipher {

        private byte[] permutionBox;
        private uint[] blockpw;
        
        private const byte sboxSize = 4;   // 4 bits
        private const byte sboxMax  = 0xF; // 1111 in bits ( 2 ^ sboxSize - 1 )
        private readonly byte[] substitutionBox = { 0xC, 0x5, 0x6, 0xB, 0x9, 0x0, 0xA, 0xD, 0x3, 0xE, 0xF, 0x8, 0x4, 0x7, 0x1, 0x2 }; // from cipher 'PRESENT'

        private const byte COUNT = 30;


        public CipherSP() : base ( "Substitution Permutation", true) {

        }

        private void shuffleList( Random random, ref byte[] list ) {

            for ( int i = 0; i < list.Length; i++ ) {

                int n       = random.Next( list.Length );

                // Swap
                byte tmp    = list[i];
                list[i] = list[n];
                list[n] = tmp;
            }
        }

        private void shuffleList( Random random, ref uint[] list ) {

            for ( int i = 0; i < list.Length; i++ ) {

                int n       = random.Next( list.Length );

                // Swap
                uint tmp    = list[i];
                list[i]     = list[n];
                list[n]     = tmp;
            }
        }

        private void setupTable( string password, byte bitmode ) {

            paddingSize     = (byte)(bitmode / 8);

            uint _password  = createPassword(password);
            permutionBox    = new byte[bitmode];
            blockpw         = new uint[COUNT];

            // Setup a random generator
            Random random = new Random( (int)_password ); // set the seed to the current password, so we get the same table with the same password


            // Setup the permutation box
            for ( int i = 0; i < permutionBox.Length; i++ ) {

                permutionBox[i] = (byte)i;
            }

            // Shuffle the permutation box
            shuffleList( random, ref permutionBox );


            // Generate some passwords from the base password
            for ( int i = 0; i < COUNT; i++ ) {
                blockpw[i] = CRC32.Checksum( (uint)random.Next() );
            }

            // Shuffle the the passwords
            shuffleList( random, ref blockpw);
        }

        private Boolean getBit( uint u, int pos ) {

            // Return true if a specific bit is set
            return ( u & ( 1 << pos ) ) > 0;
        }

        private uint setBit( uint u, int pos ) {

            // Set a bit to true
            return ( u | (uint)( 1 << pos ) );
        }

        private byte getReverseTableIndex( byte pos, byte[] list ) {

            for ( byte i = 0; i < list.Length; i++ ) {
                if ( list[i] == pos ) {
                    return i;
                }
            }

            return 0xFF;
        }

        private uint substitute( uint source, byte maxbits, Boolean reverse ) {

            uint result = 0;

            for( int i = 0; i < (maxbits / sboxSize); i++ ) {

                // Get first block
                byte subv = (byte)( source & sboxMax );

                // Get the substitute for this block
                if ( reverse ) {
                    subv = getReverseTableIndex( subv, substitutionBox );
                } else { 
                    subv = substitutionBox[subv];
                }

                // Allocate the new block
                result <<= sboxSize;
                // Add the substitute to the result
                result |= subv;

                // Remove the old block
                source >>= sboxSize;
            }

            return result;
        }

        private uint permutate( uint n, Boolean reverse ) {

            uint result = 0;

            for ( int i = 0; i < permutionBox.Length; i++ ) {

                if ( getBit( n, i ) ) { // if the bit is set

                    byte idx;

                    // find the 'new' location of this bit
                    if ( reverse ) {
                        idx = getReverseTableIndex( (byte)i, permutionBox );
                    } else {
                        idx = permutionBox[i];
                    }

                    // set the bit in the result
                    result = setBit( result, idx );                    
                }
            }

            return result;
        }

        private long encryptByTable( uint block, uint password, byte bitmode ) {

            uint result = block;

            // Substitute
            result = substitute( result, bitmode, false );
            // Permutate
            result = permutate( result, true );

            // Final XOR
            result ^= password;

            return (long)result;
        }

        private long decryptByTable( uint block, uint password, byte bitmode ) {

            // "Reverse encrypt"

            uint result = block;

            // Start with XOR
            result ^= password;

            // Permutate
            result = permutate( result, false );
            // Substitute
            result = substitute( result, bitmode, true );

            return result;
        }

        protected override long encryptBlock( uint block, uint password, long blocksize, byte bitmode ) {

            long result = (long)block;
            for ( int i = 0; i < COUNT; i++ ) {

                result = encryptByTable( (uint)result, (uint)( blockpw[i] & blocksize), bitmode );
            }

            return result;
        }

        protected override long decryptBlock( uint block, uint password, long blocksize, byte bitmode ) {

            long result = (long)block;
            for ( int i = 0; i < COUNT; i++ ) {

                uint _blockpw = blockpw[COUNT - (i+1)]; // reverse password order

                result = decryptByTable( (uint)result, (uint)(_blockpw & blocksize), bitmode );
            }

            return result;
        }

        public override Boolean decrypt( byte bitmode, string password, string filename, out byte[] decodeData ) {

            // Hook to build table
            setupTable( password, bitmode );

            return base.decrypt( bitmode, password, filename, out decodeData );
        }

        public override Boolean encrypt( byte bitmode, string password, string filename, out byte[] encodedData ) {

            // Hook to build table
            setupTable( password, bitmode );

            return base.encrypt( bitmode, password, filename, out encodedData );
        }
    }
   
}
