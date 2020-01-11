using EtaModemConfigurator.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EtaModemConfigurator.Modbus
{
    public class ModbusProtocol
    {
        public static byte[] GetReadRequest(ModbusFunctionData functionData, ModbusMode modbusMode)
        {
            var buffer = new List<byte>();
            buffer.AddRange(GetPduHeader(functionData));

            if (modbusMode == ModbusMode.RTU)
            {
                buffer.AddRange(buffer.ToArray().Crc16());
            }
            if (modbusMode == ModbusMode.ASCII)
            {
                buffer = PrepareAsciiPackage(buffer);
            }

            return buffer.ToArray();
        }

        public static byte[] GetWriteRequest(ModbusFunctionData functionData, ModbusMode modbusMode,  bool isNeedToAddDataLength = true, bool isNeedToAddRegistersCount = true)
        {
            var buffer = new List<byte>();
            buffer.AddRange(GetPduHeader(functionData, isNeedToAddRegistersCount));
            // если есть секция данных, то добавляем её
            if (functionData.Data != null && functionData.Data.Length > 0)
            {
                if (isNeedToAddDataLength)
                {
                    buffer.Add(Convert.ToByte(functionData.Data.Length));
                }
                buffer.AddRange(functionData.Data);
            }
            // иначе смотрим, есть ли произвольные данные, неукладывающиеся в идеологию пакета Modbus
            else if (functionData.ArbitraryData != null)
            {
                buffer.AddRange(functionData.ArbitraryData);
            }

            if (modbusMode == ModbusMode.RTU)
            {
                buffer.AddRange(buffer.ToArray().Crc16());
            }
            if (modbusMode == ModbusMode.ASCII)
            {
                buffer = PrepareAsciiPackage(buffer);
            }

            return buffer.ToArray();
        }

        private static List<byte> PrepareAsciiPackage(List<byte> buffer)
        {
            buffer.Add(buffer.ToArray().Lrc());

            // преобразуем пакет в ASCII-формат
            buffer = buffer.ToArray().HexToAscii().ToList();
            
            // добавляем байты конца "возврат каретки" и "перевод строки"
            buffer.AddRange(new byte[] { 0x0D, 0x0A });
            // разворачиваем
            buffer.Reverse();
            // добавляем начальный байт "символ двоеточия"
            buffer.Add(0x3A);
            // обратно разворачиваем
            buffer.Reverse();

            return buffer;
        }


        private static byte[] GetPduHeader(ModbusFunctionData functionData, bool isNeedToAddRegistersCount = true)
        {
            var buffer = new List<byte>
            {
                Convert.ToByte(functionData.DeviceAddress),
                Convert.ToByte(functionData.Function.RequestCode)
            };
            buffer.AddRange(functionData.StartingAddress.ToTwoBigEndianBytes());

            if (isNeedToAddRegistersCount)
            {
                buffer.AddRange(functionData.RegistersCount.ToTwoBigEndianBytes());
            }
            return buffer.ToArray();
        }

        /// <summary>
        /// Определяет корректность контрольной суммы пакета Modbus в режиме ASCII
        /// </summary>
        /// <param name="buffer">Входной ASCII-буфер</param>
        public static bool VerifyLrcCheckSum(byte[] buffer)
        {
            var hexBuffer = ConvertToRtuFormat(buffer, ModbusMode.ASCII);
            
            var packageLrc = hexBuffer.Take(hexBuffer.Length - 1).ToArray().Lrc();

            return packageLrc == hexBuffer[hexBuffer.Length - 1];
        }

        /// <summary>
        /// Преобразует пакет к виду RTU-формата
        /// </summary>
        /// <param name="buffer">Буфер, содержащий пакет</param>
        /// <param name="modbusMode">Режим Modbus</param>
        public static byte[] ConvertToRtuFormat(byte[] buffer, ModbusMode modbusMode)
        {
            if (modbusMode == ModbusMode.ASCII)
            {
                // отсекаем ":", разворачиваем, отсекаем [CR][LF], разворачиваем обратно
                var bufferWithoutStartEnd = buffer.Skip(1).Reverse().Skip(2).Reverse().ToArray();

                return bufferWithoutStartEnd.AsciiToHex();
            }
            return buffer;
        }
    }
}