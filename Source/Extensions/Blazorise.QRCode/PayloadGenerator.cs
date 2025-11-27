#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
#endregion

namespace Blazorise.QRCode;

/// <summary>
/// Generate a QR code payloads based on a diferent usecases.
/// </summary>
/// <remarks>
/// This is a straight copy from QRCoder so that we don't have direct connect to the QRCoder api from Blazorise users.
/// </remarks>
public static class PayloadGenerator
{
    public abstract class Payload
    {
        public virtual int Version => -1;

        public virtual EccLevel EccLevel => EccLevel.M;

        public virtual EciMode EciMode => EciMode.Default;

        public abstract override string ToString();
    }

    public class WiFi : Payload
    {
        public enum Authentication
        {
            WEP,
            WPA,
            nopass
        }

        private readonly string ssid;

        private readonly string password;

        private readonly string authenticationMode;

        private readonly bool isHiddenSsid;

        public WiFi( string ssid, string password, Authentication authenticationMode, bool isHiddenSSID = false, bool escapeHexStrings = true )
        {
            this.ssid = EscapeInput( ssid );
            this.ssid = ( ( escapeHexStrings && isHexStyle( this.ssid ) ) ? ( "\"" + this.ssid + "\"" ) : this.ssid );
            this.password = EscapeInput( password );
            this.password = ( ( escapeHexStrings && isHexStyle( this.password ) ) ? ( "\"" + this.password + "\"" ) : this.password );
            this.authenticationMode = authenticationMode.ToString();
            isHiddenSsid = isHiddenSSID;
        }

        public override string ToString()
        {
            return "WIFI:T:" + authenticationMode + ";S:" + ssid + ";P:" + password + ";" + ( isHiddenSsid ? "H:true" : string.Empty ) + ";";
        }
    }

    public class Mail : Payload
    {
        public enum MailEncoding
        {
            MAILTO,
            MATMSG,
            SMTP
        }

        private readonly string mailReceiver;

        private readonly string subject;

        private readonly string message;

        private readonly MailEncoding encoding;

        public Mail( string mailReceiver = null, string subject = null, string message = null, MailEncoding encoding = MailEncoding.MAILTO )
        {
            this.mailReceiver = mailReceiver;
            this.subject = subject;
            this.message = message;
            this.encoding = encoding;
        }

        public override string ToString()
        {
            string result = string.Empty;
            switch ( encoding )
            {
                case MailEncoding.MAILTO:
                    {
                        List<string> list = new List<string>();
                        if ( !string.IsNullOrEmpty( subject ) )
                        {
                            list.Add( "subject=" + Uri.EscapeDataString( subject ) );
                        }

                        if ( !string.IsNullOrEmpty( message ) )
                        {
                            list.Add( "body=" + Uri.EscapeDataString( message ) );
                        }

                        string text = ( list.Any() ? ( "?" + string.Join( "&", list.ToArray() ) ) : "" );
                        result = "mailto:" + mailReceiver + text;
                        break;
                    }
                case MailEncoding.MATMSG:
                    result = "MATMSG:TO:" + mailReceiver + ";SUB:" + EscapeInput( subject ) + ";BODY:" + EscapeInput( message ) + ";;";
                    break;
                case MailEncoding.SMTP:
                    result = "SMTP:" + mailReceiver + ":" + EscapeInput( subject, simple: true ) + ":" + EscapeInput( message, simple: true );
                    break;
            }

            return result;
        }
    }

    public class SMS : Payload
    {
        public enum SMSEncoding
        {
            SMS,
            SMSTO,
            SMS_iOS
        }

        private readonly string number;

        private readonly string subject;

        private readonly SMSEncoding encoding;

        public SMS( string number, SMSEncoding encoding = SMSEncoding.SMS )
        {
            this.number = number;
            subject = string.Empty;
            this.encoding = encoding;
        }

        public SMS( string number, string subject, SMSEncoding encoding = SMSEncoding.SMS )
        {
            this.number = number;
            this.subject = subject;
            this.encoding = encoding;
        }

        public override string ToString()
        {
            string result = string.Empty;
            switch ( encoding )
            {
                case SMSEncoding.SMS:
                    {
                        string text2 = string.Empty;
                        if ( !string.IsNullOrEmpty( subject ) )
                        {
                            text2 = "?body=" + Uri.EscapeDataString( subject );
                        }

                        result = "sms:" + number + text2;
                        break;
                    }
                case SMSEncoding.SMS_iOS:
                    {
                        string text = string.Empty;
                        if ( !string.IsNullOrEmpty( subject ) )
                        {
                            text = ";body=" + Uri.EscapeDataString( subject );
                        }

                        result = "sms:" + number + text;
                        break;
                    }
                case SMSEncoding.SMSTO:
                    result = "SMSTO:" + number + ":" + subject;
                    break;
            }

            return result;
        }
    }

    public class MMS : Payload
    {
        public enum MMSEncoding
        {
            MMS,
            MMSTO
        }

        private readonly string number;

        private readonly string subject;

        private readonly MMSEncoding encoding;

        public MMS( string number, MMSEncoding encoding = MMSEncoding.MMS )
        {
            this.number = number;
            subject = string.Empty;
            this.encoding = encoding;
        }

        public MMS( string number, string subject, MMSEncoding encoding = MMSEncoding.MMS )
        {
            this.number = number;
            this.subject = subject;
            this.encoding = encoding;
        }

        public override string ToString()
        {
            string result = string.Empty;
            switch ( encoding )
            {
                case MMSEncoding.MMSTO:
                    {
                        string text2 = string.Empty;
                        if ( !string.IsNullOrEmpty( subject ) )
                        {
                            text2 = "?subject=" + Uri.EscapeDataString( subject );
                        }

                        result = "mmsto:" + number + text2;
                        break;
                    }
                case MMSEncoding.MMS:
                    {
                        string text = string.Empty;
                        if ( !string.IsNullOrEmpty( subject ) )
                        {
                            text = "?body=" + Uri.EscapeDataString( subject );
                        }

                        result = "mms:" + number + text;
                        break;
                    }
            }

            return result;
        }
    }

    public class Geolocation : Payload
    {
        public enum GeolocationEncoding
        {
            GEO,
            GoogleMaps
        }

        private readonly string latitude;

        private readonly string longitude;

        private readonly GeolocationEncoding encoding;

        public Geolocation( string latitude, string longitude, GeolocationEncoding encoding = GeolocationEncoding.GEO )
        {
            this.latitude = latitude.Replace( ",", "." );
            this.longitude = longitude.Replace( ",", "." );
            this.encoding = encoding;
        }

        public override string ToString()
        {
            return encoding switch
            {
                GeolocationEncoding.GEO => "geo:" + latitude + "," + longitude,
                GeolocationEncoding.GoogleMaps => "http://maps.google.com/maps?q=" + latitude + "," + longitude,
                _ => "geo:",
            };
        }
    }

    public class PhoneNumber : Payload
    {
        private readonly string number;

        public PhoneNumber( string number )
        {
            this.number = number;
        }

        public override string ToString()
        {
            return "tel:" + number;
        }
    }

    public class SkypeCall : Payload
    {
        private readonly string skypeUsername;

        public SkypeCall( string skypeUsername )
        {
            this.skypeUsername = skypeUsername;
        }

        public override string ToString()
        {
            return "skype:" + skypeUsername + "?call";
        }
    }

    public class Url : Payload
    {
        private readonly string url;

        public Url( string url )
        {
            this.url = url;
        }

        public override string ToString()
        {
            if ( url.StartsWith( "http" ) )
            {
                return url;
            }

            return "http://" + url;
        }
    }

    public class WhatsAppMessage : Payload
    {
        private readonly string number;

        private readonly string message;

        public WhatsAppMessage( string number, string message )
        {
            this.number = number;
            this.message = message;
        }

        public WhatsAppMessage( string message )
        {
            number = string.Empty;
            this.message = message;
        }

        public override string ToString()
        {
            string text = Regex.Replace( number, "^[0+]+|[ ()-]", string.Empty );
            return "https://wa.me/" + text + "?text=" + Uri.EscapeDataString( message );
        }
    }

    public class Bookmark : Payload
    {
        private readonly string url;

        private readonly string title;

        public Bookmark( string url, string title )
        {
            this.url = EscapeInput( url );
            this.title = EscapeInput( title );
        }

        public override string ToString()
        {
            return "MEBKM:TITLE:" + title + ";URL:" + url + ";;";
        }
    }

    public class ContactData : Payload
    {
        public enum ContactOutputType
        {
            MeCard,
            VCard21,
            VCard3,
            VCard4
        }

        public enum AddressOrder
        {
            Default,
            Reversed
        }

        private readonly string firstname;

        private readonly string lastname;

        private readonly string nickname;

        private readonly string org;

        private readonly string orgTitle;

        private readonly string phone;

        private readonly string mobilePhone;

        private readonly string workPhone;

        private readonly string email;

        private readonly DateTime? birthday;

        private readonly string website;

        private readonly string street;

        private readonly string houseNumber;

        private readonly string city;

        private readonly string zipCode;

        private readonly string stateRegion;

        private readonly string country;

        private readonly string note;

        private readonly ContactOutputType outputType;

        private readonly AddressOrder addressOrder;

        public ContactData( ContactOutputType outputType, string firstname, string lastname, string nickname = null, string phone = null, string mobilePhone = null, string workPhone = null, string email = null, DateTime? birthday = null, string website = null, string street = null, string houseNumber = null, string city = null, string zipCode = null, string country = null, string note = null, string stateRegion = null, AddressOrder addressOrder = AddressOrder.Default, string org = null, string orgTitle = null )
        {
            this.firstname = firstname;
            this.lastname = lastname;
            this.nickname = nickname;
            this.org = org;
            this.orgTitle = orgTitle;
            this.phone = phone;
            this.mobilePhone = mobilePhone;
            this.workPhone = workPhone;
            this.email = email;
            this.birthday = birthday;
            this.website = website;
            this.street = street;
            this.houseNumber = houseNumber;
            this.city = city;
            this.stateRegion = stateRegion;
            this.zipCode = zipCode;
            this.country = country;
            this.addressOrder = addressOrder;
            this.note = note;
            this.outputType = outputType;
        }

