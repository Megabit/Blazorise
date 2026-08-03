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
    /// <summary>
    /// Base contract for content encoded into a QR code.
    /// </summary>
    public abstract class Payload
    {
        /// <summary>
        /// QR version requested by this payload.
        /// </summary>
        public virtual int Version => -1;

        /// <summary>
        /// Error-correction level requested by this payload.
        /// </summary>
        public virtual EccLevel EccLevel => EccLevel.M;

        /// <summary>
        /// Character-encoding mode requested by this payload.
        /// </summary>
        public virtual EciMode EciMode => EciMode.Default;

        /// <inheritdoc />
        public abstract override string ToString();
    }

    /// <summary>
    /// Encodes Wi-Fi credentials for quick network access.
    /// </summary>
    public class WiFi : Payload
    {
        /// <summary>
        /// Lists the supported authentication values.
        /// </summary>
        public enum Authentication
        {
            /// <summary>
            /// Uses wep authentication.
            /// </summary>
            WEP,
            /// <summary>
            /// Uses wpa authentication.
            /// </summary>
            WPA,
            /// <summary>
            /// Uses nopass authentication.
            /// </summary>
            nopass
        }

        private readonly string ssid;

        private readonly string password;

        private readonly string authenticationMode;

        private readonly bool isHiddenSsid;

        /// <summary>
        /// Generates a WiFi payload. Scanned by a QR Code scanner app, the device will connect to the WiFi.
        /// </summary>
        /// <param name="ssid">SSID of the WiFi network</param>
        /// <param name="password">Password of the WiFi network</param>
        /// <param name="authenticationMode">Authentification mode (WEP, WPA, WPA2)</param>
        /// <param name="isHiddenSSID">Set flag, if the WiFi network hides its SSID</param>
        /// <param name="escapeHexStrings">Set flag, if ssid/password is delivered as HEX string. Note: May not be supported on iOS devices.</param>
        public WiFi( string ssid, string password, Authentication authenticationMode, bool isHiddenSSID = false, bool escapeHexStrings = true )
        {
            this.ssid = EscapeInput( ssid );
            this.ssid = ( ( escapeHexStrings && isHexStyle( this.ssid ) ) ? ( "\"" + this.ssid + "\"" ) : this.ssid );
            this.password = EscapeInput( password );
            this.password = ( ( escapeHexStrings && isHexStyle( this.password ) ) ? ( "\"" + this.password + "\"" ) : this.password );
            this.authenticationMode = authenticationMode.ToString();
            isHiddenSsid = isHiddenSSID;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return "WIFI:T:" + authenticationMode + ";S:" + ssid + ";P:" + password + ";" + ( isHiddenSsid ? "H:true" : string.Empty ) + ";";
        }
    }

    /// <summary>
    /// Composes an email recipient, subject, and message payload.
    /// </summary>
    public class Mail : Payload
    {
        /// <summary>
        /// Lists the supported mail encoding values.
        /// </summary>
        public enum MailEncoding
        {
            /// <summary>
            /// Encodes the payload using mailto.
            /// </summary>
            MAILTO,
            /// <summary>
            /// Encodes the payload using matmsg.
            /// </summary>
            MATMSG,
            /// <summary>
            /// Encodes the payload using smtp.
            /// </summary>
            SMTP
        }

        private readonly string mailReceiver;

        private readonly string subject;

        private readonly string message;

        private readonly MailEncoding encoding;

        /// <summary>
        /// Creates an email payload with subject and message/text
        /// </summary>
        /// <param name="mailReceiver">Receiver's email address</param>
        /// <param name="subject">Subject line of the email</param>
        /// <param name="message">Message content of the email</param>
        /// <param name="encoding">Payload encoding type. Choose dependent on your QR Code scanner app.</param>
        public Mail( string mailReceiver = null, string subject = null, string message = null, MailEncoding encoding = MailEncoding.MAILTO )
        {
            this.mailReceiver = mailReceiver;
            this.subject = subject;
            this.message = message;
            this.encoding = encoding;
        }

        /// <inheritdoc />
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

    /// <summary>
    /// Prepares an SMS recipient and optional message body.
    /// </summary>
    public class SMS : Payload
    {
        /// <summary>
        /// Lists the supported sms encoding values.
        /// </summary>
        public enum SMSEncoding
        {
            /// <summary>
            /// Encodes the payload using sms.
            /// </summary>
            SMS,
            /// <summary>
            /// Encodes the payload using smsto.
            /// </summary>
            SMSTO,
            /// <summary>
            /// Encodes the payload using sms_i os.
            /// </summary>
            SMS_iOS
        }

        private readonly string number;

        private readonly string subject;

        private readonly SMSEncoding encoding;

        /// <summary>
        /// Creates a SMS payload without text
        /// </summary>
        /// <param name="number">Receiver phone number</param>
        /// <param name="encoding">Encoding type</param>
        public SMS( string number, SMSEncoding encoding = SMSEncoding.SMS )
        {
            this.number = number;
            subject = string.Empty;
            this.encoding = encoding;
        }

        /// <summary>
        /// Creates a SMS payload with text (subject)
        /// </summary>
        /// <param name="number">Receiver phone number</param>
        /// <param name="subject">Text of the SMS</param>
        /// <param name="encoding">Encoding type</param>
        public SMS( string number, string subject, SMSEncoding encoding = SMSEncoding.SMS )
        {
            this.number = number;
            this.subject = subject;
            this.encoding = encoding;
        }

        /// <inheritdoc />
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

    /// <summary>
    /// Prepares an MMS recipient and optional message body.
    /// </summary>
    public class MMS : Payload
    {
        /// <summary>
        /// Lists the supported mms encoding values.
        /// </summary>
        public enum MMSEncoding
        {
            /// <summary>
            /// Encodes the payload using mms.
            /// </summary>
            MMS,
            /// <summary>
            /// Encodes the payload using mmsto.
            /// </summary>
            MMSTO
        }

        private readonly string number;

        private readonly string subject;

        private readonly MMSEncoding encoding;

        /// <summary>
        /// Creates a MMS payload without text
        /// </summary>
        /// <param name="number">Receiver phone number</param>
        /// <param name="encoding">Encoding type</param>
        public MMS( string number, MMSEncoding encoding = MMSEncoding.MMS )
        {
            this.number = number;
            subject = string.Empty;
            this.encoding = encoding;
        }

        /// <summary>
        /// Creates a MMS payload with text (subject)
        /// </summary>
        /// <param name="number">Receiver phone number</param>
        /// <param name="subject">Text of the MMS</param>
        /// <param name="encoding">Encoding type</param>
        public MMS( string number, string subject, MMSEncoding encoding = MMSEncoding.MMS )
        {
            this.number = number;
            this.subject = subject;
            this.encoding = encoding;
        }

        /// <inheritdoc />
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

    /// <summary>
    /// Encodes latitude and longitude as a geographic location.
    /// </summary>
    public class Geolocation : Payload
    {
        /// <summary>
        /// Lists the supported geolocation encoding values.
        /// </summary>
        public enum GeolocationEncoding
        {
            /// <summary>
            /// Encodes the payload using geo.
            /// </summary>
            GEO,
            /// <summary>
            /// Encodes the payload using google maps.
            /// </summary>
            GoogleMaps
        }

        private readonly string latitude;

        private readonly string longitude;

        private readonly GeolocationEncoding encoding;

        /// <summary>
        /// Generates a geo location payload. Supports raw location (GEO encoding) or Google Maps link (GoogleMaps encoding)
        /// </summary>
        /// <param name="latitude">Latitude with . as splitter</param>
        /// <param name="longitude">Longitude with . as splitter</param>
        /// <param name="encoding">Encoding type - GEO or GoogleMaps</param>
        public Geolocation( string latitude, string longitude, GeolocationEncoding encoding = GeolocationEncoding.GEO )
        {
            this.latitude = latitude.Replace( ",", "." );
            this.longitude = longitude.Replace( ",", "." );
            this.encoding = encoding;
        }

        /// <inheritdoc />
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

    /// <summary>
    /// Creates a QR action that dials a telephone number.
    /// </summary>
    public class PhoneNumber : Payload
    {
        private readonly string number;

        /// <summary>
        /// Generates a phone call payload
        /// </summary>
        /// <param name="number">Phonenumber of the receiver</param>
        public PhoneNumber( string number )
        {
            this.number = number;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return "tel:" + number;
        }
    }

    /// <summary>
    /// Starts a Skype call to the supplied username.
    /// </summary>
    public class SkypeCall : Payload
    {
        private readonly string skypeUsername;

        /// <summary>
        /// Generates a Skype call payload
        /// </summary>
        /// <param name="skypeUsername">Skype username which will be called</param>
        public SkypeCall( string skypeUsername )
        {
            this.skypeUsername = skypeUsername;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return "skype:" + skypeUsername + "?call";
        }
    }

    /// <summary>
    /// Encodes a web address that opens when scanned.
    /// </summary>
    public class Url : Payload
    {
        private readonly string url;

        /// <summary>
        /// Generates a link. If not given, http/https protocol will be added.
        /// </summary>
        /// <param name="url">Link url target</param>
        public Url( string url )
        {
            this.url = url;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            if ( url.StartsWith( "http" ) )
            {
                return url;
            }

            return "http://" + url;
        }
    }

    /// <summary>
    /// Opens a WhatsApp conversation with an optional recipient.
    /// </summary>
    public class WhatsAppMessage : Payload
    {
        private readonly string number;

        private readonly string message;

        /// <summary>
        /// Composes a WhatsApp message for the supplied recipient.
        /// </summary>
        /// <param name="number">Receiver phone number in full international format.
        /// Omit any zeroes, brackets, or dashes when adding the phone number in international format.
        /// Use: 1XXXXXXXXXX | Don't use: +001-(XXX)XXXXXXX
        /// </param>
        /// <param name="message">The message</param>
        public WhatsAppMessage( string number, string message )
        {
            this.number = number;
            this.message = message;
        }

        /// <summary>
        /// Let's you compose a WhatApp message. When scanned the user is asked to choose a contact who will receive the message.
        /// </summary>
        /// <param name="message">The message</param>
        public WhatsAppMessage( string message )
        {
            number = string.Empty;
            this.message = message;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            string text = Regex.Replace( number, "^[0+]+|[ ()-]", string.Empty );
            return "https://wa.me/" + text + "?text=" + Uri.EscapeDataString( message );
        }
    }

    /// <summary>
    /// Stores a titled URL in bookmark-compatible form.
    /// </summary>
    public class Bookmark : Payload
    {
        private readonly string url;

        private readonly string title;

        /// <summary>
        /// Generates a bookmark payload. Scanned by an QR Code reader, this one creates a browser bookmark.
        /// </summary>
        /// <param name="url">Url of the bookmark</param>
        /// <param name="title">Title of the bookmark</param>
        public Bookmark( string url, string title )
        {
            this.url = EscapeInput( url );
            this.title = EscapeInput( title );
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return "MEBKM:TITLE:" + title + ";URL:" + url + ";;";
        }
    }

    /// <summary>
    /// Formats personal and organization details as contact data.
    /// </summary>
    public class ContactData : Payload
    {
        /// <summary>
        /// Possible output types. Either vCard 2.1, vCard 3.0, vCard 4.0 or MeCard.
        /// </summary>
        public enum ContactOutputType
        {
            /// <summary>
            /// Formats the contact as me card.
            /// </summary>
            MeCard,
            /// <summary>
            /// Formats the contact as v card21.
            /// </summary>
            VCard21,
            /// <summary>
            /// Formats the contact as v card3.
            /// </summary>
            VCard3,
            /// <summary>
            /// Formats the contact as v card4.
            /// </summary>
            VCard4
        }

        /// <summary>
        /// define the address format
        /// Default: European format, ([Street] [House Number] and [Postal Code] [City]
        /// Reversed: North American and others format ([House Number] [Street] and [City] [Postal Code])
        /// </summary>
        public enum AddressOrder
        {
            /// <summary>
            /// Writes address fields in default order.
            /// </summary>
            Default,
            /// <summary>
            /// Writes address fields in reversed order.
            /// </summary>
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

        /// <summary>
        /// Generates a vCard or meCard contact dataset
        /// </summary>
        /// <param name="outputType">Payload output type</param>
        /// <param name="firstname">The firstname</param>
        /// <param name="lastname">The lastname</param>
        /// <param name="nickname">The displayname</param>
        /// <param name="phone">Normal phone number</param>
        /// <param name="mobilePhone">Mobile phone</param>
        /// <param name="workPhone">Office phone number</param>
        /// <param name="email">E-Mail address</param>
        /// <param name="birthday">Birthday</param>
        /// <param name="website">Website / Homepage</param>
        /// <param name="street">Street</param>
        /// <param name="houseNumber">Housenumber</param>
        /// <param name="city">City</param>
        /// <param name="stateRegion">State or Region</param>
        /// <param name="zipCode">Zip code</param>
        /// <param name="country">Country</param>
        /// <param name="addressOrder">The address order format to use</param>
        /// <param name="note">Memo text / notes</param>
        /// <param name="org">Organisation/Company</param>
        /// <param name="orgTitle">Organisation/Company Title</param>
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

        /// <inheritdoc />
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

    /// <summary>
    /// Builds payment URIs for Bitcoin-compatible cryptocurrencies.
    /// </summary>
    public class BitcoinLikeCryptoCurrencyAddress : Payload
    {
        /// <summary>
        /// Lists the supported bitcoin like crypto currency type values.
        /// </summary>
        public enum BitcoinLikeCryptoCurrencyType
        {
            /// <summary>
            /// Creates a payment request for bitcoin.
            /// </summary>
            Bitcoin,
            /// <summary>
            /// Creates a payment request for bitcoin cash.
            /// </summary>
            BitcoinCash,
            /// <summary>
            /// Creates a payment request for litecoin.
            /// </summary>
            Litecoin
        }

        private readonly BitcoinLikeCryptoCurrencyType currencyType;

        private readonly string address;

        private readonly string label;

        private readonly string message;

        private readonly double? amount;

        /// <summary>
        /// Generates a Bitcoin like cryptocurrency payment payload. QR Codes with this payload can open a payment app.
        /// </summary>
        /// <param name="currencyName">Bitcoin like cryptocurrency address of the payment receiver</param>
        /// <param name="address">Bitcoin like cryptocurrency address of the payment receiver</param>
        /// <param name="amount">Amount of coins to transfer</param>
        /// <param name="label">Reference label</param>
        /// <param name="message">Referece text aka message</param>
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

        /// <inheritdoc />
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

    /// <summary>
    /// Requests a Bitcoin payment to a wallet address.
    /// </summary>
    public class BitcoinAddress : BitcoinLikeCryptoCurrencyAddress
    {
        /// <summary>
        /// Creates a bitcoin address payload.
        /// </summary>
        public BitcoinAddress( string address, double? amount, string label = null, string message = null )
            : base( BitcoinLikeCryptoCurrencyType.Bitcoin, address, amount, label, message )
        {
        }
    }

    /// <summary>
    /// Requests a Bitcoin Cash payment to a wallet address.
    /// </summary>
    public class BitcoinCashAddress : BitcoinLikeCryptoCurrencyAddress
    {
        /// <summary>
        /// Creates a bitcoin cash address payload.
        /// </summary>
        public BitcoinCashAddress( string address, double? amount, string label = null, string message = null )
            : base( BitcoinLikeCryptoCurrencyType.BitcoinCash, address, amount, label, message )
        {
        }
    }

    /// <summary>
    /// Requests a Litecoin payment to a wallet address.
    /// </summary>
    public class LitecoinAddress : BitcoinLikeCryptoCurrencyAddress
    {
        /// <summary>
        /// Creates a litecoin address payload.
        /// </summary>
        public LitecoinAddress( string address, double? amount, string label = null, string message = null )
            : base( BitcoinLikeCryptoCurrencyType.Litecoin, address, amount, label, message )
        {
        }
    }

    /// <summary>
    /// Produces a Swiss QR invoice payment payload.
    /// </summary>
    public class SwissQrCode : Payload
    {
        /// <summary>
        /// Carries optional messages and billing details for a Swiss payment.
        /// </summary>
        public class AdditionalInformation
        {
            /// <summary>
            /// Reports errors encountered while processing swiss qr code additional information.
            /// </summary>
            public class SwissQrCodeAdditionalInformationException : Exception
            {
                /// <summary>
                /// Creates an exception for swiss qr code additional information failures.
                /// </summary>
                public SwissQrCodeAdditionalInformationException()
                {
                }

                /// <summary>
                /// Creates an exception for swiss qr code additional information failures.
                /// </summary>
                public SwissQrCodeAdditionalInformationException( string message )
                    : base( message )
                {
                }

                /// <summary>
                /// Creates an exception for swiss qr code additional information failures.
                /// </summary>
                public SwissQrCodeAdditionalInformationException( string message, Exception inner )
                    : base( message, inner )
                {
                }
            }

            private readonly string unstructuredMessage;

            private readonly string billInformation;

            private readonly string trailer;

            /// <summary>
            /// Unstructure Message consumed by the additional information.
            /// </summary>
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

            /// <summary>
            /// Bill Information consumed by the additional information.
            /// </summary>
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

            /// <summary>
            /// Trailer used by the additional information.
            /// </summary>
            public string Trailer => trailer;

            /// <summary>
            /// Creates an additional information object. Both parameters are optional and must be shorter than 141 chars in combination.
            /// </summary>
            /// <param name="unstructuredMessage">Unstructured text message</param>
            /// <param name="billInformation">Bill information</param>
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

        /// <summary>
        /// Validates and formats a Swiss payment reference.
        /// </summary>
        public class Reference
        {
            /// <summary>
            /// Reference type. When using a QR-IBAN you have to use either "QRR" or "SCOR"
            /// </summary>
            public enum ReferenceType
            {
                /// <summary>
                /// Uses the qrr payment-reference scheme.
                /// </summary>
                QRR,
                /// <summary>
                /// Uses the scor payment-reference scheme.
                /// </summary>
                SCOR,
                /// <summary>
                /// Uses the non payment-reference scheme.
                /// </summary>
                NON
            }

            /// <summary>
            /// Lists the supported reference text type values.
            /// </summary>
            public enum ReferenceTextType
            {
                /// <summary>
                /// Validates references using the qr reference format.
                /// </summary>
                QrReference,
                /// <summary>
                /// Validates references using the creditor reference iso11649 format.
                /// </summary>
                CreditorReferenceIso11649
            }

            /// <summary>
            /// Reports errors encountered while processing swiss qr code reference.
            /// </summary>
            public class SwissQrCodeReferenceException : Exception
            {
                /// <summary>
                /// Creates an exception for swiss qr code reference failures.
                /// </summary>
                public SwissQrCodeReferenceException()
                {
                }

                /// <summary>
                /// Creates an exception for swiss qr code reference failures.
                /// </summary>
                public SwissQrCodeReferenceException( string message )
                    : base( message )
                {
                }

                /// <summary>
                /// Creates an exception for swiss qr code reference failures.
                /// </summary>
                public SwissQrCodeReferenceException( string message, Exception inner )
                    : base( message, inner )
                {
                }
            }

            private readonly ReferenceType referenceType;

            private readonly string reference;

            private readonly ReferenceTextType? referenceTextType;

            /// <summary>
            /// Ref Type controlling how the reference behaves.
            /// </summary>
            public ReferenceType RefType => referenceType;

            /// <summary>
            /// Reference Text consumed by the reference.
            /// </summary>
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

            /// <summary>
            /// Creates a reference object which must be passed to the SwissQrCode instance
            /// </summary>
            /// <param name="referenceType">Type of the reference (QRR, SCOR or NON)</param>
            /// <param name="reference">Reference text</param>
            /// <param name="referenceTextType">Type of the reference text (QR-reference or Creditor Reference)</param>
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

        /// <summary>
        /// Validates an IBAN used by a Swiss payment.
        /// </summary>
        public class Iban
        {
            /// <summary>
            /// Lists the supported iban type values.
            /// </summary>
            public enum IbanType
            {
                /// <summary>
                /// Treats the account number as iban.
                /// </summary>
                Iban,
                /// <summary>
                /// Treats the account number as qr iban.
                /// </summary>
                QrIban
            }

            /// <summary>
            /// Reports errors encountered while processing swiss qr code iban.
            /// </summary>
            public class SwissQrCodeIbanException : Exception
            {
                /// <summary>
                /// Creates an exception for swiss qr code iban failures.
                /// </summary>
                public SwissQrCodeIbanException()
                {
                }

                /// <summary>
                /// Creates an exception for swiss qr code iban failures.
                /// </summary>
                public SwissQrCodeIbanException( string message )
                    : base( message )
                {
                }

                /// <summary>
                /// Creates an exception for swiss qr code iban failures.
                /// </summary>
                public SwissQrCodeIbanException( string message, Exception inner )
                    : base( message, inner )
                {
                }
            }

            private string iban;

            private IbanType ibanType;

            /// <summary>
            /// Indicates whether the iban is qr iban.
            /// </summary>
            public bool IsQrIban => ibanType == IbanType.QrIban;

            /// <summary>
            /// IBAN object with type information
            /// </summary>
            /// <param name="iban">IBAN</param>
            /// <param name="ibanType">Type of IBAN (normal or QR-IBAN)</param>
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

            /// <inheritdoc />
            public override string ToString()
            {
                return iban.Replace( "-", "" ).Replace( "\n", "" ).Replace( " ", "" );
            }
        }

        /// <summary>
        /// Describes a creditor or debtor address on a Swiss invoice.
        /// </summary>
        public class Contact
        {
            /// <summary>
            /// Lists the supported address type values.
            /// </summary>
            public enum AddressType
            {
                /// <summary>
                /// Uses a structured address postal address.
                /// </summary>
                StructuredAddress,
                /// <summary>
                /// Uses a combined address postal address.
                /// </summary>
                CombinedAddress
            }

            /// <summary>
            /// Reports errors encountered while processing swiss qr code contact.
            /// </summary>
            public class SwissQrCodeContactException : Exception
            {
                /// <summary>
                /// Creates an exception for swiss qr code contact failures.
                /// </summary>
                public SwissQrCodeContactException()
                {
                }

                /// <summary>
                /// Creates an exception for swiss qr code contact failures.
                /// </summary>
                public SwissQrCodeContactException( string message )
                    : base( message )
                {
                }

                /// <summary>
                /// Creates an exception for swiss qr code contact failures.
                /// </summary>
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

            /// <summary>
            /// Creates a contact with separate street, postal code, and city fields.
            /// </summary>
            public static Contact WithStructuredAddress( string name, string zipCode, string city, string country, string street = null, string houseNumber = null )
            {
                return new Contact( name, zipCode, city, country, street, houseNumber, AddressType.StructuredAddress );
            }

            /// <summary>
            /// Creates a contact from two combined address lines.
            /// </summary>
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

            /// <inheritdoc />
            public override string ToString()
            {
                return string.Concat( string.Concat( string.Concat( string.Concat( string.Concat( string.Concat( ( ( adrType == AddressType.StructuredAddress ) ? "S" : "K" ) + br, name.Replace( "\n", "" ), br ), ( !string.IsNullOrEmpty( streetOrAddressline1 ) ) ? streetOrAddressline1.Replace( "\n", "" ) : string.Empty, br ), ( !string.IsNullOrEmpty( houseNumberOrAddressline2 ) ) ? houseNumberOrAddressline2.Replace( "\n", "" ) : string.Empty, br ), zipCode.Replace( "\n", "" ), br ), city.Replace( "\n", "" ), br ), country, br );
            }
        }

        /// <summary>
        /// Lists the supported currency values.
        /// </summary>
        public enum Currency
        {
            /// <summary>
            /// Uses chf as the payment currency.
            /// </summary>
            CHF = 756,
            /// <summary>
            /// Uses eur as the payment currency.
            /// </summary>
            EUR = 978
        }

        /// <summary>
        /// Reports errors encountered while processing swiss qr code.
        /// </summary>
        public class SwissQrCodeException : Exception
        {
            /// <summary>
            /// Creates an exception for swiss qr code failures.
            /// </summary>
            public SwissQrCodeException()
            {
            }

            /// <summary>
            /// Creates an exception for swiss qr code failures.
            /// </summary>
            public SwissQrCodeException( string message )
                : base( message )
            {
            }

            /// <summary>
            /// Creates an exception for swiss qr code failures.
            /// </summary>
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

        /// <summary>
        /// Generates the payload for a SwissQrCode v2.0. (Don't forget to use ECC-Level=M, EncodingMode=UTF-8 and to set the Swiss flag icon to the final QR code.)
        /// </summary>
        /// <param name="iban">IBAN object</param>
        /// <param name="currency">Currency (either EUR or CHF)</param>
        /// <param name="creditor">Creditor (payee) information</param>
        /// <param name="reference">Reference information</param>
        /// <param name="debitor">Debitor (payer) information</param>
        /// <param name="amount">Amount</param>
        /// <param name="requestedDateOfPayment">Requested date of debitor's payment</param>
        /// <param name="ultimateCreditor">Ultimate creditor information (use only in consultation with your bank - for future use only!)</param>
        /// <param name="alternativeProcedure1">Optional command for alternative processing mode - line 1</param>
        /// <param name="alternativeProcedure2">Optional command for alternative processing mode - line 2</param>
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

        /// <inheritdoc />
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

    /// <summary>
    /// Produces a SEPA credit-transfer Girocode payload.
    /// </summary>
    public class Girocode : Payload
    {
        /// <summary>
        /// Lists the supported girocode version values.
        /// </summary>
        public enum GirocodeVersion
        {
            /// <summary>
            /// Emits version1 of the Girocode format.
            /// </summary>
            Version1,
            /// <summary>
            /// Emits version2 of the Girocode format.
            /// </summary>
            Version2
        }

        /// <summary>
        /// Lists the supported type of remittance values.
        /// </summary>
        public enum TypeOfRemittance
        {
            /// <summary>
            /// Uses structured remittance information.
            /// </summary>
            Structured,
            /// <summary>
            /// Uses unstructured remittance information.
            /// </summary>
            Unstructured
        }

        /// <summary>
        /// Lists the supported girocode encoding values.
        /// </summary>
        public enum GirocodeEncoding
        {
            /// <summary>
            /// Encodes the payload using utf_8.
            /// </summary>
            UTF_8,
            /// <summary>
            /// Encodes the payload using iso_8859_1.
            /// </summary>
            ISO_8859_1,
            /// <summary>
            /// Encodes the payload using iso_8859_2.
            /// </summary>
            ISO_8859_2,
            /// <summary>
            /// Encodes the payload using iso_8859_4.
            /// </summary>
            ISO_8859_4,
            /// <summary>
            /// Encodes the payload using iso_8859_5.
            /// </summary>
            ISO_8859_5,
            /// <summary>
            /// Encodes the payload using iso_8859_7.
            /// </summary>
            ISO_8859_7,
            /// <summary>
            /// Encodes the payload using iso_8859_10.
            /// </summary>
            ISO_8859_10,
            /// <summary>
            /// Encodes the payload using iso_8859_15.
            /// </summary>
            ISO_8859_15
        }

        /// <summary>
        /// Reports errors encountered while processing girocode.
        /// </summary>
        public class GirocodeException : Exception
        {
            /// <summary>
            /// Creates an exception for girocode failures.
            /// </summary>
            public GirocodeException()
            {
            }

            /// <summary>
            /// Creates an exception for girocode failures.
            /// </summary>
            public GirocodeException( string message )
                : base( message )
            {
            }

            /// <summary>
            /// Creates an exception for girocode failures.
            /// </summary>
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

        /// <summary>
        /// Generates the payload for a Girocode (QR-Code with credit transfer information).
        /// Attention: When using Girocode payload, QR code must be generated with ECC level M!
        /// </summary>
        /// <param name="iban">Account number of the Beneficiary. Only IBAN is allowed.</param>
        /// <param name="bic">BIC of the Beneficiary Bank.</param>
        /// <param name="name">Name of the Beneficiary.</param>
        /// <param name="amount">Amount of the Credit Transfer in Euro.
        /// (Amount must be more than 0.01 and less than 999999999.99)</param>
        /// <param name="remittanceInformation">Remittance Information (Purpose-/reference text). (optional)</param>
        /// <param name="typeOfRemittance">Type of remittance information. Either structured (e.g. ISO 11649 RF Creditor Reference) and max. 35 chars or unstructured and max. 140 chars.</param>
        /// <param name="purposeOfCreditTransfer">Purpose of the Credit Transfer (optional)</param>
        /// <param name="messageToGirocodeUser">Beneficiary to originator information. (optional)</param>
        /// <param name="version">Girocode version. Either 001 or 002. Default: 001.</param>
        /// <param name="encoding">Encoding of the Girocode payload. Default: ISO-8859-1</param>
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

        /// <inheritdoc />
        public override string ToString()
        {
            return ConvertStringToEncoding( string.Concat( string.Concat( string.Concat( string.Concat( string.Concat( string.Concat( string.Concat( string.Concat( string.Concat( string.Concat( string.Concat( "BCD" + br, ( version == GirocodeVersion.Version1 ) ? "001" : "002", br ), ( (int)( encoding + 1 ) ).ToString(), br ), "SCT", br ), bic, br ), name, br ), iban, br ), $"EUR{amount:0.00}".Replace( ",", "." ), br ), purposeOfCreditTransfer, br ), ( typeOfRemittance == TypeOfRemittance.Structured ) ? remittanceInformation : string.Empty, br ), ( typeOfRemittance == TypeOfRemittance.Unstructured ) ? remittanceInformation : string.Empty, br ), messageToGirocodeUser ), encoding.ToString().Replace( "_", "-" ) );
        }
    }

    /// <summary>
    /// Encodes German BezahlCode payment and banking operations.
    /// </summary>
    public class BezahlCode : Payload
    {
        /// <summary>
        /// Lists the supported currency values.
        /// </summary>
        public enum Currency
        {
            /// <summary>
            /// Uses aed as the payment currency.
            /// </summary>
            AED = 784,
            /// <summary>
            /// Uses afn as the payment currency.
            /// </summary>
            AFN = 971,
            /// <summary>
            /// Uses all as the payment currency.
            /// </summary>
            ALL = 8,
            /// <summary>
            /// Uses amd as the payment currency.
            /// </summary>
            AMD = 51,
            /// <summary>
            /// Uses ang as the payment currency.
            /// </summary>
            ANG = 532,
            /// <summary>
            /// Uses aoa as the payment currency.
            /// </summary>
            AOA = 973,
            /// <summary>
            /// Uses ars as the payment currency.
            /// </summary>
            ARS = 0x20,
            /// <summary>
            /// Uses aud as the payment currency.
            /// </summary>
            AUD = 36,
            /// <summary>
            /// Uses awg as the payment currency.
            /// </summary>
            AWG = 533,
            /// <summary>
            /// Uses azn as the payment currency.
            /// </summary>
            AZN = 944,
            /// <summary>
            /// Uses bam as the payment currency.
            /// </summary>
            BAM = 977,
            /// <summary>
            /// Uses bbd as the payment currency.
            /// </summary>
            BBD = 52,
            /// <summary>
            /// Uses bdt as the payment currency.
            /// </summary>
            BDT = 50,
            /// <summary>
            /// Uses bgn as the payment currency.
            /// </summary>
            BGN = 975,
            /// <summary>
            /// Uses bhd as the payment currency.
            /// </summary>
            BHD = 48,
            /// <summary>
            /// Uses bif as the payment currency.
            /// </summary>
            BIF = 108,
            /// <summary>
            /// Uses bmd as the payment currency.
            /// </summary>
            BMD = 60,
            /// <summary>
            /// Uses bnd as the payment currency.
            /// </summary>
            BND = 96,
            /// <summary>
            /// Uses bob as the payment currency.
            /// </summary>
            BOB = 68,
            /// <summary>
            /// Uses bov as the payment currency.
            /// </summary>
            BOV = 984,
            /// <summary>
            /// Uses brl as the payment currency.
            /// </summary>
            BRL = 986,
            /// <summary>
            /// Uses bsd as the payment currency.
            /// </summary>
            BSD = 44,
            /// <summary>
            /// Uses btn as the payment currency.
            /// </summary>
            BTN = 0x40,
            /// <summary>
            /// Uses bwp as the payment currency.
            /// </summary>
            BWP = 72,
            /// <summary>
            /// Uses byr as the payment currency.
            /// </summary>
            BYR = 974,
            /// <summary>
            /// Uses bzd as the payment currency.
            /// </summary>
            BZD = 84,
            /// <summary>
            /// Uses cad as the payment currency.
            /// </summary>
            CAD = 124,
            /// <summary>
            /// Uses cdf as the payment currency.
            /// </summary>
            CDF = 976,
            /// <summary>
            /// Uses che as the payment currency.
            /// </summary>
            CHE = 947,
            /// <summary>
            /// Uses chf as the payment currency.
            /// </summary>
            CHF = 756,
            /// <summary>
            /// Uses chw as the payment currency.
            /// </summary>
            CHW = 948,
            /// <summary>
            /// Uses clf as the payment currency.
            /// </summary>
            CLF = 990,
            /// <summary>
            /// Uses clp as the payment currency.
            /// </summary>
            CLP = 152,
            /// <summary>
            /// Uses cny as the payment currency.
            /// </summary>
            CNY = 156,
            /// <summary>
            /// Uses cop as the payment currency.
            /// </summary>
            COP = 170,
            /// <summary>
            /// Uses cou as the payment currency.
            /// </summary>
            COU = 970,
            /// <summary>
            /// Uses crc as the payment currency.
            /// </summary>
            CRC = 188,
            /// <summary>
            /// Uses cuc as the payment currency.
            /// </summary>
            CUC = 931,
            /// <summary>
            /// Uses cup as the payment currency.
            /// </summary>
            CUP = 192,
            /// <summary>
            /// Uses cve as the payment currency.
            /// </summary>
            CVE = 132,
            /// <summary>
            /// Uses czk as the payment currency.
            /// </summary>
            CZK = 203,
            /// <summary>
            /// Uses djf as the payment currency.
            /// </summary>
            DJF = 262,
            /// <summary>
            /// Uses dkk as the payment currency.
            /// </summary>
            DKK = 208,
            /// <summary>
            /// Uses dop as the payment currency.
            /// </summary>
            DOP = 214,
            /// <summary>
            /// Uses dzd as the payment currency.
            /// </summary>
            DZD = 12,
            /// <summary>
            /// Uses egp as the payment currency.
            /// </summary>
            EGP = 818,
            /// <summary>
            /// Uses ern as the payment currency.
            /// </summary>
            ERN = 232,
            /// <summary>
            /// Uses etb as the payment currency.
            /// </summary>
            ETB = 230,
            /// <summary>
            /// Uses eur as the payment currency.
            /// </summary>
            EUR = 978,
            /// <summary>
            /// Uses fjd as the payment currency.
            /// </summary>
            FJD = 242,
            /// <summary>
            /// Uses fkp as the payment currency.
            /// </summary>
            FKP = 238,
            /// <summary>
            /// Uses gbp as the payment currency.
            /// </summary>
            GBP = 826,
            /// <summary>
            /// Uses gel as the payment currency.
            /// </summary>
            GEL = 981,
            /// <summary>
            /// Uses ghs as the payment currency.
            /// </summary>
            GHS = 936,
            /// <summary>
            /// Uses gip as the payment currency.
            /// </summary>
            GIP = 292,
            /// <summary>
            /// Uses gmd as the payment currency.
            /// </summary>
            GMD = 270,
            /// <summary>
            /// Uses gnf as the payment currency.
            /// </summary>
            GNF = 324,
            /// <summary>
            /// Uses gtq as the payment currency.
            /// </summary>
            GTQ = 320,
            /// <summary>
            /// Uses gyd as the payment currency.
            /// </summary>
            GYD = 328,
            /// <summary>
            /// Uses hkd as the payment currency.
            /// </summary>
            HKD = 344,
            /// <summary>
            /// Uses hnl as the payment currency.
            /// </summary>
            HNL = 340,
            /// <summary>
            /// Uses hrk as the payment currency.
            /// </summary>
            HRK = 191,
            /// <summary>
            /// Uses htg as the payment currency.
            /// </summary>
            HTG = 332,
            /// <summary>
            /// Uses huf as the payment currency.
            /// </summary>
            HUF = 348,
            /// <summary>
            /// Uses idr as the payment currency.
            /// </summary>
            IDR = 360,
            /// <summary>
            /// Uses ils as the payment currency.
            /// </summary>
            ILS = 376,
            /// <summary>
            /// Uses inr as the payment currency.
            /// </summary>
            INR = 356,
            /// <summary>
            /// Uses iqd as the payment currency.
            /// </summary>
            IQD = 368,
            /// <summary>
            /// Uses irr as the payment currency.
            /// </summary>
            IRR = 364,
            /// <summary>
            /// Uses isk as the payment currency.
            /// </summary>
            ISK = 352,
            /// <summary>
            /// Uses jmd as the payment currency.
            /// </summary>
            JMD = 388,
            /// <summary>
            /// Uses jod as the payment currency.
            /// </summary>
            JOD = 400,
            /// <summary>
            /// Uses jpy as the payment currency.
            /// </summary>
            JPY = 392,
            /// <summary>
            /// Uses kes as the payment currency.
            /// </summary>
            KES = 404,
            /// <summary>
            /// Uses kgs as the payment currency.
            /// </summary>
            KGS = 417,
            /// <summary>
            /// Uses khr as the payment currency.
            /// </summary>
            KHR = 116,
            /// <summary>
            /// Uses kmf as the payment currency.
            /// </summary>
            KMF = 174,
            /// <summary>
            /// Uses kpw as the payment currency.
            /// </summary>
            KPW = 408,
            /// <summary>
            /// Uses krw as the payment currency.
            /// </summary>
            KRW = 410,
            /// <summary>
            /// Uses kwd as the payment currency.
            /// </summary>
            KWD = 414,
            /// <summary>
            /// Uses kyd as the payment currency.
            /// </summary>
            KYD = 136,
            /// <summary>
            /// Uses kzt as the payment currency.
            /// </summary>
            KZT = 398,
            /// <summary>
            /// Uses lak as the payment currency.
            /// </summary>
            LAK = 418,
            /// <summary>
            /// Uses lbp as the payment currency.
            /// </summary>
            LBP = 422,
            /// <summary>
            /// Uses lkr as the payment currency.
            /// </summary>
            LKR = 144,
            /// <summary>
            /// Uses lrd as the payment currency.
            /// </summary>
            LRD = 430,
            /// <summary>
            /// Uses lsl as the payment currency.
            /// </summary>
            LSL = 426,
            /// <summary>
            /// Uses lyd as the payment currency.
            /// </summary>
            LYD = 434,
            /// <summary>
            /// Uses mad as the payment currency.
            /// </summary>
            MAD = 504,
            /// <summary>
            /// Uses mdl as the payment currency.
            /// </summary>
            MDL = 498,
            /// <summary>
            /// Uses mga as the payment currency.
            /// </summary>
            MGA = 969,
            /// <summary>
            /// Uses mkd as the payment currency.
            /// </summary>
            MKD = 807,
            /// <summary>
            /// Uses mmk as the payment currency.
            /// </summary>
            MMK = 104,
            /// <summary>
            /// Uses mnt as the payment currency.
            /// </summary>
            MNT = 496,
            /// <summary>
            /// Uses mop as the payment currency.
            /// </summary>
            MOP = 446,
            /// <summary>
            /// Uses mro as the payment currency.
            /// </summary>
            MRO = 478,
            /// <summary>
            /// Uses mur as the payment currency.
            /// </summary>
            MUR = 480,
            /// <summary>
            /// Uses mvr as the payment currency.
            /// </summary>
            MVR = 462,
            /// <summary>
            /// Uses mwk as the payment currency.
            /// </summary>
            MWK = 454,
            /// <summary>
            /// Uses mxn as the payment currency.
            /// </summary>
            MXN = 484,
            /// <summary>
            /// Uses mxv as the payment currency.
            /// </summary>
            MXV = 979,
            /// <summary>
            /// Uses myr as the payment currency.
            /// </summary>
            MYR = 458,
            /// <summary>
            /// Uses mzn as the payment currency.
            /// </summary>
            MZN = 943,
            /// <summary>
            /// Uses nad as the payment currency.
            /// </summary>
            NAD = 516,
            /// <summary>
            /// Uses ngn as the payment currency.
            /// </summary>
            NGN = 566,
            /// <summary>
            /// Uses nio as the payment currency.
            /// </summary>
            NIO = 558,
            /// <summary>
            /// Uses nok as the payment currency.
            /// </summary>
            NOK = 578,
            /// <summary>
            /// Uses npr as the payment currency.
            /// </summary>
            NPR = 524,
            /// <summary>
            /// Uses nzd as the payment currency.
            /// </summary>
            NZD = 554,
            /// <summary>
            /// Uses omr as the payment currency.
            /// </summary>
            OMR = 0x200,
            /// <summary>
            /// Uses pab as the payment currency.
            /// </summary>
            PAB = 590,
            /// <summary>
            /// Uses pen as the payment currency.
            /// </summary>
            PEN = 604,
            /// <summary>
            /// Uses pgk as the payment currency.
            /// </summary>
            PGK = 598,
            /// <summary>
            /// Uses php as the payment currency.
            /// </summary>
            PHP = 608,
            /// <summary>
            /// Uses pkr as the payment currency.
            /// </summary>
            PKR = 586,
            /// <summary>
            /// Uses pln as the payment currency.
            /// </summary>
            PLN = 985,
            /// <summary>
            /// Uses pyg as the payment currency.
            /// </summary>
            PYG = 600,
            /// <summary>
            /// Uses qar as the payment currency.
            /// </summary>
            QAR = 634,
            /// <summary>
            /// Uses ron as the payment currency.
            /// </summary>
            RON = 946,
            /// <summary>
            /// Uses rsd as the payment currency.
            /// </summary>
            RSD = 941,
            /// <summary>
            /// Uses rub as the payment currency.
            /// </summary>
            RUB = 643,
            /// <summary>
            /// Uses rwf as the payment currency.
            /// </summary>
            RWF = 646,
            /// <summary>
            /// Uses sar as the payment currency.
            /// </summary>
            SAR = 682,
            /// <summary>
            /// Uses sbd as the payment currency.
            /// </summary>
            SBD = 90,
            /// <summary>
            /// Uses scr as the payment currency.
            /// </summary>
            SCR = 690,
            /// <summary>
            /// Uses sdg as the payment currency.
            /// </summary>
            SDG = 938,
            /// <summary>
            /// Uses sek as the payment currency.
            /// </summary>
            SEK = 752,
            /// <summary>
            /// Uses sgd as the payment currency.
            /// </summary>
            SGD = 702,
            /// <summary>
            /// Uses shp as the payment currency.
            /// </summary>
            SHP = 654,
            /// <summary>
            /// Uses sll as the payment currency.
            /// </summary>
            SLL = 694,
            /// <summary>
            /// Uses sos as the payment currency.
            /// </summary>
            SOS = 706,
            /// <summary>
            /// Uses srd as the payment currency.
            /// </summary>
            SRD = 968,
            /// <summary>
            /// Uses ssp as the payment currency.
            /// </summary>
            SSP = 728,
            /// <summary>
            /// Uses std as the payment currency.
            /// </summary>
            STD = 678,
            /// <summary>
            /// Uses svc as the payment currency.
            /// </summary>
            SVC = 222,
            /// <summary>
            /// Uses syp as the payment currency.
            /// </summary>
            SYP = 760,
            /// <summary>
            /// Uses szl as the payment currency.
            /// </summary>
            SZL = 748,
            /// <summary>
            /// Uses thb as the payment currency.
            /// </summary>
            THB = 764,
            /// <summary>
            /// Uses tjs as the payment currency.
            /// </summary>
            TJS = 972,
            /// <summary>
            /// Uses tmt as the payment currency.
            /// </summary>
            TMT = 934,
            /// <summary>
            /// Uses tnd as the payment currency.
            /// </summary>
            TND = 788,
            /// <summary>
            /// Uses top as the payment currency.
            /// </summary>
            TOP = 776,
            /// <summary>
            /// Uses try as the payment currency.
            /// </summary>
            TRY = 949,
            /// <summary>
            /// Uses ttd as the payment currency.
            /// </summary>
            TTD = 780,
            /// <summary>
            /// Uses twd as the payment currency.
            /// </summary>
            TWD = 901,
            /// <summary>
            /// Uses tzs as the payment currency.
            /// </summary>
            TZS = 834,
            /// <summary>
            /// Uses uah as the payment currency.
            /// </summary>
            UAH = 980,
            /// <summary>
            /// Uses ugx as the payment currency.
            /// </summary>
            UGX = 800,
            /// <summary>
            /// Uses usd as the payment currency.
            /// </summary>
            USD = 840,
            /// <summary>
            /// Uses usn as the payment currency.
            /// </summary>
            USN = 997,
            /// <summary>
            /// Uses uyi as the payment currency.
            /// </summary>
            UYI = 940,
            /// <summary>
            /// Uses uyu as the payment currency.
            /// </summary>
            UYU = 858,
            /// <summary>
            /// Uses uzs as the payment currency.
            /// </summary>
            UZS = 860,
            /// <summary>
            /// Uses vef as the payment currency.
            /// </summary>
            VEF = 937,
            /// <summary>
            /// Uses vnd as the payment currency.
            /// </summary>
            VND = 704,
            /// <summary>
            /// Uses vuv as the payment currency.
            /// </summary>
            VUV = 548,
            /// <summary>
            /// Uses wst as the payment currency.
            /// </summary>
            WST = 882,
            /// <summary>
            /// Uses xaf as the payment currency.
            /// </summary>
            XAF = 950,
            /// <summary>
            /// Uses xag as the payment currency.
            /// </summary>
            XAG = 961,
            /// <summary>
            /// Uses xau as the payment currency.
            /// </summary>
            XAU = 959,
            /// <summary>
            /// Uses xba as the payment currency.
            /// </summary>
            XBA = 955,
            /// <summary>
            /// Uses xbb as the payment currency.
            /// </summary>
            XBB = 956,
            /// <summary>
            /// Uses xbc as the payment currency.
            /// </summary>
            XBC = 957,
            /// <summary>
            /// Uses xbd as the payment currency.
            /// </summary>
            XBD = 958,
            /// <summary>
            /// Uses xcd as the payment currency.
            /// </summary>
            XCD = 951,
            /// <summary>
            /// Uses xdr as the payment currency.
            /// </summary>
            XDR = 960,
            /// <summary>
            /// Uses xof as the payment currency.
            /// </summary>
            XOF = 952,
            /// <summary>
            /// Uses xpd as the payment currency.
            /// </summary>
            XPD = 964,
            /// <summary>
            /// Uses xpf as the payment currency.
            /// </summary>
            XPF = 953,
            /// <summary>
            /// Uses xpt as the payment currency.
            /// </summary>
            XPT = 962,
            /// <summary>
            /// Uses xsu as the payment currency.
            /// </summary>
            XSU = 994,
            /// <summary>
            /// Uses xts as the payment currency.
            /// </summary>
            XTS = 963,
            /// <summary>
            /// Uses xua as the payment currency.
            /// </summary>
            XUA = 965,
            /// <summary>
            /// Uses xxx as the payment currency.
            /// </summary>
            XXX = 999,
            /// <summary>
            /// Uses yer as the payment currency.
            /// </summary>
            YER = 886,
            /// <summary>
            /// Uses zar as the payment currency.
            /// </summary>
            ZAR = 710,
            /// <summary>
            /// Uses zmw as the payment currency.
            /// </summary>
            ZMW = 967,
            /// <summary>
            /// Uses zwl as the payment currency.
            /// </summary>
            ZWL = 932
        }

        /// <summary>
        /// Operation modes of the BezahlCode
        /// </summary>
        public enum AuthorityType
        {
            /// <summary>
            /// Single SEPA payment (SEPA-Überweisung)
            /// </summary>
            singlepaymentsepa,
            /// <summary>
            /// Single SEPA debit (SEPA-Lastschrift)
            /// </summary>
            singledirectdebitsepa,
            /// <summary>
            /// Periodic SEPA payment (SEPA-Dauerauftrag)
            /// </summary>
            periodicsinglepaymentsepa,
            /// <summary>
            /// Contact data
            /// </summary>
            contact,
            /// <summary>
            /// Contact data V2
            /// </summary>
            contact_v2
        }

        /// <summary>
        /// Reports errors encountered while processing bezahl code.
        /// </summary>
        public class BezahlCodeException : Exception
        {
            /// <summary>
            /// Creates an exception for bezahl code failures.
            /// </summary>
            public BezahlCodeException()
            {
            }

            /// <summary>
            /// Creates an exception for bezahl code failures.
            /// </summary>
            public BezahlCodeException( string message )
                : base( message )
            {
            }

            /// <summary>
            /// Creates an exception for bezahl code failures.
            /// </summary>
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

        private readonly int periodicTimeunitRotation;

        private readonly Currency currency;

        private readonly AuthorityType authority;

        private readonly DateTime executionDate;

        private readonly DateTime dateOfSignature;

        private readonly DateTime periodicFirstExecutionDate;

        private readonly DateTime periodicLastExecutionDate;

        /// <summary>
        /// Creates a bezahl code payload.
        /// </summary>
        public BezahlCode( AuthorityType authority, string name, string account = "", string bnc = "", string iban = "", string bic = "", string reason = "" )
            : this( authority, name, account, bnc, iban, bic, 0m, string.Empty, 0, null, null, string.Empty, string.Empty, null, reason, string.Empty, Currency.EUR, null, 1 )
        {
        }

        /// <summary>
        /// Creates a bezahl code payload.
        /// </summary>
        public BezahlCode( AuthorityType authority, string name, string iban, string bic, decimal amount, string periodicTimeunit = "", int periodicTimeunitRotation = 0, DateTime? periodicFirstExecutionDate = null, DateTime? periodicLastExecutionDate = null, string creditorId = "", string mandateId = "", DateTime? dateOfSignature = null, string reason = "", string sepaReference = "", Currency currency = Currency.EUR, DateTime? executionDate = null )
            : this( authority, name, string.Empty, string.Empty, iban, bic, amount, periodicTimeunit, periodicTimeunitRotation, periodicFirstExecutionDate, periodicLastExecutionDate, creditorId, mandateId, dateOfSignature, reason, sepaReference, currency, executionDate, 3 )
        {
        }

        /// <summary>
        /// Creates a bezahl code payload.
        /// </summary>
        public BezahlCode( AuthorityType authority, string name, string account, string bnc, string iban, string bic, decimal amount, string periodicTimeunit = "", int periodicTimeunitRotation = 0, DateTime? periodicFirstExecutionDate = null, DateTime? periodicLastExecutionDate = null, string creditorId = "", string mandateId = "", DateTime? dateOfSignature = null, string reason = "", string sepaReference = "", Currency currency = Currency.EUR, DateTime? executionDate = null, int internalMode = 0 )
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
            if ( authority == AuthorityType.contact || ( authority == AuthorityType.contact_v2 && flag3 ) )
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

            if ( authority == AuthorityType.periodicsinglepaymentsepa )
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

        /// <inheritdoc />
        public override string ToString()
        {
            string text = $"bank://{authority}?";
            text = text + "name=" + Uri.EscapeDataString( name ) + "&";
            if ( authority != AuthorityType.contact && authority != AuthorityType.contact_v2 )
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

                text += $"amount={amount:0.00}&".Replace( ".", "," );
                if ( !string.IsNullOrEmpty( reason ) )
                {
                    text = text + "reason=" + Uri.EscapeDataString( reason ) + "&";
                }

                text += $"currency={currency}&";
                text = text + "executiondate=" + executionDate.ToString( "ddMMyyyy" ) + "&";
                if ( authority == AuthorityType.periodicsinglepaymentsepa )
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

    /// <summary>
    /// Creates an iCalendar event that can be imported after scanning.
    /// </summary>
    public class CalendarEvent : Payload
    {
        /// <summary>
        /// Lists the supported event encoding values.
        /// </summary>
        public enum EventEncoding
        {
            /// <summary>
            /// Encodes the payload using i cal complete.
            /// </summary>
            iCalComplete,
            /// <summary>
            /// Encodes the payload using universal.
            /// </summary>
            Universal
        }

        private readonly string subject;

        private readonly string description;

        private readonly string location;

        private readonly string start;

        private readonly string end;

        private readonly EventEncoding encoding;

        /// <summary>
        /// Generates a calender entry/event payload.
        /// </summary>
        /// <param name="subject">Subject/title of the calender event</param>
        /// <param name="description">Description of the event</param>
        /// <param name="location">Location (lat:long or address) of the event</param>
        /// <param name="start">Start time of the event</param>
        /// <param name="end">End time of the event</param>
        /// <param name="allDayEvent">Is it a full day event?</param>
        /// <param name="encoding">Type of encoding (universal or iCal)</param>
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

        /// <inheritdoc />
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

    /// <summary>
    /// Configures a TOTP or HOTP authenticator account.
    /// </summary>
    public class OneTimePassword : Payload
    {
        /// <summary>
        /// Lists the supported one time password auth type values.
        /// </summary>
        public enum OneTimePasswordAuthType
        {
            /// <summary>
            /// Generates totp one-time passwords.
            /// </summary>
            TOTP,
            /// <summary>
            /// Generates hotp one-time passwords.
            /// </summary>
            HOTP
        }

        /// <summary>
        /// Lists the supported one time password auth algorithm values.
        /// </summary>
        public enum OneTimePasswordAuthAlgorithm
        {
            /// <summary>
            /// Signs one-time passwords with sha1.
            /// </summary>
            SHA1,
            /// <summary>
            /// Signs one-time passwords with sha256.
            /// </summary>
            SHA256,
            /// <summary>
            /// Signs one-time passwords with sha512.
            /// </summary>
            SHA512
        }

        /// <summary>
        /// Type controlling how the one time password behaves.
        /// </summary>
        public OneTimePasswordAuthType Type { get; set; }

        /// <summary>
        /// Secret used by the one time password.
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// Auth Algorithm controlling how the one time password behaves.
        /// </summary>
        public OneTimePasswordAuthAlgorithm AuthAlgorithm { get; set; } = OneTimePasswordAuthAlgorithm.SHA1;

        /// <summary>
        /// Issuer used by the one time password.
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// Label displayed for the rendered value.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Digits used by the one time password.
        /// </summary>
        public int Digits { get; set; } = 6;


        /// <summary>
        /// Counter used by the one time password.
        /// </summary>
        public int? Counter { get; set; }

        /// <summary>
        /// Period used by the one time password.
        /// </summary>
        public int? Period { get; set; } = 30;


        /// <inheritdoc />
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

    /// <summary>
    /// Encodes a Shadowsocks proxy configuration URI.
    /// </summary>
    public class ShadowSocksConfig : Payload
    {
        /// <summary>
        /// Lists the supported method values.
        /// </summary>
        public enum Method
        {
            /// <summary>
            /// Encrypts the Shadowsocks connection with chacha20 ietf poly1305.
            /// </summary>
            Chacha20IetfPoly1305,
            /// <summary>
            /// Encrypts the Shadowsocks connection with aes128 gcm.
            /// </summary>
            Aes128Gcm,
            /// <summary>
            /// Encrypts the Shadowsocks connection with aes192 gcm.
            /// </summary>
            Aes192Gcm,
            /// <summary>
            /// Encrypts the Shadowsocks connection with aes256 gcm.
            /// </summary>
            Aes256Gcm,
            /// <summary>
            /// Encrypts the Shadowsocks connection with x chacha20 ietf poly1305.
            /// </summary>
            XChacha20IetfPoly1305,
            /// <summary>
            /// Encrypts the Shadowsocks connection with aes128 cfb.
            /// </summary>
            Aes128Cfb,
            /// <summary>
            /// Encrypts the Shadowsocks connection with aes192 cfb.
            /// </summary>
            Aes192Cfb,
            /// <summary>
            /// Encrypts the Shadowsocks connection with aes256 cfb.
            /// </summary>
            Aes256Cfb,
            /// <summary>
            /// Encrypts the Shadowsocks connection with aes128 ctr.
            /// </summary>
            Aes128Ctr,
            /// <summary>
            /// Encrypts the Shadowsocks connection with aes192 ctr.
            /// </summary>
            Aes192Ctr,
            /// <summary>
            /// Encrypts the Shadowsocks connection with aes256 ctr.
            /// </summary>
            Aes256Ctr,
            /// <summary>
            /// Encrypts the Shadowsocks connection with camellia128 cfb.
            /// </summary>
            Camellia128Cfb,
            /// <summary>
            /// Encrypts the Shadowsocks connection with camellia192 cfb.
            /// </summary>
            Camellia192Cfb,
            /// <summary>
            /// Encrypts the Shadowsocks connection with camellia256 cfb.
            /// </summary>
            Camellia256Cfb,
            /// <summary>
            /// Encrypts the Shadowsocks connection with chacha20 ietf.
            /// </summary>
            Chacha20Ietf,
            /// <summary>
            /// Encrypts the Shadowsocks connection with aes256 cb.
            /// </summary>
            Aes256Cb,
            /// <summary>
            /// Encrypts the Shadowsocks connection with aes128 ofb.
            /// </summary>
            Aes128Ofb,
            /// <summary>
            /// Encrypts the Shadowsocks connection with aes192 ofb.
            /// </summary>
            Aes192Ofb,
            /// <summary>
            /// Encrypts the Shadowsocks connection with aes256 ofb.
            /// </summary>
            Aes256Ofb,
            /// <summary>
            /// Encrypts the Shadowsocks connection with aes128 cfb1.
            /// </summary>
            Aes128Cfb1,
            /// <summary>
            /// Encrypts the Shadowsocks connection with aes192 cfb1.
            /// </summary>
            Aes192Cfb1,
            /// <summary>
            /// Encrypts the Shadowsocks connection with aes256 cfb1.
            /// </summary>
            Aes256Cfb1,
            /// <summary>
            /// Encrypts the Shadowsocks connection with aes128 cfb8.
            /// </summary>
            Aes128Cfb8,
            /// <summary>
            /// Encrypts the Shadowsocks connection with aes192 cfb8.
            /// </summary>
            Aes192Cfb8,
            /// <summary>
            /// Encrypts the Shadowsocks connection with aes256 cfb8.
            /// </summary>
            Aes256Cfb8,
            /// <summary>
            /// Encrypts the Shadowsocks connection with chacha20.
            /// </summary>
            Chacha20,
            /// <summary>
            /// Encrypts the Shadowsocks connection with bf cfb.
            /// </summary>
            BfCfb,
            /// <summary>
            /// Encrypts the Shadowsocks connection with rc4 md5.
            /// </summary>
            Rc4Md5,
            /// <summary>
            /// Encrypts the Shadowsocks connection with salsa20.
            /// </summary>
            Salsa20,
            /// <summary>
            /// Encrypts the Shadowsocks connection with des cfb.
            /// </summary>
            DesCfb,
            /// <summary>
            /// Encrypts the Shadowsocks connection with idea cfb.
            /// </summary>
            IdeaCfb,
            /// <summary>
            /// Encrypts the Shadowsocks connection with rc2 cfb.
            /// </summary>
            Rc2Cfb,
            /// <summary>
            /// Encrypts the Shadowsocks connection with cast5 cfb.
            /// </summary>
            Cast5Cfb,
            /// <summary>
            /// Encrypts the Shadowsocks connection with salsa20 ctr.
            /// </summary>
            Salsa20Ctr,
            /// <summary>
            /// Encrypts the Shadowsocks connection with rc4.
            /// </summary>
            Rc4,
            /// <summary>
            /// Encrypts the Shadowsocks connection with seed cfb.
            /// </summary>
            SeedCfb,
            /// <summary>
            /// Encrypts the Shadowsocks connection with table.
            /// </summary>
            Table
        }

        /// <summary>
        /// Reports errors encountered while processing shadow socks config.
        /// </summary>
        public class ShadowSocksConfigException : Exception
        {
            /// <summary>
            /// Creates an exception for shadow socks config failures.
            /// </summary>
            public ShadowSocksConfigException()
            {
            }

            /// <summary>
            /// Creates an exception for shadow socks config failures.
            /// </summary>
            public ShadowSocksConfigException( string message )
                : base( message )
            {
            }

            /// <summary>
            /// Creates an exception for shadow socks config failures.
            /// </summary>
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

        /// <summary>
        /// Creates a shadow socks config payload.
        /// </summary>
        public ShadowSocksConfig( string hostname, int port, string password, Method method, string tag = null )
            : this( hostname, port, password, method, (Dictionary<string, string>)null, tag )
        {
        }

        /// <summary>
        /// Creates a shadow socks config payload.
        /// </summary>
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

        /// <summary>
        /// Creates a shadow socks config payload.
        /// </summary>
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

        /// <inheritdoc />
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

    /// <summary>
    /// Requests a Monero transfer with optional payment metadata.
    /// </summary>
    public class MoneroTransaction : Payload
    {
        /// <summary>
        /// Reports errors encountered while processing monero transaction.
        /// </summary>
        public class MoneroTransactionException : Exception
        {
            /// <summary>
            /// Creates an exception for monero transaction failures.
            /// </summary>
            public MoneroTransactionException()
            {
            }

            /// <summary>
            /// Creates an exception for monero transaction failures.
            /// </summary>
            public MoneroTransactionException( string message )
                : base( message )
            {
            }

            /// <summary>
            /// Creates an exception for monero transaction failures.
            /// </summary>
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

        /// <summary>
        /// Creates a monero transaction payload
        /// </summary>
        /// <param name="address">Receiver's monero address</param>
        /// <param name="txAmount">Amount to transfer</param>
        /// <param name="txPaymentId">Payment id</param>
        /// <param name="recipientName">Receipient's name</param>
        /// <param name="txDescription">Reference text / payment description</param>
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

        /// <inheritdoc />
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

    /// <summary>
    /// Produces a Slovenian UPN payment slip payload.
    /// </summary>
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

        /// <inheritdoc />
        public override int Version => 15;

        /// <inheritdoc />
        public override EccLevel EccLevel => EccLevel.M;

        /// <inheritdoc />
        public override EciMode EciMode => EciMode.Iso8859_2;

        private string LimitLength( string value, int maxLength )
        {
            if ( value.Length > maxLength )
            {
                return value.Substring( 0, maxLength );
            }

            return value;
        }

        /// <summary>
        /// Creates a slovenian upn qr payload.
        /// </summary>
        public SlovenianUpnQr( string payerName, string payerAddress, string payerPlace, string recipientName, string recipientAddress, string recipientPlace, string recipientIban, string description, double amount, string recipientSiModel = "SI00", string recipientSiReference = "", string code = "OTHR" )
            : this( payerName, payerAddress, payerPlace, recipientName, recipientAddress, recipientPlace, recipientIban, description, amount, null, recipientSiModel, recipientSiReference, code )
        {
        }

        /// <summary>
        /// Creates a slovenian upn qr payload.
        /// </summary>
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

        /// <inheritdoc />
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

    /// <summary>
    /// Produces a Russian bank payment order payload.
    /// </summary>
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

        /// <summary>
        /// Holds optional payer, tax, and payment metadata for a Russian order.
        /// </summary>
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

            /// <summary>
            /// Payment amount, in kopecks (FTI’s Amount.)
            /// <para>Сумма платежа, в копейках</para>
            /// </summary>
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

            /// <summary>
            /// Payment name (purpose)
            /// <para>Наименование платежа (назначение)</para>
            /// </summary>
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

            /// <summary>
            /// Payee's INN (Resident Tax Identification Number; Text, up to 12 characters.)
            /// <para>ИНН получателя платежа</para>
            /// </summary>
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

            /// <summary>
            /// Payer's INN (Resident Tax Identification Number; Text, up to 12 characters.)
            /// <para>ИНН плательщика</para>
            /// </summary>
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

            /// <summary>
            /// Status compiler payment document
            /// <para>Статус составителя платежного документа</para>
            /// </summary>
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

            /// <summary>
            /// KPP of the payee (Tax Registration Code; Text, up to 9 characters.)
            /// <para>КПП получателя платежа</para>
            /// </summary>
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

            /// <summary>
            /// CBC
            /// <para>КБК</para>
            /// </summary>
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

            /// <summary>
            /// All-Russian classifier territories of municipal formations
            /// <para>Общероссийский классификатор территорий муниципальных образований</para>
            /// </summary>
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

            /// <summary>
            /// Basis of tax payment
            /// <para>Основание налогового платежа</para>
            /// </summary>
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

            /// <summary>
            /// Taxable period
            /// <para>Налоговый период</para>
            /// </summary>
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

            /// <summary>
            /// Document number
            /// <para>Номер документа</para>
            /// </summary>
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

            /// <summary>
            /// Document date
            /// <para>Дата документа</para>
            /// </summary>
            public DateTime? DocDate { get; set; }

            /// <summary>
            /// Payment type
            /// <para>Тип платежа</para>
            /// </summary>
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

            /// <summary>
            /// Payer's surname
            /// <para>Фамилия плательщика</para>
            /// </summary>
            public string LastName { get; set; }

            /// <summary>
            /// Payer's name
            /// <para>Имя плательщика</para>
            /// </summary>
            public string FirstName { get; set; }

            /// <summary>
            /// Payer's patronymic
            /// <para>Отчество плательщика</para>
            /// </summary>
            public string MiddleName { get; set; }

            /// <summary>
            /// Payer's address
            /// <para>Адрес плательщика</para>
            /// </summary>
            public string PayerAddress { get; set; }

            /// <summary>
            /// Personal account of a budget recipient
            /// <para>Лицевой счет бюджетного получателя</para>
            /// </summary>
            public string PersonalAccount { get; set; }

            /// <summary>
            /// Payment document index
            /// <para>Индекс платежного документа</para>
            /// </summary>
            public string DocIdx { get; set; }

            /// <summary>
            /// Personal account number in the personalized accounting system in the Pension Fund of the Russian Federation - SNILS
            /// <para>№ лицевого счета в системе персонифицированного учета в ПФР - СНИЛС</para>
            /// </summary>
            public string PensAcc { get; set; }

            /// <summary>
            /// Number of contract
            /// <para>Номер договора</para>
            /// </summary>
            public string Contract { get; set; }

            /// <summary>
            /// Personal account number of the payer in the organization (in the accounting system of the PU)
            /// <para>Номер лицевого счета плательщика в организации (в системе учета ПУ)</para>
            /// </summary>
            public string PersAcc { get; set; }

            /// <summary>
            /// Apartment number
            /// <para>Номер квартиры</para>
            /// </summary>
            public string Flat { get; set; }

            /// <summary>
            /// Phone number
            /// <para>Номер телефона</para>
            /// </summary>
            public string Phone { get; set; }

            /// <summary>
            /// DUL payer type
            /// <para>Вид ДУЛ плательщика</para>
            /// </summary>
            public string PayerIdType { get; set; }

            /// <summary>
            /// DUL number of the payer
            /// <para>Номер ДУЛ плательщика</para>
            /// </summary>
            public string PayerIdNum { get; set; }

            /// <summary>
            /// FULL NAME. child / student
            /// <para>Ф.И.О. ребенка/учащегося</para>
            /// </summary>
            public string ChildFio { get; set; }

            /// <summary>
            /// Date of birth
            /// <para>Дата рождения</para>
            /// </summary>
            public DateTime? BirthDate { get; set; }

            /// <summary>
            /// Due date / Invoice date
            /// <para>Срок платежа/дата выставления счета</para>
            /// </summary>
            public string PaymTerm { get; set; }

            /// <summary>
            /// Payment period
            /// <para>Период оплаты</para>
            /// </summary>
            public string PaymPeriod { get; set; }

            /// <summary>
            /// Payment type
            /// <para>Вид платежа</para>
            /// </summary>
            public string Category { get; set; }

            /// <summary>
            /// Service code / meter name
            /// <para>Код услуги/название прибора учета</para>
            /// </summary>
            public string ServiceName { get; set; }

            /// <summary>
            /// Metering device number
            /// <para>Номер прибора учета</para>
            /// </summary>
            public string CounterId { get; set; }

            /// <summary>
            /// Meter reading
            /// <para>Показание прибора учета</para>
            /// </summary>
            public string CounterVal { get; set; }

            /// <summary>
            /// Notification, accrual, account number
            /// <para>Номер извещения, начисления, счета</para>
            /// </summary>
            public string QuittId { get; set; }

            /// <summary>
            /// Date of notification / accrual / invoice / resolution (for traffic police)
            /// <para>Дата извещения/начисления/счета/постановления (для ГИБДД)</para>
            /// </summary>
            public DateTime? QuittDate { get; set; }

            /// <summary>
            /// Institution number (educational, medical)
            /// <para>Номер учреждения (образовательного, медицинского)</para>
            /// </summary>
            public string InstNum { get; set; }

            /// <summary>
            /// Kindergarten / school class number
            /// <para>Номер группы детсада/класса школы</para>
            /// </summary>
            public string ClassNum { get; set; }

            /// <summary>
            /// Full name of the teacher, specialist providing the service
            /// <para>ФИО преподавателя, специалиста, оказывающего услугу</para>
            /// </summary>
            public string SpecFio { get; set; }

            /// <summary>
            /// Insurance / additional service amount / Penalty amount (in kopecks)
            /// <para>Сумма страховки/дополнительной услуги/Сумма пени (в копейках)</para>
            /// </summary>
            public string AddAmount { get; set; }

            /// <summary>
            /// Resolution number (for traffic police)
            /// <para>Номер постановления (для ГИБДД)</para>
            /// </summary>
            public string RuleId { get; set; }

            /// <summary>
            /// Enforcement Proceedings Number
            /// <para>Номер исполнительного производства</para>
            /// </summary>
            public string ExecId { get; set; }

            /// <summary>
            /// Type of payment code (for example, for payments to Rosreestr)
            /// <para>Код вида платежа (например, для платежей в адрес Росреестра)</para>
            /// </summary>
            public string RegType { get; set; }

            /// <summary>
            /// Unique accrual identifier
            /// <para>Уникальный идентификатор начисления</para>
            /// </summary>
            public string UIN { get; set; }

            /// <summary>
            /// The technical code recommended by the service provider. Maybe used by the receiving organization to call the appropriate processing IT system.
            /// <para>Технический код, рекомендуемый для заполнения поставщиком услуг. Может использоваться принимающей организацией для вызова соответствующей обрабатывающей ИТ-системы.</para>
            /// </summary>
            public TechCode? TechCode { get; set; }
        }

        /// <summary>
        /// (List of values of the technical code of the payment)
        /// <para>Перечень значений технического кода платежа</para>
        /// </summary>
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

        /// <summary>
        /// Lists the supported character sets values.
        /// </summary>
        public enum CharacterSets
        {
            /// <summary>
            /// Encodes the payload using windows_1251.
            /// </summary>
            windows_1251 = 1,
            /// <summary>
            /// Encodes the payload using utf_8.
            /// </summary>
            utf_8,
            /// <summary>
            /// Encodes the payload using koi8_r.
            /// </summary>
            koi8_r
        }

        /// <summary>
        /// Reports errors encountered while processing russia payment order.
        /// </summary>
        public class RussiaPaymentOrderException : Exception
        {
            /// <summary>
            /// Creates an exception for russia payment order failures.
            /// </summary>
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

        /// <summary>
        /// Creates a russia payment order payload.
        /// </summary>
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

        /// <inheritdoc />
        public override string ToString()
        {
            string name = characterSet.ToString().Replace( "_", "-" );
            byte[] bytes = ToBytes();
            Encoding.RegisterProvider( CodePagesEncodingProvider.Instance );
            return Encoding.GetEncoding( name ).GetString( bytes );
        }

        /// <summary>
        /// Encodes the Russian payment order as bytes.
        /// </summary>
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

        /// <summary>
        /// Determines a valid separator
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Takes all optional fields that are not null and returns their string represantion
        /// </summary>
        /// <returns>A List of strings</returns>
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

        /// <summary>
        /// Takes all mandatory fields that are not null and returns their string represantion
        /// </summary>
        /// <returns>A List of strings</returns>
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

        /// <summary>
        /// Validates a string against a given Regex pattern. Returns input if it matches the Regex expression (=valid) or throws Exception in case there's a mismatch
        /// </summary>
        /// <param name="input">String to be validated</param>
        /// <param name="fieldname">Name/descriptor of the string to be validated</param>
        /// <param name="pattern">A regex pattern to be used for validation</param>
        /// <param name="errorText">An optional error text. If null, a standard error text is generated</param>
        /// <returns>Input value (in case it is valid)</returns>
        private static string ValidateInput( string input, string fieldname, string pattern, string errorText = null )
        {
            return ValidateInput( input, fieldname, new string[1] { pattern }, errorText );
        }

        /// <summary>
        /// Validates a string against one or more given Regex patterns. Returns input if it matches all regex expressions (=valid) or throws Exception in case there's a mismatch
        /// </summary>
        /// <param name="input">String to be validated</param>
        /// <param name="fieldname">Name/descriptor of the string to be validated</param>
        /// <param name="patterns">An array of regex patterns to be used for validation</param>
        /// <param name="errorText">An optional error text. If null, a standard error text is generated</param>
        /// <returns>Input value (in case it is valid)</returns>
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

    /// <summary>
    /// Calculates a Mod-10 checksum for the payload.
    /// </summary>
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