        public override string ToString()
        {
            string empty = string.Empty;
            if ( outputType == ContactOutputType.MeCard )
            {
                empty += "MECARD+\r\n";
                if ( !string.IsNullOrEmpty( firstname ) && !string.IsNullOrEmpty( lastname ) )
                {
                    empty = empty + "N:" + lastname + ", " + firstname + "\r\n";
                }
                else if ( !string.IsNullOrEmpty( firstname ) || !string.IsNullOrEmpty( lastname ) )
                {
                    empty = empty + "N:" + firstname + lastname + "\r\n";
                }

                if ( !string.IsNullOrEmpty( org ) )
                {
                    empty = empty + "ORG:" + org + "\r\n";
                }

                if ( !string.IsNullOrEmpty( orgTitle ) )
                {
                    empty = empty + "TITLE:" + orgTitle + "\r\n";
                }

                if ( !string.IsNullOrEmpty( phone ) )
                {
                    empty = empty + "TEL:" + phone + "\r\n";
                }

                if ( !string.IsNullOrEmpty( mobilePhone ) )
                {
                    empty = empty + "TEL:" + mobilePhone + "\r\n";
                }

                if ( !string.IsNullOrEmpty( workPhone ) )
                {
                    empty = empty + "TEL:" + workPhone + "\r\n";
                }

                if ( !string.IsNullOrEmpty( email ) )
                {
                    empty = empty + "EMAIL:" + email + "\r\n";
                }

                if ( !string.IsNullOrEmpty( note ) )
                {
                    empty = empty + "NOTE:" + note + "\r\n";
                }

                if ( birthday.HasValue )
                {
                    empty = empty + "BDAY:" + birthday.Value.ToString( "yyyyMMdd" ) + "\r\n";
                }

                string empty2 = string.Empty;
                empty2 = ( ( addressOrder != 0 ) ? ( "ADR:,," + ( ( !string.IsNullOrEmpty( houseNumber ) ) ? ( houseNumber + " " ) : "" ) + ( ( !string.IsNullOrEmpty( street ) ) ? street : "" ) + "," + ( ( !string.IsNullOrEmpty( city ) ) ? city : "" ) + "," + ( ( !string.IsNullOrEmpty( stateRegion ) ) ? stateRegion : "" ) + "," + ( ( !string.IsNullOrEmpty( zipCode ) ) ? zipCode : "" ) + "," + ( ( !string.IsNullOrEmpty( country ) ) ? country : "" ) + "\r\n" ) : ( "ADR:,," + ( ( !string.IsNullOrEmpty( street ) ) ? ( street + " " ) : "" ) + ( ( !string.IsNullOrEmpty( houseNumber ) ) ? houseNumber : "" ) + "," + ( ( !string.IsNullOrEmpty( zipCode ) ) ? zipCode : "" ) + "," + ( ( !string.IsNullOrEmpty( city ) ) ? city : "" ) + "," + ( ( !string.IsNullOrEmpty( stateRegion ) ) ? stateRegion : "" ) + "," + ( ( !string.IsNullOrEmpty( country ) ) ? country : "" ) + "\r\n" ) );
                empty += empty2;
                if ( !string.IsNullOrEmpty( website ) )
                {
                    empty = empty + "URL:" + website + "\r\n";
                }

                if ( !string.IsNullOrEmpty( nickname ) )
                {
                    empty = empty + "NICKNAME:" + nickname + "\r\n";
                }

                return empty.Trim( '\r', '\n' );
            }

            string text = outputType.ToString().Substring( 5 );
            text = ( ( text.Length <= 1 ) ? ( text + ".0" ) : text.Insert( 1, "." ) );
            empty += "BEGIN:VCARD\r\n";
            empty = empty + "VERSION:" + text + "\r\n";
            empty = empty + "N:" + ( ( !string.IsNullOrEmpty( lastname ) ) ? lastname : "" ) + ";" + ( ( !string.IsNullOrEmpty( firstname ) ) ? firstname : "" ) + ";;;\r\n";
            empty = empty + "FN:" + ( ( !string.IsNullOrEmpty( firstname ) ) ? ( firstname + " " ) : "" ) + ( ( !string.IsNullOrEmpty( lastname ) ) ? lastname : "" ) + "\r\n";
            if ( !string.IsNullOrEmpty( org ) )
            {
                empty = empty + "ORG:" + org + "\r\n";
            }

            if ( !string.IsNullOrEmpty( orgTitle ) )
            {
                empty = empty + "TITLE:" + orgTitle + "\r\n";
            }

            if ( !string.IsNullOrEmpty( phone ) )
            {
                empty += "TEL;";
                empty = ( ( outputType == ContactOutputType.VCard21 ) ? ( empty + "HOME;VOICE:" + phone ) : ( ( outputType != ContactOutputType.VCard3 ) ? ( empty + "TYPE=home,voice;VALUE=uri:tel:" + phone ) : ( empty + "TYPE=HOME,VOICE:" + phone ) ) );
                empty += "\r\n";
            }

            if ( !string.IsNullOrEmpty( mobilePhone ) )
            {
                empty += "TEL;";
                empty = ( ( outputType == ContactOutputType.VCard21 ) ? ( empty + "HOME;CELL:" + mobilePhone ) : ( ( outputType != ContactOutputType.VCard3 ) ? ( empty + "TYPE=home,cell;VALUE=uri:tel:" + mobilePhone ) : ( empty + "TYPE=HOME,CELL:" + mobilePhone ) ) );
                empty += "\r\n";
            }

            if ( !string.IsNullOrEmpty( workPhone ) )
            {
                empty += "TEL;";
                empty = ( ( outputType == ContactOutputType.VCard21 ) ? ( empty + "WORK;VOICE:" + workPhone ) : ( ( outputType != ContactOutputType.VCard3 ) ? ( empty + "TYPE=work,voice;VALUE=uri:tel:" + workPhone ) : ( empty + "TYPE=WORK,VOICE:" + workPhone ) ) );
                empty += "\r\n";
            }

            empty += "ADR;";
            empty = ( ( outputType == ContactOutputType.VCard21 ) ? ( empty + "HOME;PREF:" ) : ( ( outputType != ContactOutputType.VCard3 ) ? ( empty + "TYPE=home,pref:" ) : ( empty + "TYPE=HOME,PREF:" ) ) );
            string empty3 = string.Empty;
            empty3 = ( ( addressOrder != 0 ) ? ( ";;" + ( ( !string.IsNullOrEmpty( houseNumber ) ) ? ( houseNumber + " " ) : "" ) + ( ( !string.IsNullOrEmpty( street ) ) ? street : "" ) + ";" + ( ( !string.IsNullOrEmpty( city ) ) ? city : "" ) + ";" + ( ( !string.IsNullOrEmpty( stateRegion ) ) ? stateRegion : "" ) + ";" + ( ( !string.IsNullOrEmpty( zipCode ) ) ? zipCode : "" ) + ";" + ( ( !string.IsNullOrEmpty( country ) ) ? country : "" ) + "\r\n" ) : ( ";;" + ( ( !string.IsNullOrEmpty( street ) ) ? ( street + " " ) : "" ) + ( ( !string.IsNullOrEmpty( houseNumber ) ) ? houseNumber : "" ) + ";" + ( ( !string.IsNullOrEmpty( zipCode ) ) ? zipCode : "" ) + ";" + ( ( !string.IsNullOrEmpty( city ) ) ? city : "" ) + ";" + ( ( !string.IsNullOrEmpty( stateRegion ) ) ? stateRegion : "" ) + ";" + ( ( !string.IsNullOrEmpty( country ) ) ? country : "" ) + "\r\n" ) );
            empty += empty3;
            if ( birthday.HasValue )
            {
                empty = empty + "BDAY:" + birthday.Value.ToString( "yyyyMMdd" ) + "\r\n";
            }

            if ( !string.IsNullOrEmpty( website ) )
            {
                empty = empty + "URL:" + website + "\r\n";
            }

            if ( !string.IsNullOrEmpty( email ) )
            {
                empty = empty + "EMAIL:" + email + "\r\n";
            }

            if ( !string.IsNullOrEmpty( note ) )
            {
                empty = empty + "NOTE:" + note + "\r\n";
            }

            if ( outputType != ContactOutputType.VCard21 && !string.IsNullOrEmpty( nickname ) )
            {
                empty = empty + "NICKNAME:" + nickname + "\r\n";
            }

            return empty + "END:VCARD";
        }
    }

    public class BitcoinLikeCryptoCurrencyAddress : Payload
    {
        public enum BitcoinLikeCryptoCurrencyType
        {
            Bitcoin,
            BitcoinCash,
            Litecoin
        }

        private readonly BitcoinLikeCryptoCurrencyType currencyType;

        private readonly string address;

        private readonly string label;

        private readonly string message;

        private readonly double? amount;

        public BitcoinLikeCryptoCurrencyAddress( BitcoinLikeCryptoCurrencyType currencyType, string address, double? amount, string label = null, string message = null )
        {
            this.currencyType = currencyType;
            this.address = address;
            if ( !string.IsNullOrEmpty( label ) )
            {
                this.label = Uri.EscapeUriString( label );
            }

            if ( !string.IsNullOrEmpty( message ) )
            {
                this.message = Uri.EscapeUriString( message );
            }

            this.amount = amount;
        }

        public override string ToString()
        {
            string text = null;
            KeyValuePair<string, string>[] source = new KeyValuePair<string, string>[3]
            {
                new KeyValuePair<string, string>("label", label),
                new KeyValuePair<string, string>("message", message),
                new KeyValuePair<string, string>("amount", amount.HasValue ? amount.Value.ToString("#.########", CultureInfo.InvariantCulture) : null)
            };
            if ( source.Any( ( KeyValuePair<string, string> keyPair ) => !string.IsNullOrEmpty( keyPair.Value ) ) )
            {
                text = "?" + string.Join( "&", ( from keyPair in source
                                                 where !string.IsNullOrEmpty( keyPair.Value )
                                                 select keyPair.Key + "=" + keyPair.Value ).ToArray() );
            }

            return Enum.GetName( typeof( BitcoinLikeCryptoCurrencyType ), currencyType )!.ToLower() + ":" + address + text;
        }
    }

    public class BitcoinAddress : BitcoinLikeCryptoCurrencyAddress
    {
        public BitcoinAddress( string address, double? amount, string label = null, string message = null )
            : base( BitcoinLikeCryptoCurrencyType.Bitcoin, address, amount, label, message )
        {
        }
    }

    public class BitcoinCashAddress : BitcoinLikeCryptoCurrencyAddress
    {
        public BitcoinCashAddress( string address, double? amount, string label = null, string message = null )
            : base( BitcoinLikeCryptoCurrencyType.BitcoinCash, address, amount, label, message )
        {
        }
    }

    public class LitecoinAddress : BitcoinLikeCryptoCurrencyAddress
    {
        public LitecoinAddress( string address, double? amount, string label = null, string message = null )
            : base( BitcoinLikeCryptoCurrencyType.Litecoin, address, amount, label, message )
        {
        }
    }

    public class SwissQrCode : Payload
    {
        public class AdditionalInformation
        {
            public class SwissQrCodeAdditionalInformationException : Exception
            {
                public SwissQrCodeAdditionalInformationException()
                {
                }

                public SwissQrCodeAdditionalInformationException( string message )
                    : base( message )
                {
                }

                public SwissQrCodeAdditionalInformationException( string message, Exception inner )
                    : base( message, inner )
                {
                }
            }

            private readonly string unstructuredMessage;

            private readonly string billInformation;

            private readonly string trailer;

            public string UnstructureMessage
            {
                get
                {
                    if ( string.IsNullOrEmpty( unstructuredMessage ) )
                    {
                        return null;
                    }

                    return unstructuredMessage.Replace( "\n", "" );
                }
            }

            public string BillInformation
            {
                get
                {
                    if ( string.IsNullOrEmpty( billInformation ) )
                    {
                        return null;
                    }

                    return billInformation.Replace( "\n", "" );
                }
            }

            public string Trailer => trailer;

            public AdditionalInformation( string unstructuredMessage = null, string billInformation = null )
            {
                if ( ( unstructuredMessage?.Length ?? 0 ) + ( billInformation?.Length ?? 0 ) > 140 )
                {
                    throw new SwissQrCodeAdditionalInformationException( "Unstructured message and bill information must be shorter than 141 chars in total/combined." );
                }

                this.unstructuredMessage = unstructuredMessage;
                this.billInformation = billInformation;
                trailer = "EPD";
            }
        }

        public class Reference
        {
            public enum ReferenceType
            {
                QRR,
                SCOR,
                NON
            }

            public enum ReferenceTextType
            {
                QrReference,
                CreditorReferenceIso11649
            }

            public class SwissQrCodeReferenceException : Exception
            {
                public SwissQrCodeReferenceException()
                {
                }

                public SwissQrCodeReferenceException( string message )
                    : base( message )
                {
                }

                public SwissQrCodeReferenceException( string message, Exception inner )
                    : base( message, inner )
                {
                }
            }

            private readonly ReferenceType referenceType;

            private readonly string reference;

            private readonly ReferenceTextType? referenceTextType;

            public ReferenceType RefType => referenceType;

            public string ReferenceText
            {
                get
                {
                    if ( string.IsNullOrEmpty( reference ) )
                    {
                        return null;
                    }

                    return reference.Replace( "\n", "" );
                }
            }

            public Reference( ReferenceType referenceType, string reference = null, ReferenceTextType? referenceTextType = null )
            {
                this.referenceType = referenceType;
                this.referenceTextType = referenceTextType;
                if ( referenceType == ReferenceType.NON && reference != null )
                {
                    throw new SwissQrCodeReferenceException( "Reference is only allowed when referenceType not equals \"NON\"" );
                }

                if ( referenceType != ReferenceType.NON && reference != null && !referenceTextType.HasValue )
                {
                    throw new SwissQrCodeReferenceException( "You have to set an ReferenceTextType when using the reference text." );
                }

                if ( referenceTextType == ReferenceTextType.QrReference && reference != null && reference.Length > 27 )
                {
                    throw new SwissQrCodeReferenceException( "QR-references have to be shorter than 28 chars." );
                }

                if ( referenceTextType == ReferenceTextType.QrReference && reference != null && !Regex.IsMatch( reference, "^[0-9]+$" ) )
                {
                    throw new SwissQrCodeReferenceException( "QR-reference must exist out of digits only." );
                }

                if ( referenceTextType == ReferenceTextType.QrReference && reference != null && !ChecksumMod10( reference ) )
                {
                    throw new SwissQrCodeReferenceException( "QR-references is invalid. Checksum error." );
                }

                if ( referenceTextType == ReferenceTextType.CreditorReferenceIso11649 && reference != null && reference.Length > 25 )
                {
                    throw new SwissQrCodeReferenceException( "Creditor references (ISO 11649) have to be shorter than 26 chars." );
                }

                this.reference = reference;
            }
        }

        public class Iban
        {
            public enum IbanType
            {
                Iban,
                QrIban
            }

            public class SwissQrCodeIbanException : Exception
            {
                public SwissQrCodeIbanException()
                {
                }

                public SwissQrCodeIbanException( string message )
                    : base( message )
                {
                }

                public SwissQrCodeIbanException( string message, Exception inner )
                    : base( message, inner )
                {
                }
            }

            private string iban;

            private IbanType ibanType;

            public bool IsQrIban => ibanType == IbanType.QrIban;

            public Iban( string iban, IbanType ibanType )
            {
                if ( ibanType == IbanType.Iban && !IsValidIban( iban ) )
                {
                    throw new SwissQrCodeIbanException( "The IBAN entered isn't valid." );
                }

                if ( ibanType == IbanType.QrIban && !IsValidQRIban( iban ) )
                {
                    throw new SwissQrCodeIbanException( "The QR-IBAN entered isn't valid." );
                }

                if ( !iban.StartsWith( "CH" ) && !iban.StartsWith( "LI" ) )
                {
                    throw new SwissQrCodeIbanException( "The IBAN must start with \"CH\" or \"LI\"." );
                }

                this.iban = iban;
                this.ibanType = ibanType;
            }

            public override string ToString()
            {
                return iban.Replace( "-", "" ).Replace( "\n", "" ).Replace( " ", "" );
            }
        }

        public class Contact
        {
            public enum AddressType
            {
                StructuredAddress,
                CombinedAddress
            }

            public class SwissQrCodeContactException : Exception
            {
                public SwissQrCodeContactException()
                {
                }

                public SwissQrCodeContactException( string message )
                    : base( message )
                {
                }

                public SwissQrCodeContactException( string message, Exception inner )
                    : base( message, inner )
                {
                }
            }

            private static readonly HashSet<string> twoLetterCodes = ValidTwoLetterCodes();

            private string br = "\r\n";

            private string name;

            private string streetOrAddressline1;

            private string houseNumberOrAddressline2;

            private string zipCode;

            private string city;

            private string country;

            private AddressType adrType;

            [Obsolete( "This constructor is deprecated. Use WithStructuredAddress instead." )]
            public Contact( string name, string zipCode, string city, string country, string street = null, string houseNumber = null )
                : this( name, zipCode, city, country, street, houseNumber, AddressType.StructuredAddress )
            {
            }

            [Obsolete( "This constructor is deprecated. Use WithCombinedAddress instead." )]
            public Contact( string name, string country, string addressLine1, string addressLine2 )
                : this( name, null, null, country, addressLine1, addressLine2, AddressType.CombinedAddress )
            {
            }

            public static Contact WithStructuredAddress( string name, string zipCode, string city, string country, string street = null, string houseNumber = null )
            {
                return new Contact( name, zipCode, city, country, street, houseNumber, AddressType.StructuredAddress );
            }

            public static Contact WithCombinedAddress( string name, string country, string addressLine1, string addressLine2 )
            {
                return new Contact( name, null, null, country, addressLine1, addressLine2, AddressType.CombinedAddress );
            }

            private Contact( string name, string zipCode, string city, string country, string streetOrAddressline1, string houseNumberOrAddressline2, AddressType addressType )
            {
                string text = "^([a-zA-Z0-9\\.,;:'\\ \\+\\-/\\(\\)?\\*\\[\\]\\{\\}\\\\`\u00b4~ ]|[!\"#%&<>÷=@_$£]|[àáâäçèéêëìíîïñòóôöùúûüýßÀÁÂÄÇÈÉÊËÌÍÎÏÒÓÔÖÙÚÛÜÑ])*$";
                adrType = addressType;
                if ( string.IsNullOrEmpty( name ) )
                {
                    throw new SwissQrCodeContactException( "Name must not be empty." );
                }

                if ( name.Length > 70 )
                {
                    throw new SwissQrCodeContactException( "Name must be shorter than 71 chars." );
                }

                if ( !Regex.IsMatch( name, text ) )
                {
                    throw new SwissQrCodeContactException( "Name must match the following pattern as defined in pain.001: " + text );
                }

                this.name = name;
                if ( adrType == AddressType.StructuredAddress )
                {
                    if ( !string.IsNullOrEmpty( streetOrAddressline1 ) && streetOrAddressline1.Length > 70 )
                    {
                        throw new SwissQrCodeContactException( "Street must be shorter than 71 chars." );
                    }

                    if ( !string.IsNullOrEmpty( streetOrAddressline1 ) && !Regex.IsMatch( streetOrAddressline1, text ) )
                    {
                        throw new SwissQrCodeContactException( "Street must match the following pattern as defined in pain.001: " + text );
                    }

                    this.streetOrAddressline1 = streetOrAddressline1;
                    if ( !string.IsNullOrEmpty( houseNumberOrAddressline2 ) && houseNumberOrAddressline2.Length > 16 )
                    {
                        throw new SwissQrCodeContactException( "House number must be shorter than 17 chars." );
                    }

                    this.houseNumberOrAddressline2 = houseNumberOrAddressline2;
                }
                else
                {
                    if ( !string.IsNullOrEmpty( streetOrAddressline1 ) && streetOrAddressline1.Length > 70 )
                    {
                        throw new SwissQrCodeContactException( "Address line 1 must be shorter than 71 chars." );
                    }

                    if ( !string.IsNullOrEmpty( streetOrAddressline1 ) && !Regex.IsMatch( streetOrAddressline1, text ) )
                    {
                        throw new SwissQrCodeContactException( "Address line 1 must match the following pattern as defined in pain.001: " + text );
                    }

                    this.streetOrAddressline1 = streetOrAddressline1;
                    if ( string.IsNullOrEmpty( houseNumberOrAddressline2 ) )
                    {
                        throw new SwissQrCodeContactException( "Address line 2 must be provided for combined addresses (address line-based addresses)." );
                    }

                    if ( !string.IsNullOrEmpty( houseNumberOrAddressline2 ) && houseNumberOrAddressline2.Length > 70 )
                    {
                        throw new SwissQrCodeContactException( "Address line 2 must be shorter than 71 chars." );
                    }

                    if ( !string.IsNullOrEmpty( houseNumberOrAddressline2 ) && !Regex.IsMatch( houseNumberOrAddressline2, text ) )
                    {
                        throw new SwissQrCodeContactException( "Address line 2 must match the following pattern as defined in pain.001: " + text );
                    }

                    this.houseNumberOrAddressline2 = houseNumberOrAddressline2;
                }

                if ( adrType == AddressType.StructuredAddress )
                {
                    if ( string.IsNullOrEmpty( zipCode ) )
                    {
                        throw new SwissQrCodeContactException( "Zip code must not be empty." );
                    }

                    if ( zipCode.Length > 16 )
                    {
                        throw new SwissQrCodeContactException( "Zip code must be shorter than 17 chars." );
                    }

                    if ( !Regex.IsMatch( zipCode, text ) )
                    {
                        throw new SwissQrCodeContactException( "Zip code must match the following pattern as defined in pain.001: " + text );
                    }

                    this.zipCode = zipCode;
                    if ( string.IsNullOrEmpty( city ) )
                    {
                        throw new SwissQrCodeContactException( "City must not be empty." );
                    }

                    if ( city.Length > 35 )
                    {
                        throw new SwissQrCodeContactException( "City name must be shorter than 36 chars." );
                    }

                    if ( !Regex.IsMatch( city, text ) )
                    {
                        throw new SwissQrCodeContactException( "City name must match the following pattern as defined in pain.001: " + text );
                    }

                    this.city = city;
                }
                else
                {
                    this.zipCode = ( this.city = string.Empty );
                }

                if ( !IsValidTwoLetterCode( country ) )
                {
                    throw new SwissQrCodeContactException( "Country must be a valid \"two letter\" country code as defined by  ISO 3166-1, but it isn't." );
                }

                this.country = country;
            }

            private static bool IsValidTwoLetterCode( string code )
            {
                return twoLetterCodes.Contains( code );
            }

            private static HashSet<string> ValidTwoLetterCodes()
            {
                return new HashSet<string>( new string[249]
                {
                    "AF", "AL", "DZ", "AS", "AD", "AO", "AI", "AQ", "AG", "AR",
                    "AM", "AW", "AU", "AT", "AZ", "BS", "BH", "BD", "BB", "BY",
                    "BE", "BZ", "BJ", "BM", "BT", "BO", "BQ", "BA", "BW", "BV",
                    "BR", "IO", "BN", "BG", "BF", "BI", "CV", "KH", "CM", "CA",
                    "KY", "CF", "TD", "CL", "CN", "CX", "CC", "CO", "KM", "CG",
                    "CD", "CK", "CR", "CI", "HR", "CU", "CW", "CY", "CZ", "DK",
                    "DJ", "DM", "DO", "EC", "EG", "SV", "GQ", "ER", "EE", "SZ",
                    "ET", "FK", "FO", "FJ", "FI", "FR", "GF", "PF", "TF", "GA",
                    "GM", "GE", "DE", "GH", "GI", "GR", "GL", "GD", "GP", "GU",
                    "GT", "GG", "GN", "GW", "GY", "HT", "HM", "VA", "HN", "HK",
                    "HU", "IS", "IN", "ID", "IR", "IQ", "IE", "IM", "IL", "IT",
                    "JM", "JP", "JE", "JO", "KZ", "KE", "KI", "KP", "KR", "KW",
                    "KG", "LA", "LV", "LB", "LS", "LR", "LY", "LI", "LT", "LU",
                    "MO", "MG", "MW", "MY", "MV", "ML", "MT", "MH", "MQ", "MR",
                    "MU", "YT", "MX", "FM", "MD", "MC", "MN", "ME", "MS", "MA",
                    "MZ", "MM", "NA", "NR", "NP", "NL", "NC", "NZ", "NI", "NE",
                    "NG", "NU", "NF", "MP", "MK", "NO", "OM", "PK", "PW", "PS",
                    "PA", "PG", "PY", "PE", "PH", "PN", "PL", "PT", "PR", "QA",
                    "RE", "RO", "RU", "RW", "BL", "SH", "KN", "LC", "MF", "PM",
                    "VC", "WS", "SM", "ST", "SA", "SN", "RS", "SC", "SL", "SG",
                    "SX", "SK", "SI", "SB", "SO", "ZA", "GS", "SS", "ES", "LK",
                    "SD", "SR", "SJ", "SE", "CH", "SY", "TW", "TJ", "TZ", "TH",
                    "TL", "TG", "TK", "TO", "TT", "TN", "TR", "TM", "TC", "TV",
                    "UG", "UA", "AE", "GB", "US", "UM", "UY", "UZ", "VU", "VE",
                    "VN", "VG", "VI", "WF", "EH", "YE", "ZM", "ZW", "AX"
                }, StringComparer.OrdinalIgnoreCase );
            }

            public override string ToString()
            {
                return string.Concat( string.Concat( string.Concat( string.Concat( string.Concat( string.Concat( ( ( adrType == AddressType.StructuredAddress ) ? "S" : "K" ) + br, name.Replace( "\n", "" ), br ), ( !string.IsNullOrEmpty( streetOrAddressline1 ) ) ? streetOrAddressline1.Replace( "\n", "" ) : string.Empty, br ), ( !string.IsNullOrEmpty( houseNumberOrAddressline2 ) ) ? houseNumberOrAddressline2.Replace( "\n", "" ) : string.Empty, br ), zipCode.Replace( "\n", "" ), br ), city.Replace( "\n", "" ), br ), country, br );
            }
        }

        public enum Currency
        {
            CHF = 756,
            EUR = 978
        }

        public class SwissQrCodeException : Exception
        {
            public SwissQrCodeException()
            {
            }

            public SwissQrCodeException( string message )
                : base( message )
            {
            }

            public SwissQrCodeException( string message, Exception inner )
                : base( message, inner )
            {
            }
        }

        private readonly string br = "\r\n";

        private readonly string alternativeProcedure1;

        private readonly string alternativeProcedure2;

        private readonly Iban iban;

        private readonly decimal? amount;

        private readonly Contact creditor;

        private readonly Contact ultimateCreditor;

        private readonly Contact debitor;

        private readonly Currency currency;

        private readonly DateTime? requestedDateOfPayment;

        private readonly Reference reference;

        private readonly AdditionalInformation additionalInformation;

        public SwissQrCode( Iban iban, Currency currency, Contact creditor, Reference reference, AdditionalInformation additionalInformation = null, Contact debitor = null, decimal? amount = null, DateTime? requestedDateOfPayment = null, Contact ultimateCreditor = null, string alternativeProcedure1 = null, string alternativeProcedure2 = null )
        {
            this.iban = iban;
            this.creditor = creditor;
            this.ultimateCreditor = ultimateCreditor;
            this.additionalInformation = ( ( additionalInformation != null ) ? additionalInformation : new AdditionalInformation() );
            if ( amount.HasValue && amount.ToString()!.Length > 12 )
            {
                throw new SwissQrCodeException( "Amount (including decimals) must be shorter than 13 places." );
            }

            this.amount = amount;
            this.currency = currency;
            this.requestedDateOfPayment = requestedDateOfPayment;
            this.debitor = debitor;
            if ( iban.IsQrIban && reference.RefType != 0 )
            {
                throw new SwissQrCodeException( "If QR-IBAN is used, you have to choose \"QRR\" as reference type!" );
            }

            if ( !iban.IsQrIban && reference.RefType == Reference.ReferenceType.QRR )
            {
                throw new SwissQrCodeException( "If non QR-IBAN is used, you have to choose either \"SCOR\" or \"NON\" as reference type!" );
            }

            this.reference = reference;
            if ( alternativeProcedure1 != null && alternativeProcedure1.Length > 100 )
            {
                throw new SwissQrCodeException( "Alternative procedure information block 1 must be shorter than 101 chars." );
            }

            this.alternativeProcedure1 = alternativeProcedure1;
            if ( alternativeProcedure2 != null && alternativeProcedure2.Length > 100 )
            {
                throw new SwissQrCodeException( "Alternative procedure information block 2 must be shorter than 101 chars." );
            }

            this.alternativeProcedure2 = alternativeProcedure2;
        }

        public override string ToString()
        {
            string text = "SPC" + br;
            text = text + "0200" + br;
            text = text + "1" + br;
            text = text + iban.ToString() + br;
            text += creditor.ToString();
            text = string.Concat( text, string.Concat( Enumerable.Repeat( br, 7 ).ToArray() ) );
            text = text + ( amount.HasValue ? $"{amount:0.00}".Replace( ",", "." ) : string.Empty ) + br;
            text = text + currency.ToString() + br;
            text = ( ( debitor == null ) ? string.Concat( text, string.Concat( Enumerable.Repeat( br, 7 ).ToArray() ) ) : ( text + debitor.ToString() ) );
            text = text + reference.RefType.ToString() + br;
            text = text + ( ( !string.IsNullOrEmpty( reference.ReferenceText ) ) ? reference.ReferenceText : string.Empty ) + br;
            text = text + ( ( !string.IsNullOrEmpty( additionalInformation.UnstructureMessage ) ) ? additionalInformation.UnstructureMessage : string.Empty ) + br;
            text = text + additionalInformation.Trailer + br;
            text = text + ( ( !string.IsNullOrEmpty( additionalInformation.BillInformation ) ) ? additionalInformation.BillInformation : string.Empty ) + br;
            if ( !string.IsNullOrEmpty( alternativeProcedure1 ) )
            {
                text = text + alternativeProcedure1.Replace( "\n", "" ) + br;
            }

            if ( !string.IsNullOrEmpty( alternativeProcedure2 ) )
            {
                text = text + alternativeProcedure2.Replace( "\n", "" ) + br;
            }

            if ( text.EndsWith( br ) )
            {
                text = text.Remove( text.Length - br.Length );
            }

            return text;
        }
    }

    public class Girocode : Payload
    {
        public enum GirocodeVersion
        {
            Version1,
            Version2
        }

        public enum TypeOfRemittance
        {
            Structured,
            Unstructured
        }

        public enum GirocodeEncoding
        {
            UTF_8,
            ISO_8859_1,
            ISO_8859_2,
            ISO_8859_4,
            ISO_8859_5,
            ISO_8859_7,
            ISO_8859_10,
            ISO_8859_15
        }

        public class GirocodeException : Exception
        {
            public GirocodeException()
            {
            }

            public GirocodeException( string message )
                : base( message )
            {
            }

            public GirocodeException( string message, Exception inner )
                : base( message, inner )
            {
            }
        }

        private string br = "\n";

        private readonly string iban;

        private readonly string bic;

        private readonly string name;

        private readonly string purposeOfCreditTransfer;

        private readonly string remittanceInformation;

        private readonly string messageToGirocodeUser;

        private readonly decimal amount;

        private readonly GirocodeVersion version;

        private readonly GirocodeEncoding encoding;

        private readonly TypeOfRemittance typeOfRemittance;

        public Girocode( string iban, string bic, string name, decimal amount, string remittanceInformation = "", TypeOfRemittance typeOfRemittance = TypeOfRemittance.Unstructured, string purposeOfCreditTransfer = "", string messageToGirocodeUser = "", GirocodeVersion version = GirocodeVersion.Version1, GirocodeEncoding encoding = GirocodeEncoding.ISO_8859_1 )
        {
            this.version = version;
            this.encoding = encoding;
            if ( !IsValidIban( iban ) )
            {
                throw new GirocodeException( "The IBAN entered isn't valid." );
            }

            this.iban = iban.Replace( " ", "" ).ToUpper();
            if ( !IsValidBic( bic ) )
            {
                throw new GirocodeException( "The BIC entered isn't valid." );
            }

            this.bic = bic.Replace( " ", "" ).ToUpper();
            if ( name.Length > 70 )
            {
                throw new GirocodeException( "(Payee-)Name must be shorter than 71 chars." );
            }

            this.name = name;
            if ( amount.ToString().Replace( ",", "." ).Contains( "." ) && amount.ToString().Replace( ",", "." ).Split( '.' )[1].TrimEnd( '0' ).Length > 2 )
            {
                throw new GirocodeException( "Amount must have less than 3 digits after decimal point." );
            }

            if ( amount < 0.01m || amount > 999999999.99m )
            {
                throw new GirocodeException( "Amount has to at least 0.01 and must be smaller or equal to 999999999.99." );
            }

            this.amount = amount;
            if ( purposeOfCreditTransfer.Length > 4 )
            {
                throw new GirocodeException( "Purpose of credit transfer can only have 4 chars at maximum." );
            }

            this.purposeOfCreditTransfer = purposeOfCreditTransfer;
            if ( typeOfRemittance == TypeOfRemittance.Unstructured && remittanceInformation.Length > 140 )
            {
                throw new GirocodeException( "Unstructured reference texts have to shorter than 141 chars." );
            }

            if ( typeOfRemittance == TypeOfRemittance.Structured && remittanceInformation.Length > 35 )
            {
                throw new GirocodeException( "Structured reference texts have to shorter than 36 chars." );
            }

            this.typeOfRemittance = typeOfRemittance;
            this.remittanceInformation = remittanceInformation;
            if ( messageToGirocodeUser.Length > 70 )
            {
                throw new GirocodeException( "Message to the Girocode-User reader texts have to shorter than 71 chars." );
            }

            this.messageToGirocodeUser = messageToGirocodeUser;
        }

        public override string ToString()
        {
            return ConvertStringToEncoding( string.Concat( string.Concat( string.Concat( string.Concat( string.Concat( string.Concat( string.Concat( string.Concat( string.Concat( string.Concat( string.Concat( "BCD" + br, ( version == GirocodeVersion.Version1 ) ? "001" : "002", br ), ( (int)( encoding + 1 ) ).ToString(), br ), "SCT", br ), bic, br ), name, br ), iban, br ), $"EUR{amount:0.00}".Replace( ",", "." ), br ), purposeOfCreditTransfer, br ), ( typeOfRemittance == TypeOfRemittance.Structured ) ? remittanceInformation : string.Empty, br ), ( typeOfRemittance == TypeOfRemittance.Unstructured ) ? remittanceInformation : string.Empty, br ), messageToGirocodeUser ), encoding.ToString().Replace( "_", "-" ) );
        }
    }

    public class BezahlCode : Payload
    {
        public enum Currency
        {
            AED = 784,
            AFN = 971,
            ALL = 8,
            AMD = 51,
            ANG = 532,
            AOA = 973,
            ARS = 0x20,
            AUD = 36,
            AWG = 533,
            AZN = 944,
            BAM = 977,
            BBD = 52,
            BDT = 50,
            BGN = 975,
            BHD = 48,
            BIF = 108,
            BMD = 60,
            BND = 96,
            BOB = 68,
            BOV = 984,
            BRL = 986,
            BSD = 44,
            BTN = 0x40,
            BWP = 72,
            BYR = 974,
            BZD = 84,
            CAD = 124,
            CDF = 976,
            CHE = 947,
            CHF = 756,
            CHW = 948,
            CLF = 990,
            CLP = 152,
            CNY = 156,
            COP = 170,
            COU = 970,
            CRC = 188,
            CUC = 931,
            CUP = 192,
            CVE = 132,
            CZK = 203,
            DJF = 262,
            DKK = 208,
            DOP = 214,
            DZD = 12,
            EGP = 818,
            ERN = 232,
            ETB = 230,
            EUR = 978,
            FJD = 242,
            FKP = 238,
            GBP = 826,
            GEL = 981,
            GHS = 936,
            GIP = 292,
            GMD = 270,
            GNF = 324,
            GTQ = 320,
            GYD = 328,
            HKD = 344,
            HNL = 340,
            HRK = 191,
            HTG = 332,
            HUF = 348,
            IDR = 360,
            ILS = 376,
            INR = 356,
            IQD = 368,
            IRR = 364,
            ISK = 352,
            JMD = 388,
            JOD = 400,
            JPY = 392,
            KES = 404,
            KGS = 417,
            KHR = 116,
            KMF = 174,
            KPW = 408,
            KRW = 410,
            KWD = 414,
            KYD = 136,
            KZT = 398,
            LAK = 418,
            LBP = 422,
            LKR = 144,
            LRD = 430,
            LSL = 426,
            LYD = 434,
            MAD = 504,
            MDL = 498,
            MGA = 969,
            MKD = 807,
            MMK = 104,
            MNT = 496,
            MOP = 446,
            MRO = 478,
            MUR = 480,
            MVR = 462,
            MWK = 454,
            MXN = 484,
            MXV = 979,
            MYR = 458,
            MZN = 943,
            NAD = 516,
            NGN = 566,
            NIO = 558,
            NOK = 578,
            NPR = 524,
            NZD = 554,
            OMR = 0x200,
            PAB = 590,
            PEN = 604,
            PGK = 598,
            PHP = 608,
            PKR = 586,
            PLN = 985,
            PYG = 600,
            QAR = 634,
            RON = 946,
            RSD = 941,
            RUB = 643,
            RWF = 646,
            SAR = 682,
            SBD = 90,
            SCR = 690,
            SDG = 938,
            SEK = 752,
            SGD = 702,
            SHP = 654,
            SLL = 694,
            SOS = 706,
            SRD = 968,
            SSP = 728,
            STD = 678,
            SVC = 222,
            SYP = 760,
            SZL = 748,
            THB = 764,
            TJS = 972,
            TMT = 934,
            TND = 788,
            TOP = 776,
            TRY = 949,
            TTD = 780,
            TWD = 901,
            TZS = 834,
            UAH = 980,
            UGX = 800,
            USD = 840,
            USN = 997,
            UYI = 940,
            UYU = 858,
            UZS = 860,
            VEF = 937,
            VND = 704,
            VUV = 548,
            WST = 882,
            XAF = 950,
            XAG = 961,
            XAU = 959,
            XBA = 955,
            XBB = 956,
            XBC = 957,
            XBD = 958,
            XCD = 951,
            XDR = 960,
            XOF = 952,
            XPD = 964,
            XPF = 953,
            XPT = 962,
            XSU = 994,
            XTS = 963,
            XUA = 965,
            XXX = 999,
            YER = 886,
            ZAR = 710,
            ZMW = 967,
            ZWL = 932
        }

        public enum AuthorityType
        {
            [Obsolete]
            singlepayment,
            singlepaymentsepa,
            [Obsolete]
            singledirectdebit,
            singledirectdebitsepa,
            [Obsolete]
            periodicsinglepayment,
            periodicsinglepaymentsepa,
            contact,
            contact_v2
        }

        public class BezahlCodeException : Exception
        {
            public BezahlCodeException()
            {
            }

            public BezahlCodeException( string message )
                : base( message )
            {
            }

            public BezahlCodeException( string message, Exception inner )
                : base( message, inner )
            {
            }
        }

        private readonly string name;

        private readonly string iban;

        private readonly string bic;

        private readonly string account;

        private readonly string bnc;

        private readonly string sepaReference;

        private readonly string reason;

        private readonly string creditorId;

        private readonly string mandateId;

        private readonly string periodicTimeunit;

        private readonly decimal amount;

        private readonly int postingKey;

        private readonly int periodicTimeunitRotation;

        private readonly Currency currency;

        private readonly AuthorityType authority;

        private readonly DateTime executionDate;

        private readonly DateTime dateOfSignature;

        private readonly DateTime periodicFirstExecutionDate;

        private readonly DateTime periodicLastExecutionDate;

        public BezahlCode( AuthorityType authority, string name, string account = "", string bnc = "", string iban = "", string bic = "", string reason = "" )
            : this( authority, name, account, bnc, iban, bic, 0m, string.Empty, 0, null, null, string.Empty, string.Empty, null, reason, 0, string.Empty, Currency.EUR, null, 1 )
        {
        }

        public BezahlCode( AuthorityType authority, string name, string account, string bnc, decimal amount, string periodicTimeunit = "", int periodicTimeunitRotation = 0, DateTime? periodicFirstExecutionDate = null, DateTime? periodicLastExecutionDate = null, string reason = "", int postingKey = 0, Currency currency = Currency.EUR, DateTime? executionDate = null )
            : this( authority, name, account, bnc, string.Empty, string.Empty, amount, periodicTimeunit, periodicTimeunitRotation, periodicFirstExecutionDate, periodicLastExecutionDate, string.Empty, string.Empty, null, reason, postingKey, string.Empty, currency, executionDate, 2 )
        {
        }

        public BezahlCode( AuthorityType authority, string name, string iban, string bic, decimal amount, string periodicTimeunit = "", int periodicTimeunitRotation = 0, DateTime? periodicFirstExecutionDate = null, DateTime? periodicLastExecutionDate = null, string creditorId = "", string mandateId = "", DateTime? dateOfSignature = null, string reason = "", string sepaReference = "", Currency currency = Currency.EUR, DateTime? executionDate = null )
            : this( authority, name, string.Empty, string.Empty, iban, bic, amount, periodicTimeunit, periodicTimeunitRotation, periodicFirstExecutionDate, periodicLastExecutionDate, creditorId, mandateId, dateOfSignature, reason, 0, sepaReference, currency, executionDate, 3 )
        {
        }

        public BezahlCode( AuthorityType authority, string name, string account, string bnc, string iban, string bic, decimal amount, string periodicTimeunit = "", int periodicTimeunitRotation = 0, DateTime? periodicFirstExecutionDate = null, DateTime? periodicLastExecutionDate = null, string creditorId = "", string mandateId = "", DateTime? dateOfSignature = null, string reason = "", int postingKey = 0, string sepaReference = "", Currency currency = Currency.EUR, DateTime? executionDate = null, int internalMode = 0 )
        {
            switch ( internalMode )
            {
                case 1:
                    if ( authority != AuthorityType.contact && authority != AuthorityType.contact_v2 )
                    {
                        throw new BezahlCodeException( "The constructor without an amount may only ne used with authority types 'contact' and 'contact_v2'." );
                    }

                    if ( authority == AuthorityType.contact && ( string.IsNullOrEmpty( account ) || string.IsNullOrEmpty( bnc ) ) )
                    {
                        throw new BezahlCodeException( "When using authority type 'contact' the parameters 'account' and 'bnc' must be set." );
                    }

                    if ( authority != AuthorityType.contact_v2 )
                    {
                        bool flag = !string.IsNullOrEmpty( account ) && !string.IsNullOrEmpty( bnc );
                        bool flag2 = !string.IsNullOrEmpty( iban ) && !string.IsNullOrEmpty( bic );
                        if ( ( !flag && !flag2 ) || ( flag && flag2 ) )
                        {
                            throw new BezahlCodeException( "When using authority type 'contact_v2' either the parameters 'account' and 'bnc' or the parameters 'iban' and 'bic' must be set. Leave the other parameter pair empty." );
                        }
                    }

                    break;
                case 2:
                    if ( authority != AuthorityType.periodicsinglepayment && authority != AuthorityType.singledirectdebit && authority != 0 )
                    {
                        throw new BezahlCodeException( "The constructor with 'account' and 'bnc' may only be used with 'non SEPA' authority types. Either choose another authority type or switch constructor." );
                    }

                    if ( authority == AuthorityType.periodicsinglepayment && ( string.IsNullOrEmpty( periodicTimeunit ) || periodicTimeunitRotation == 0 ) )
                    {
                        throw new BezahlCodeException( "When using 'periodicsinglepayment' as authority type, the parameters 'periodicTimeunit' and 'periodicTimeunitRotation' must be set." );
                    }

                    break;
                case 3:
                    if ( authority != AuthorityType.periodicsinglepaymentsepa && authority != AuthorityType.singledirectdebitsepa && authority != AuthorityType.singlepaymentsepa )
                    {
                        throw new BezahlCodeException( "The constructor with 'iban' and 'bic' may only be used with 'SEPA' authority types. Either choose another authority type or switch constructor." );
                    }

                    if ( authority == AuthorityType.periodicsinglepaymentsepa && ( string.IsNullOrEmpty( periodicTimeunit ) || periodicTimeunitRotation == 0 ) )
                    {
                        throw new BezahlCodeException( "When using 'periodicsinglepaymentsepa' as authority type, the parameters 'periodicTimeunit' and 'periodicTimeunitRotation' must be set." );
                    }

                    break;
            }

            this.authority = authority;
            if ( name.Length > 70 )
            {
                throw new BezahlCodeException( "(Payee-)Name must be shorter than 71 chars." );
            }

            this.name = name;
            if ( reason.Length > 27 )
            {
                throw new BezahlCodeException( "Reasons texts have to be shorter than 28 chars." );
            }

            this.reason = reason;
            bool flag3 = !string.IsNullOrEmpty( account ) && !string.IsNullOrEmpty( bnc );
            bool flag4 = !string.IsNullOrEmpty( iban ) && !string.IsNullOrEmpty( bic );
            if ( authority == AuthorityType.periodicsinglepayment || authority == AuthorityType.singledirectdebit || authority == AuthorityType.singlepayment || authority == AuthorityType.contact || ( authority == AuthorityType.contact_v2 && flag3 ) )
            {
                if ( !Regex.IsMatch( account.Replace( " ", "" ), "^[0-9]{1,9}$" ) )
                {
                    throw new BezahlCodeException( "The account entered isn't valid." );
                }

                this.account = account.Replace( " ", "" ).ToUpper();
                if ( !Regex.IsMatch( bnc.Replace( " ", "" ), "^[0-9]{1,9}$" ) )
                {
                    throw new BezahlCodeException( "The bnc entered isn't valid." );
                }

                this.bnc = bnc.Replace( " ", "" ).ToUpper();
                if ( authority != AuthorityType.contact && authority != AuthorityType.contact_v2 )
                {
                    if ( postingKey < 0 || postingKey >= 100 )
                    {
                        throw new BezahlCodeException( "PostingKey must be within 0 and 99." );
                    }

                    this.postingKey = postingKey;
                }
            }

            if ( authority == AuthorityType.periodicsinglepaymentsepa || authority == AuthorityType.singledirectdebitsepa || authority == AuthorityType.singlepaymentsepa || ( authority == AuthorityType.contact_v2 && flag4 ) )
            {
                if ( !IsValidIban( iban ) )
                {
                    throw new BezahlCodeException( "The IBAN entered isn't valid." );
                }

                this.iban = iban.Replace( " ", "" ).ToUpper();
                if ( !IsValidBic( bic ) )
                {
                    throw new BezahlCodeException( "The BIC entered isn't valid." );
                }

                this.bic = bic.Replace( " ", "" ).ToUpper();
                if ( authority != AuthorityType.contact_v2 )
                {
                    if ( sepaReference.Length > 35 )
                    {
                        throw new BezahlCodeException( "SEPA reference texts have to be shorter than 36 chars." );
                    }

                    this.sepaReference = sepaReference;
                    if ( !string.IsNullOrEmpty( creditorId ) && !Regex.IsMatch( creditorId.Replace( " ", "" ), "^[a-zA-Z]{2,2}[0-9]{2,2}([A-Za-z0-9]|[\\+|\\?|/|\\-|:|\\(|\\)|\\.|,|']){3,3}([A-Za-z0-9]|[\\+|\\?|/|\\-|:|\\(|\\)|\\.|,|']){1,28}$" ) )
                    {
                        throw new BezahlCodeException( "The creditorId entered isn't valid." );
                    }

                    this.creditorId = creditorId;
                    if ( !string.IsNullOrEmpty( mandateId ) && !Regex.IsMatch( mandateId.Replace( " ", "" ), "^([A-Za-z0-9]|[\\+|\\?|/|\\-|:|\\(|\\)|\\.|,|']){1,35}$" ) )
                    {
                        throw new BezahlCodeException( "The mandateId entered isn't valid." );
                    }

                    this.mandateId = mandateId;
                    if ( dateOfSignature.HasValue )
                    {
                        this.dateOfSignature = dateOfSignature.Value;
                    }
                }
            }

            if ( authority == AuthorityType.contact || authority == AuthorityType.contact_v2 )
            {
                return;
            }

            if ( amount.ToString().Replace( ",", "." ).Contains( "." ) && amount.ToString().Replace( ",", "." ).Split( '.' )[1].TrimEnd( '0' ).Length > 2 )
            {
                throw new BezahlCodeException( "Amount must have less than 3 digits after decimal point." );
            }

            if ( amount < 0.01m || amount > 999999999.99m )
            {
                throw new BezahlCodeException( "Amount has to at least 0.01 and must be smaller or equal to 999999999.99." );
            }

            this.amount = amount;
            this.currency = currency;
            if ( !executionDate.HasValue )
            {
                this.executionDate = DateTime.Now;
            }
            else
            {
                if ( DateTime.Today.Ticks > executionDate.Value.Ticks )
                {
                    throw new BezahlCodeException( "Execution date must be today or in future." );
                }

                this.executionDate = executionDate.Value;
            }

            if ( authority == AuthorityType.periodicsinglepayment || authority == AuthorityType.periodicsinglepaymentsepa )
            {
                if ( periodicTimeunit.ToUpper() != "M" && periodicTimeunit.ToUpper() != "W" )
                {
                    throw new BezahlCodeException( "The periodicTimeunit must be either 'M' (monthly) or 'W' (weekly)." );
                }

                this.periodicTimeunit = periodicTimeunit;
                if ( periodicTimeunitRotation < 1 || periodicTimeunitRotation > 52 )
                {
                    throw new BezahlCodeException( "The periodicTimeunitRotation must be 1 or greater. (It means repeat the payment every 'periodicTimeunitRotation' weeks/months." );
                }

                this.periodicTimeunitRotation = periodicTimeunitRotation;
                if ( periodicFirstExecutionDate.HasValue )
                {
                    this.periodicFirstExecutionDate = periodicFirstExecutionDate.Value;
                }

                if ( periodicLastExecutionDate.HasValue )
                {
                    this.periodicLastExecutionDate = periodicLastExecutionDate.Value;
                }
            }
        }

        public override string ToString()
        {
            string text = $"bank://{authority}?";
            text = text + "name=" + Uri.EscapeDataString( name ) + "&";
            if ( authority != AuthorityType.contact && authority != AuthorityType.contact_v2 )
            {
                if ( authority == AuthorityType.periodicsinglepayment || authority == AuthorityType.singledirectdebit || authority == AuthorityType.singlepayment )
                {
                    text = text + "account=" + account + "&";
                    text = text + "bnc=" + bnc + "&";
                    if ( postingKey > 0 )
                    {
                        text += $"postingkey={postingKey}&";
                    }
                }
                else
                {
                    text = text + "iban=" + iban + "&";
                    text = text + "bic=" + bic + "&";
                    if ( !string.IsNullOrEmpty( sepaReference ) )
                    {
                        text = text + "separeference=" + Uri.EscapeDataString( sepaReference ) + "&";
                    }

                    if ( authority == AuthorityType.singledirectdebitsepa )
                    {
                        if ( !string.IsNullOrEmpty( creditorId ) )
                        {
                            text = text + "creditorid=" + Uri.EscapeDataString( creditorId ) + "&";
                        }

                        if ( !string.IsNullOrEmpty( mandateId ) )
                        {
                            text = text + "mandateid=" + Uri.EscapeDataString( mandateId ) + "&";
                        }

                        if ( dateOfSignature != DateTime.MinValue )
                        {
                            text = text + "dateofsignature=" + dateOfSignature.ToString( "ddMMyyyy" ) + "&";
                        }
                    }
                }

                text += $"amount={amount:0.00}&".Replace( ".", "," );
                if ( !string.IsNullOrEmpty( reason ) )
                {
                    text = text + "reason=" + Uri.EscapeDataString( reason ) + "&";
                }

                text += $"currency={currency}&";
                text = text + "executiondate=" + executionDate.ToString( "ddMMyyyy" ) + "&";
                if ( authority == AuthorityType.periodicsinglepayment || authority == AuthorityType.periodicsinglepaymentsepa )
                {
                    text = text + "periodictimeunit=" + periodicTimeunit + "&";
                    text += $"periodictimeunitrotation={periodicTimeunitRotation}&";
                    if ( periodicFirstExecutionDate != DateTime.MinValue )
                    {
                        text = text + "periodicfirstexecutiondate=" + periodicFirstExecutionDate.ToString( "ddMMyyyy" ) + "&";
                    }

                    if ( periodicLastExecutionDate != DateTime.MinValue )
                    {
                        text = text + "periodiclastexecutiondate=" + periodicLastExecutionDate.ToString( "ddMMyyyy" ) + "&";
                    }
                }
            }
            else
            {
                if ( authority == AuthorityType.contact )
                {
                    text = text + "account=" + account + "&";
                    text = text + "bnc=" + bnc + "&";
                }
                else if ( authority == AuthorityType.contact_v2 )
                {
                    if ( !string.IsNullOrEmpty( account ) && !string.IsNullOrEmpty( bnc ) )
                    {
                        text = text + "account=" + account + "&";
                        text = text + "bnc=" + bnc + "&";
                    }
                    else
                    {
                        text = text + "iban=" + iban + "&";
                        text = text + "bic=" + bic + "&";
                    }
                }

                if ( !string.IsNullOrEmpty( reason ) )
                {
                    text = text + "reason=" + Uri.EscapeDataString( reason ) + "&";
                }
            }

            return text.Trim( '&' );
        }
    }

    public class CalendarEvent : Payload
    {
        public enum EventEncoding
        {
            iCalComplete,
            Universal
        }

        private readonly string subject;

        private readonly string description;

        private readonly string location;

        private readonly string start;

        private readonly string end;

        private readonly EventEncoding encoding;

        public CalendarEvent( string subject, string description, string location, DateTime start, DateTime end, bool allDayEvent, EventEncoding encoding = EventEncoding.Universal )
        {
            this.subject = subject;
            this.description = description;
            this.location = location;
            this.encoding = encoding;
            string text = ( allDayEvent ? "yyyyMMdd" : "yyyyMMddTHHmmss" );
            this.start = start.ToString( text );
            this.end = end.ToString( text );
        }

        public override string ToString()
        {
            string text = "BEGIN:VEVENT" + Environment.NewLine;
            text = text + "SUMMARY:" + subject + Environment.NewLine;
            text += ( ( !string.IsNullOrEmpty( description ) ) ? ( "DESCRIPTION:" + description + Environment.NewLine ) : "" );
            text += ( ( !string.IsNullOrEmpty( location ) ) ? ( "LOCATION:" + location + Environment.NewLine ) : "" );
            text = text + "DTSTART:" + start + Environment.NewLine;
            text = text + "DTEND:" + end + Environment.NewLine;
            text += "END:VEVENT";
            if ( encoding == EventEncoding.iCalComplete )
            {
                text = "BEGIN:VCALENDAR" + Environment.NewLine + "VERSION:2.0" + Environment.NewLine + text + Environment.NewLine + "END:VCALENDAR";
            }

            return text;
        }
    }

    public class OneTimePassword : Payload
    {
        public enum OneTimePasswordAuthType
        {
            TOTP,
            HOTP
        }

        public enum OneTimePasswordAuthAlgorithm
        {
            SHA1,
            SHA256,
            SHA512
        }

        [Obsolete( "This enum is obsolete, use OneTimePasswordAuthAlgorithm instead", false )]
        public enum OoneTimePasswordAuthAlgorithm
        {
            SHA1,
            SHA256,
            SHA512
        }

        public OneTimePasswordAuthType Type { get; set; }

        public string Secret { get; set; }

        public OneTimePasswordAuthAlgorithm AuthAlgorithm { get; set; }

        [Obsolete( "This property is obsolete, use AuthAlgorithm instead", false )]
        public OoneTimePasswordAuthAlgorithm Algorithm
        {
            get
            {
                return (OoneTimePasswordAuthAlgorithm)Enum.Parse( typeof( OoneTimePasswordAuthAlgorithm ), AuthAlgorithm.ToString() );
            }
            set
            {
                AuthAlgorithm = (OneTimePasswordAuthAlgorithm)Enum.Parse( typeof( OneTimePasswordAuthAlgorithm ), value.ToString() );
            }
        }

        public string Issuer { get; set; }

        public string Label { get; set; }

        public int Digits { get; set; } = 6;


        public int? Counter { get; set; }

        public int? Period { get; set; } = 30;


        public override string ToString()
        {
            return Type switch
            {
                OneTimePasswordAuthType.TOTP => TimeToString(),
                OneTimePasswordAuthType.HOTP => HMACToString(),
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        private string HMACToString()
        {
            StringBuilder stringBuilder = new StringBuilder( "otpauth://hotp/" );
            ProcessCommonFields( stringBuilder );
            stringBuilder.Append( "&counter=" + ( Counter ?? 1 ) );
            return stringBuilder.ToString();
        }

        private string TimeToString()
        {
            if ( !Period.HasValue )
            {
                throw new Exception( "Period must be set when using OneTimePasswordAuthType.TOTP" );
            }

            StringBuilder stringBuilder = new StringBuilder( "otpauth://totp/" );
            ProcessCommonFields( stringBuilder );
            if ( Period != 30 )
            {
                stringBuilder.Append( "&period=" + Period );
            }

            return stringBuilder.ToString();
        }

        private void ProcessCommonFields( StringBuilder sb )
        {
            if ( string.IsNullOrWhiteSpace( Secret ) )
            {
                throw new Exception( "Secret must be a filled out base32 encoded string" );
            }

            string text = Secret.Replace( " ", "" );
            string text2 = null;
            string text3 = null;
            if ( !string.IsNullOrWhiteSpace( Issuer ) )
            {
                if ( Issuer.Contains( ":" ) )
                {
                    throw new Exception( "Issuer must not have a ':'" );
                }

                text2 = Uri.EscapeUriString( Issuer );
            }

            if ( !string.IsNullOrWhiteSpace( Label ) )
            {
                if ( Label.Contains( ":" ) )
                {
                    throw new Exception( "Label must not have a ':'" );
                }

                text3 = Uri.EscapeUriString( Label );
            }

            if ( text3 != null )
            {
                if ( text2 != null )
                {
                    text3 = text2 + ":" + text3;
                }
            }
            else if ( text2 != null )
            {
                text3 = text2;
            }

            if ( text3 != null )
            {
                sb.Append( text3 );
            }

            sb.Append( "?secret=" + text );
            if ( text2 != null )
            {
                sb.Append( "&issuer=" + text2 );
            }

            if ( Digits != 6 )
            {
                sb.Append( "&digits=" + Digits );
            }
        }
    }

    public class ShadowSocksConfig : Payload
    {
        public enum Method
        {
            Chacha20IetfPoly1305,
            Aes128Gcm,
            Aes192Gcm,
            Aes256Gcm,
            XChacha20IetfPoly1305,
            Aes128Cfb,
            Aes192Cfb,
            Aes256Cfb,
            Aes128Ctr,
            Aes192Ctr,
            Aes256Ctr,
            Camellia128Cfb,
            Camellia192Cfb,
            Camellia256Cfb,
            Chacha20Ietf,
            Aes256Cb,
            Aes128Ofb,
            Aes192Ofb,
            Aes256Ofb,
            Aes128Cfb1,
            Aes192Cfb1,
            Aes256Cfb1,
            Aes128Cfb8,
            Aes192Cfb8,
            Aes256Cfb8,
            Chacha20,
            BfCfb,
            Rc4Md5,
            Salsa20,
            DesCfb,
            IdeaCfb,
            Rc2Cfb,
            Cast5Cfb,
            Salsa20Ctr,
            Rc4,
            SeedCfb,
            Table
        }

        public class ShadowSocksConfigException : Exception
        {
            public ShadowSocksConfigException()
            {
            }

            public ShadowSocksConfigException( string message )
                : base( message )
            {
            }

            public ShadowSocksConfigException( string message, Exception inner )
                : base( message, inner )
            {
            }
        }

        private readonly string hostname;

        private readonly string password;

        private readonly string tag;

        private readonly string methodStr;

        private readonly string parameter;

        private readonly Method method;

        private readonly int port;

        private Dictionary<string, string> encryptionTexts = new Dictionary<string, string>
        {
            { "Chacha20IetfPoly1305", "chacha20-ietf-poly1305" },
            { "Aes128Gcm", "aes-128-gcm" },
            { "Aes192Gcm", "aes-192-gcm" },
            { "Aes256Gcm", "aes-256-gcm" },
            { "XChacha20IetfPoly1305", "xchacha20-ietf-poly1305" },
            { "Aes128Cfb", "aes-128-cfb" },
            { "Aes192Cfb", "aes-192-cfb" },
            { "Aes256Cfb", "aes-256-cfb" },
            { "Aes128Ctr", "aes-128-ctr" },
            { "Aes192Ctr", "aes-192-ctr" },
            { "Aes256Ctr", "aes-256-ctr" },
            { "Camellia128Cfb", "camellia-128-cfb" },
            { "Camellia192Cfb", "camellia-192-cfb" },
            { "Camellia256Cfb", "camellia-256-cfb" },
            { "Chacha20Ietf", "chacha20-ietf" },
            { "Aes256Cb", "aes-256-cfb" },
            { "Aes128Ofb", "aes-128-ofb" },
            { "Aes192Ofb", "aes-192-ofb" },
            { "Aes256Ofb", "aes-256-ofb" },
            { "Aes128Cfb1", "aes-128-cfb1" },
            { "Aes192Cfb1", "aes-192-cfb1" },
            { "Aes256Cfb1", "aes-256-cfb1" },
            { "Aes128Cfb8", "aes-128-cfb8" },
            { "Aes192Cfb8", "aes-192-cfb8" },
            { "Aes256Cfb8", "aes-256-cfb8" },
            { "Chacha20", "chacha20" },
            { "BfCfb", "bf-cfb" },
            { "Rc4Md5", "rc4-md5" },
            { "Salsa20", "salsa20" },
            { "DesCfb", "des-cfb" },
            { "IdeaCfb", "idea-cfb" },
            { "Rc2Cfb", "rc2-cfb" },
            { "Cast5Cfb", "cast5-cfb" },
            { "Salsa20Ctr", "salsa20-ctr" },
            { "Rc4", "rc4" },
            { "SeedCfb", "seed-cfb" },
            { "Table", "table" }
        };

        private Dictionary<string, string> UrlEncodeTable = new Dictionary<string, string>
        {
            [" "] = "+",
            ["\0"] = "%00",
            ["\t"] = "%09",
            ["\n"] = "%0a",
            ["\r"] = "%0d",
            ["\""] = "%22",
            ["#"] = "%23",
            ["$"] = "%24",
            ["%"] = "%25",
            ["&"] = "%26",
            ["'"] = "%27",
            ["+"] = "%2b",
            [","] = "%2c",
            ["/"] = "%2f",
            [":"] = "%3a",
            [";"] = "%3b",
            ["<"] = "%3c",
            ["="] = "%3d",
            [">"] = "%3e",
            ["?"] = "%3f",
            ["@"] = "%40",
            ["["] = "%5b",
            ["\\"] = "%5c",
            ["]"] = "%5d",
            ["^"] = "%5e",
            ["`"] = "%60",
            ["{"] = "%7b",
            ["|"] = "%7c",
            ["}"] = "%7d",
            ["~"] = "%7e"
        };

        public ShadowSocksConfig( string hostname, int port, string password, Method method, string tag = null )
            : this( hostname, port, password, method, (Dictionary<string, string>)null, tag )
        {
        }

        public ShadowSocksConfig( string hostname, int port, string password, Method method, string plugin, string pluginOption, string tag = null )
            : this( hostname, port, password, method, new Dictionary<string, string> { ["plugin"] = plugin + ( string.IsNullOrEmpty( pluginOption ) ? "" : ( ";" + pluginOption ) ) }, tag )
        {
        }

        private string UrlEncode( string i )
        {
            string text = i;
            foreach ( KeyValuePair<string, string> item in UrlEncodeTable )
            {
                text = text.Replace( item.Key, item.Value );
            }

            return text;
        }

        public ShadowSocksConfig( string hostname, int port, string password, Method method, Dictionary<string, string> parameters, string tag = null )
        {
            this.hostname = ( ( Uri.CheckHostName( hostname ) == UriHostNameType.IPv6 ) ? ( "[" + hostname + "]" ) : hostname );
            if ( port < 1 || port > 65535 )
            {
                throw new ShadowSocksConfigException( "Value of 'port' must be within 0 and 65535." );
            }

            this.port = port;
            this.password = password;
            this.method = method;
            methodStr = encryptionTexts[method.ToString()];
            this.tag = tag;
            if ( parameters != null )
            {
                parameter = string.Join( "&", parameters.Select( ( KeyValuePair<string, string> kv ) => UrlEncode( kv.Key ) + "=" + UrlEncode( kv.Value ) ).ToArray() );
            }
        }

        public override string ToString()
        {
            if ( string.IsNullOrEmpty( parameter ) )
            {
                string s = $"{methodStr}:{password}@{hostname}:{port}";
                string text = Convert.ToBase64String( Encoding.UTF8.GetBytes( s ) );
                return "ss://" + text + ( ( !string.IsNullOrEmpty( tag ) ) ? ( "#" + tag ) : string.Empty );
            }

            string s2 = methodStr + ":" + password;
            string text2 = Convert.ToBase64String( Encoding.UTF8.GetBytes( s2 ) ).Replace( '+', '-' ).Replace( '/', '_' )
                .TrimEnd( '=' );
            return string.Format( "ss://{0}@{1}:{2}/?{3}{4}", text2, hostname, port, parameter, ( !string.IsNullOrEmpty( tag ) ) ? ( "#" + tag ) : string.Empty );
        }
    }

    public class MoneroTransaction : Payload
    {
        public class MoneroTransactionException : Exception
        {
            public MoneroTransactionException()
            {
            }

            public MoneroTransactionException( string message )
                : base( message )
            {
            }

            public MoneroTransactionException( string message, Exception inner )
                : base( message, inner )
            {
            }
        }

        private readonly string address;

        private readonly string txPaymentId;

        private readonly string recipientName;

        private readonly string txDescription;

        private readonly float? txAmount;

        public MoneroTransaction( string address, float? txAmount = null, string txPaymentId = null, string recipientName = null, string txDescription = null )
        {
            if ( string.IsNullOrEmpty( address ) )
            {
                throw new MoneroTransactionException( "The address is mandatory and has to be set." );
            }

            this.address = address;
            if ( txAmount.HasValue && txAmount <= 0f )
            {
                throw new MoneroTransactionException( "Value of 'txAmount' must be greater than 0." );
            }

            this.txAmount = txAmount;
            this.txPaymentId = txPaymentId;
            this.recipientName = recipientName;
            this.txDescription = txDescription;
        }

        public override string ToString()
        {
            string text = string.Concat( string.Concat( "monero://" + address + ( ( !string.IsNullOrEmpty( txPaymentId ) || !string.IsNullOrEmpty( recipientName ) || !string.IsNullOrEmpty( txDescription ) || txAmount.HasValue ) ? "?" : string.Empty ), ( !string.IsNullOrEmpty( txPaymentId ) ) ? ( "tx_payment_id=" + Uri.EscapeDataString( txPaymentId ) + "&" ) : string.Empty ), ( !string.IsNullOrEmpty( recipientName ) ) ? ( "recipient_name=" + Uri.EscapeDataString( recipientName ) + "&" ) : string.Empty );
            string text2;
            if ( !txAmount.HasValue )
            {
                text2 = string.Empty;
            }
            else
            {
                float? num = txAmount;
                text2 = "tx_amount=" + num.ToString()!.Replace( ",", "." ) + "&";
            }

            return string.Concat( text + text2, ( !string.IsNullOrEmpty( txDescription ) ) ? ( "tx_description=" + Uri.EscapeDataString( txDescription ) ) : string.Empty ).TrimEnd( '&' );
        }
    }

    public class SlovenianUpnQr : Payload
    {
        private string _payerName = "";

        private string _payerAddress = "";

        private string _payerPlace = "";

        private string _amount = "";

        private string _code = "";

        private string _purpose = "";

        private string _deadLine = "";

        private string _recipientIban = "";

        private string _recipientName = "";

        private string _recipientAddress = "";

        private string _recipientPlace = "";

        private string _recipientSiModel = "";

        private string _recipientSiReference = "";

        public override int Version => 15;

        public override EccLevel EccLevel => EccLevel.M;

        public override EciMode EciMode => EciMode.Iso8859_2;

        private string LimitLength( string value, int maxLength )
        {
            if ( value.Length > maxLength )
            {
                return value.Substring( 0, maxLength );
            }

            return value;
        }

        public SlovenianUpnQr( string payerName, string payerAddress, string payerPlace, string recipientName, string recipientAddress, string recipientPlace, string recipientIban, string description, double amount, string recipientSiModel = "SI00", string recipientSiReference = "", string code = "OTHR" )
            : this( payerName, payerAddress, payerPlace, recipientName, recipientAddress, recipientPlace, recipientIban, description, amount, null, recipientSiModel, recipientSiReference, code )
        {
        }

        public SlovenianUpnQr( string payerName, string payerAddress, string payerPlace, string recipientName, string recipientAddress, string recipientPlace, string recipientIban, string description, double amount, DateTime? deadline, string recipientSiModel = "SI99", string recipientSiReference = "", string code = "OTHR" )
        {
            _payerName = LimitLength( payerName.Trim(), 33 );
            _payerAddress = LimitLength( payerAddress.Trim(), 33 );
            _payerPlace = LimitLength( payerPlace.Trim(), 33 );
            _amount = FormatAmount( amount );
            _code = LimitLength( code.Trim().ToUpper(), 4 );
            _purpose = LimitLength( description.Trim(), 42 );
            _deadLine = ( ( !deadline.HasValue ) ? "" : deadline?.ToString( "dd.MM.yyyy" ) );
            _recipientIban = LimitLength( recipientIban.Trim(), 34 );
            _recipientName = LimitLength( recipientName.Trim(), 33 );
            _recipientAddress = LimitLength( recipientAddress.Trim(), 33 );
            _recipientPlace = LimitLength( recipientPlace.Trim(), 33 );
            _recipientSiModel = LimitLength( recipientSiModel.Trim().ToUpper(), 4 );
            _recipientSiReference = LimitLength( recipientSiReference.Trim(), 22 );
        }

        private string FormatAmount( double amount )
        {
            int num = (int)Math.Round( amount * 100.0 );
            return $"{num:00000000000}";
        }

        private int CalculateChecksum()
        {
            return 5 + _payerName.Length + _payerAddress.Length + _payerPlace.Length + _amount.Length + _code.Length + _purpose.Length + _deadLine.Length + _recipientIban.Length + _recipientName.Length + _recipientAddress.Length + _recipientPlace.Length + _recipientSiModel.Length + _recipientSiReference.Length + 19;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append( "UPNQR" );
            stringBuilder.Append( '\n' ).Append( '\n' ).Append( '\n' )
                .Append( '\n' )
                .Append( '\n' );
            stringBuilder.Append( _payerName ).Append( '\n' );
            stringBuilder.Append( _payerAddress ).Append( '\n' );
            stringBuilder.Append( _payerPlace ).Append( '\n' );
            stringBuilder.Append( _amount ).Append( '\n' ).Append( '\n' )
                .Append( '\n' );
            stringBuilder.Append( _code.ToUpper() ).Append( '\n' );
            stringBuilder.Append( _purpose ).Append( '\n' );
            stringBuilder.Append( _deadLine ).Append( '\n' );
            stringBuilder.Append( _recipientIban.ToUpper() ).Append( '\n' );
            stringBuilder.Append( _recipientSiModel ).Append( _recipientSiReference ).Append( '\n' );
            stringBuilder.Append( _recipientName ).Append( '\n' );
            stringBuilder.Append( _recipientAddress ).Append( '\n' );
            stringBuilder.Append( _recipientPlace ).Append( '\n' );
            stringBuilder.AppendFormat( "{0:000}", CalculateChecksum() ).Append( '\n' );
            return stringBuilder.ToString();
        }
    }

    public class RussiaPaymentOrder : Payload
    {
        private class MandatoryFields
        {
            public string Name;

            public string PersonalAcc;

            public string BankName;

            public string BIC;

            public string CorrespAcc;
        }

        public class OptionalFields
        {
            private string _sum;

            private string _purpose;

            private string _payeeInn;

            private string _payerInn;

            private string _drawerStatus;

            private string _kpp;

            private string _cbc;

            private string _oktmo;

            private string _paytReason;

            private string _taxPeriod;

            private string _docNo;

            private string _taxPaytKind;

            public string Sum
            {
                get
                {
                    return _sum;
                }
                set
                {
                    _sum = ValidateInput( value, "Sum", "^\\d{1,18}$" );
                }
            }

            public string Purpose
            {
                get
                {
                    return _purpose;
                }
                set
                {
                    _purpose = ValidateInput( value, "Purpose", "^.{1,160}$" );
                }
            }

            public string PayeeINN
            {
                get
                {
                    return _payeeInn;
                }
                set
                {
                    _payeeInn = ValidateInput( value, "PayeeINN", "^.{1,12}$" );
                }
            }

            public string PayerINN
            {
                get
                {
                    return _payerInn;
                }
                set
                {
                    _payerInn = ValidateInput( value, "PayerINN", "^.{1,12}$" );
                }
            }

            public string DrawerStatus
            {
                get
                {
                    return _drawerStatus;
                }
                set
                {
                    _drawerStatus = ValidateInput( value, "DrawerStatus", "^.{1,2}$" );
                }
            }

            public string KPP
            {
                get
                {
                    return _kpp;
                }
                set
                {
                    _kpp = ValidateInput( value, "KPP", "^.{1,9}$" );
                }
            }

            public string CBC
            {
                get
                {
                    return _cbc;
                }
                set
                {
                    _cbc = ValidateInput( value, "CBC", "^.{1,20}$" );
                }
            }

            public string OKTMO
            {
                get
                {
                    return _oktmo;
                }
                set
                {
                    _oktmo = ValidateInput( value, "OKTMO", "^.{1,11}$" );
                }
            }

            public string PaytReason
            {
                get
                {
                    return _paytReason;
                }
                set
                {
                    _paytReason = ValidateInput( value, "PaytReason", "^.{1,2}$" );
                }
            }

            public string TaxPeriod
            {
                get
                {
                    return _taxPeriod;
                }
                set
                {
                    _taxPeriod = ValidateInput( value, "ТaxPeriod", "^.{1,10}$" );
                }
            }

            public string DocNo
            {
                get
                {
                    return _docNo;
                }
                set
                {
                    _docNo = ValidateInput( value, "DocNo", "^.{1,15}$" );
                }
            }

            public DateTime? DocDate { get; set; }

            public string TaxPaytKind
            {
                get
                {
                    return _taxPaytKind;
                }
                set
                {
                    _taxPaytKind = ValidateInput( value, "TaxPaytKind", "^.{1,2}$" );
                }
            }

            public string LastName { get; set; }

            public string FirstName { get; set; }

            public string MiddleName { get; set; }

            public string PayerAddress { get; set; }

            public string PersonalAccount { get; set; }

            public string DocIdx { get; set; }

            public string PensAcc { get; set; }

            public string Contract { get; set; }

            public string PersAcc { get; set; }

            public string Flat { get; set; }

            public string Phone { get; set; }

            public string PayerIdType { get; set; }

            public string PayerIdNum { get; set; }

            public string ChildFio { get; set; }

            public DateTime? BirthDate { get; set; }

            public string PaymTerm { get; set; }

            public string PaymPeriod { get; set; }

            public string Category { get; set; }

            public string ServiceName { get; set; }

            public string CounterId { get; set; }

            public string CounterVal { get; set; }

            public string QuittId { get; set; }

            public DateTime? QuittDate { get; set; }

            public string InstNum { get; set; }

            public string ClassNum { get; set; }

            public string SpecFio { get; set; }

            public string AddAmount { get; set; }

            public string RuleId { get; set; }

            public string ExecId { get; set; }

            public string RegType { get; set; }

            public string UIN { get; set; }

            public TechCode? TechCode { get; set; }
        }

        public enum TechCode
        {
            Мобильная_связь_стационарный_телефон = 1,
            Коммунальные_услуги_ЖКХAFN,
            ГИБДД_налоги_пошлины_бюджетные_платежи,
            Охранные_услуги,
            Услуги_оказываемые_УФМС,
            ПФР,
            Погашение_кредитов,
            Образовательные_учреждения,
            Интернет_и_ТВ,
            Электронные_деньги,
            Отдых_и_путешествия,
            Инвестиции_и_страхование,
            Спорт_и_здоровье,
            Благотворительные_и_общественные_организации,
            Прочие_услуги
        }

        public enum CharacterSets
        {
            windows_1251 = 1,
            utf_8,
            koi8_r
        }

        public class RussiaPaymentOrderException : Exception
        {
            public RussiaPaymentOrderException( string message )
                : base( message )
            {
            }
        }

        private CharacterSets characterSet;

        private MandatoryFields mFields;

        private OptionalFields oFields;

        private string separator = "|";

        private RussiaPaymentOrder()
        {
            mFields = new MandatoryFields();
            oFields = new OptionalFields();
        }

        public RussiaPaymentOrder( string name, string personalAcc, string bankName, string BIC, string correspAcc, OptionalFields optionalFields = null, CharacterSets characterSet = CharacterSets.utf_8 )
            : this()
        {
            this.characterSet = characterSet;
            mFields.Name = ValidateInput( name, "Name", "^.{1,160}$" );
            mFields.PersonalAcc = ValidateInput( personalAcc, "PersonalAcc", "^[1-9]\\d{4}[0-9ABCEHKMPTX]\\d{14}$" );
            mFields.BankName = ValidateInput( bankName, "BankName", "^.{1,45}$" );
            mFields.BIC = ValidateInput( BIC, "BIC", "^\\d{9}$" );
            mFields.CorrespAcc = ValidateInput( correspAcc, "CorrespAcc", "^[1-9]\\d{4}[0-9ABCEHKMPTX]\\d{14}$" );
            if ( optionalFields != null )
            {
                oFields = optionalFields;
            }
        }

        public override string ToString()
        {
            string name = characterSet.ToString().Replace( "_", "-" );
            byte[] bytes = ToBytes();
            Encoding.RegisterProvider( CodePagesEncodingProvider.Instance );
            return Encoding.GetEncoding( name ).GetString( bytes );
        }

        public byte[] ToBytes()
        {
            separator = DetermineSeparator();
            string[] obj = new string[17]
            {
                "ST0001", null, null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null
            };
            int num = (int)characterSet;
            obj[1] = num.ToString();
            obj[2] = separator;
            obj[3] = "Name=";
            obj[4] = mFields.Name;
            obj[5] = separator;
            obj[6] = "PersonalAcc=";
            obj[7] = mFields.PersonalAcc;
            obj[8] = separator;
            obj[9] = "BankName=";
            obj[10] = mFields.BankName;
            obj[11] = separator;
            obj[12] = "BIC=";
            obj[13] = mFields.BIC;
            obj[14] = separator;
            obj[15] = "CorrespAcc=";
            obj[16] = mFields.CorrespAcc;
            string text = string.Concat( obj );
            List<string> optionalFieldsAsList = GetOptionalFieldsAsList();
            if ( optionalFieldsAsList.Count > 0 )
            {
                text = text + "|" + string.Join( "|", optionalFieldsAsList.ToArray() );
            }

            text += separator;
            Encoding.RegisterProvider( CodePagesEncodingProvider.Instance );
            string name = characterSet.ToString().Replace( "_", "-" );
            byte[] array = Encoding.Convert( Encoding.UTF8, Encoding.GetEncoding( name ), Encoding.UTF8.GetBytes( text ) );
            if ( array.Length > 300 )
            {
                throw new RussiaPaymentOrderException( $"Data too long. Payload must not exceed 300 bytes, but actually is {array.Length} bytes long. Remove additional data fields or shorten strings/values." );
            }

            return array;
        }

        private string DetermineSeparator()
        {
            List<string> mandatoryFieldsAsList = GetMandatoryFieldsAsList();
            List<string> optionalFieldsAsList = GetOptionalFieldsAsList();
            string[] array = new string[21]
            {
                "|", "#", ";", ":", "^", "_", "~", "{", "}", "!",
                "#", "$", "%", "&", "(", ")", "*", "+", ",", "/",
                "@"
            };
            foreach ( string sepCandidate in array )
            {
                if ( !mandatoryFieldsAsList.Any( ( string x ) => x.Contains( sepCandidate ) ) && !optionalFieldsAsList.Any( ( string x ) => x.Contains( sepCandidate ) ) )
                {
                    return sepCandidate;
                }
            }

            throw new RussiaPaymentOrderException( "No valid separator found." );
        }

        private List<string> GetOptionalFieldsAsList()
        {
            return ( from field in oFields.GetType().GetProperties()
                     where field.GetValue( oFields, null ) != null
                     select field ).Select( delegate ( PropertyInfo field )
                     {
                         object value = field.GetValue( oFields, null );
                         string text = ( field.PropertyType.Equals( typeof( DateTime? ) ) ? ( (DateTime)value ).ToString( "dd.MM.yyyy" ) : value.ToString() );
                         return field.Name + "=" + text;
                     } ).ToList();
        }

        private List<string> GetMandatoryFieldsAsList()
        {
            return ( from field in mFields.GetType().GetFields()
                     where field.GetValue( mFields ) != null
                     select field ).Select( delegate ( FieldInfo field )
                     {
                         object value = field.GetValue( mFields );
                         string text = ( field.FieldType.Equals( typeof( DateTime? ) ) ? ( (DateTime)value ).ToString( "dd.MM.yyyy" ) : value.ToString() );
                         return field.Name + "=" + text;
                     } ).ToList();
        }

        private static string ValidateInput( string input, string fieldname, string pattern, string errorText = null )
        {
            return ValidateInput( input, fieldname, new string[1] { pattern }, errorText );
        }

        private static string ValidateInput( string input, string fieldname, string[] patterns, string errorText = null )
        {
            if ( input == null )
            {
                throw new RussiaPaymentOrderException( "The input for '" + fieldname + "' must not be null." );
            }

            foreach ( string text in patterns )
            {
                if ( !Regex.IsMatch( input, text ) )
                {
                    throw new RussiaPaymentOrderException( errorText ?? ( "The input for '" + fieldname + "' (" + input + ") doesn't match the pattern " + text ) );
                }
            }

            return input;
        }
    }

    private static bool IsValidIban( string iban )
    {
        string text = iban.ToUpper().Replace( " ", "" ).Replace( "-", "" );
        bool flag = Regex.IsMatch( text, "^[a-zA-Z]{2}[0-9]{2}([a-zA-Z0-9]?){16,30}$" );
        bool flag2 = false;
        string text2 = ( text.Substring( 4 ) + text.Substring( 0, 4 ) ).ToCharArray().Aggregate( "", ( string current, char c ) => current + ( char.IsLetter( c ) ? ( c - 55 ).ToString() : c.ToString() ) );
        int result = 0;
        for ( int i = 0; i < (int)Math.Ceiling( (double)( text2.Length - 2 ) / 7.0 ); i++ )
        {
            int num = ( ( i != 0 ) ? 2 : 0 );
            int num2 = i * 7 + num;
            if ( !int.TryParse( ( ( i == 0 ) ? "" : result.ToString() ) + text2.Substring( num2, Math.Min( 9 - num, text2.Length - num2 ) ), NumberStyles.Any, CultureInfo.InvariantCulture, out result ) )
            {
                break;
            }

            result %= 97;
        }

        flag2 = result == 1;
        return flag && flag2;
    }

    private static bool IsValidQRIban( string iban )
    {
        bool flag = false;
        try
        {
            int num = Convert.ToInt32( iban.ToUpper().Replace( " ", "" ).Replace( "-", "" )
                .Substring( 4, 5 ) );
            flag = num >= 30000 && num <= 31999;
        }
        catch
        {
        }

        return IsValidIban( iban ) && flag;
    }

    private static bool IsValidBic( string bic )
    {
        return Regex.IsMatch( bic.Replace( " ", "" ), "^([a-zA-Z]{4}[a-zA-Z]{2}[a-zA-Z0-9]{2}([a-zA-Z0-9]{3})?)$" );
    }

    private static string ConvertStringToEncoding( string message, string encoding )
    {
        Encoding encoding2 = Encoding.GetEncoding( encoding );
        Encoding uTF = Encoding.UTF8;
        byte[] bytes = uTF.GetBytes( message );
        byte[] array = Encoding.Convert( uTF, encoding2, bytes );
        return encoding2.GetString( array, 0, array.Length );
    }

    private static string EscapeInput( string inp, bool simple = false )
    {
        char[] array = new char[4] { '\\', ';', ',', ':' };
        if ( simple )
        {
            array = new char[1] { ':' };
        }

        char[] array2 = array;
        for ( int i = 0; i < array2.Length; i++ )
        {
            char c = array2[i];
            inp = inp.Replace( c.ToString(), "\\" + c );
        }

        return inp;
    }

    public static bool ChecksumMod10( string digits )
    {
        if ( string.IsNullOrEmpty( digits ) || digits.Length < 2 )
        {
            return false;
        }

        int[] array = new int[10] { 0, 9, 4, 6, 8, 2, 7, 1, 3, 5 };
        int num = 0;
        for ( int i = 0; i < digits.Length - 1; i++ )
        {
            int num2 = Convert.ToInt32( digits[i] ) - 48;
            num = array[( num2 + num ) % 10];
        }

        return ( 10 - num ) % 10 == Convert.ToInt32( digits[digits.Length - 1] ) - 48;
    }

    private static bool isHexStyle( string inp )
    {
        if ( !Regex.IsMatch( inp, "\\A\\b[0-9a-fA-F]+\\b\\Z" ) )
        {
            return Regex.IsMatch( inp, "\\A\\b(0[xX])?[0-9a-fA-F]+\\b\\Z" );
        }

        return true;
    }
}