// Copyright 2008 Andy Kernahan
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AK.Net.Dns
{
    /// <summary>
    /// Represents an immutable DNS name. This class cannot be inherited.
    /// </summary>
    [Serializable]
    public sealed class DnsName : IEquatable<DnsName>, IComparable<DnsName>, IComparable
    {
        #region Private Fields.

        private string _absName;
        private readonly string _name;
        private readonly DnsNameKind _kind;        
        private IList<string> _labels;

        private static readonly char[] LABEL_DELIMS =
            DnsName.LabelSeperator.ToCharArray();

        #endregion

        #region Public Interface.

        /// <summary>
        /// Defines the maximum allowed length of an encoded domain name.
        /// This field is constant.
        /// </summary>
        public const int MaxLength = 255;

        /// <summary>
        /// Defines the maximum allowed length of a single domain label.
        /// This field is constant.
        /// </summary>
        public const int MaxLabelLength = 63;

        /// <summary>
        /// Defines the maxmimum number of labels in a domain name.
        /// This field is constant.
        /// </summary>
        public const int MaxLabels = 128;

        /// <summary>
        /// Defines the label seperator token. This field is constant.
        /// </summary>
        public const string LabelSeperator = ".";

        /// <summary>
        /// Defines the string comparer used to compare domains name labels.
        /// This field is <see langword="readonly"/>.
        /// </summary>
        public static readonly StringComparer LabelComparer = StringComparer.OrdinalIgnoreCase;

        /// <summary>
        /// Defines the root DNS name. This field is <see langword="readonly"/>.
        /// </summary>
        public static readonly DnsName Root = new DnsName(DnsName.LabelSeperator);

        /// <summary>
        /// Creates a new name instance that represents this name made relative to
        /// the specified parent <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The parent name.</param>
        /// <returns>A new name instance that represents this name made relative to the
        /// specified parent <paramref name="name"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="name"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when this name is not a sub-domain of the specified <paramref name="name"/>
        /// and therefore cannot be made relative to it.
        /// </exception>
        public DnsName MakeRelative(DnsName name) {

            Guard.NotNull(name, "name");
            if(!IsChildOf(name))
                throw Guard.UnableToMakeRelativeNotAChildOfName(this, name, "name");

            // All domains are relative to the root domain.
            if(name.Equals(DnsName.Root))
                return this;

            int length = this.Name.Length - name.Name.Length - 1;

            if(this.Kind != name.Kind)
                length = name.Kind == DnsNameKind.Absolute ? length + 1 : length - 1;

            return new DnsName(this.Name.Substring(0, length));
        }

        /// <summary>
        /// Returns a value indicating if this name is the parent of the specified
        /// <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The other name.</param>
        /// <returns><see langword="true"/> if this name is the parent of the specified
        /// <paramref name="name"/>, otherwise; <see langword="false"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="name"/> is <see langword="null"/>.
        /// </exception>
        public bool IsParentOf(DnsName name) {

            Guard.NotNull(name, "name");

            if(this.Labels.Count >= name.Labels.Count)
                return false;

            for(int i = this.Labels.Count - 1, j = name.Labels.Count - 1; i >= 0; --i, --j) {
                if(!DnsName.LabelComparer.Equals(this.Labels[i], name.Labels[j]))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Returns a value indicating if this name is a sub-domain of the specified
        /// <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The parent domain.</param>
        /// <returns><see langword="true"/> if this name is a sub-domain of the specified
        /// <paramref name="name"/>, otherwise; <see langword="false"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="name"/> is <see langword="null"/>.
        /// </exception>
        public bool IsChildOf(DnsName name) {

            Guard.NotNull(name, "name");

            return name.IsParentOf(this);
        }

        /// <summary>
        /// Creates new name instance that represents this name with the specified
        /// <paramref name="name"/> concatenated.
        /// </summary>
        /// <param name="name">The name to be concatenated.</param>
        /// <returns>A new name instance that represents this name with the specified
        /// <paramref name="name"/> concatenated.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="name"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// Thrown when <see langword="this"/> is absolute.
        /// </exception>
        public DnsName Concat(DnsName name) {

            Guard.NotNull(name, "name");
            if(this.Kind == DnsNameKind.Absolute)
                throw Guard.UnableToConcatToAbsoluteDnsName();            

            return new DnsName(this.Name + DnsName.LabelSeperator + name.Name);
        }

        /// <summary>
        /// Returns a value indicating equality with the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns><see langword="true"/> if this instance is considered equal to the
        /// specified object, otherwise; <see langword="false"/>.</returns>
        public override bool Equals(object obj) {

            if(obj == null)
                return false;            
            if(obj.GetType() != GetType())
                return false;

            return Equals((DnsName)obj);
        }

        /// <summary>
        /// Returns a value indicating equality with the other DnsName.
        /// </summary>
        /// <param name="other">The other name.</param>
        /// <returns><see langword="true"/> if this instance is considered equal to the
        /// other instance, otherwise; <see langword="false"/>.</returns>
        public bool Equals(DnsName other) {

            if(other == null)
                return false;

            return DnsName.LabelComparer.Equals(this.AbsName, other.AbsName);
        }

        /// <summary>
        /// Returns a hash code based on the values of this instance.
        /// </summary>
        /// <returns>A hash code based on the values of this instance.</returns>
        public override int GetHashCode() {

            return DnsName.LabelComparer.GetHashCode(this.AbsName);
        }

        /// <summary>
        /// Returns a value indicating relative equality with the other DnsName.
        /// </summary>
        /// <param name="other">The other name.</param>
        /// <returns>A value indicating relative equality with the other DnsName.</returns>
        public int CompareTo(DnsName other) {

            return other != null ? DnsName.LabelComparer.Compare(this.AbsName, other.AbsName) : 1;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> representation of this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> representation of this instance.</returns>
        public override string ToString() {

            return this.Name;
        }

        /// <summary>
        /// Gets the full name.
        /// </summary>
        public string Name {

            get { return _name; }
        }

        /// <summary>
        /// Gets the <see cref="AK.Net.Dns.DnsNameKind"/> if this name.
        /// </summary>
        public DnsNameKind Kind {

            get { return _kind; }
        }

        /// <summary>
        /// Gets the labels of this name.
        /// </summary>
        public IList<string> Labels {

            get {
                if(_labels == null) {
                    _labels = new ReadOnlyCollection<string>(this.Name.Split(
                        LABEL_DELIMS, StringSplitOptions.RemoveEmptyEntries));
                }
                return _labels;
            }
        }

        /// <summary>
        /// Parses a <see cref="AK.Net.Dns.DnsName"/> from the specified
        /// <paramref name="name"/>.
        /// </summary>
        /// <param name="name">
        /// The domain name to parse.
        /// </param>
        /// <returns>
        /// The parsed DnsName.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="name"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <paramref name="name"/> is an empty <see cref="System.String"/>.
        /// </exception>
        /// <exception cref="AK.Net.Dns.DnsFormatException">
        /// Thrown when <paramref name="name"/> is not correctly formatted.
        /// </exception>
        public static DnsName Parse(string name) {

            Guard.NotEmpty(name, "name");
            if(!IsValid(name))
                throw Guard.MustBeAValidDnsName("name", name);

            return new DnsName(name);
        }

        /// <summary>
        /// Attemps to parse a <see cref="AK.Net.Dns.DnsName"/> from the specified
        /// <paramref name="name"/> and returns a result indicating success.
        /// </summary>
        /// <param name="name">
        /// The domain name to parse.
        /// </param>
        /// <param name="result">
        /// On success, this parameter will contain the parsed name.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a name was sucessfuly parsed, otherwise;
        /// <see langword="false"/>.
        /// </returns>
        public static bool TryParse(string name, out DnsName result) {
            
            if(IsValid(name)) {
                result = new DnsName(name);
                return true;
            }
            result = null;
            return false;
        }

        /// <summary>
        /// Implicit <see cref="System.String"/> conversion operator.
        /// </summary>
        /// <param name="name">The name to convert.</param>
        /// <returns>The converted name.</returns>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <paramref name="name"/> is an empty <see cref="System.String"/>.
        /// </exception>
        /// <exception cref="AK.Net.Dns.DnsFormatException">
        /// Thrown when <paramref name="name"/> is not correctly formatted.
        /// </exception>
        public static implicit operator DnsName(string name) {

            return name != null ? DnsName.Parse(name) : null;
        }

        /// <summary>
        /// Implicit <see cref="System.String"/> conversion operator.
        /// </summary>
        /// <param name="name">The name to convert.</param>
        /// <returns>The converted name.</returns>
        /// <exception cref="System.NullReferenceException">
        /// Thrown when <paramref name="name"/> is <see langword="null"/>.
        /// </exception>
        public static implicit operator string(DnsName name) {

            return name != null ? name.Name : null;
        }

        /// <summary>
        /// Returns a value indicating if the specified name is correctly formatted.
        /// </summary>
        /// <param name="name">The domain name.</param>
        /// <returns>
        /// <see langword="true"/> if canonical, otherwise; <see langword="false"/>.
        /// </returns>
        public static bool IsValid(string name) {

            if(string.IsNullOrEmpty(name) || name.Length > DnsName.MaxLength)
                return false;

            // Root domain.
            if(name.Equals("."))
                return true;

            char ch;            
            int labelLen = 0;
            int labelCount = 0;
            
            for(int i = 0; i < name.Length; ++i) {
                ch = name[i];
                switch(ch) {
                    case '.':
                        // Empty labels are illegal.
                        if(labelLen == 0)
                            return false;                        
                        if(++labelCount > DnsName.MaxLabels)
                            return false;
                        labelLen = 0;
                        break;
                    case '-':
                        // Empty labels are illegal.
                        if(labelLen == 0)
                            return false;
                        // Labels ending with '-' are illegal.
                        if(i >= name.Length - 1 || name[i + 1] == '.')
                            return false;
                        ++labelLen;
                        break;
                    case '_':
                        // In order to support SRV [RFC2782] we allow labels to start with
                        // an underscore, but they are illegal anywhere else.
                        if(labelLen != 0)
                            return false;
                        break;
                    default:
                        if(!IsAlhaNum(ch) || ++labelLen > DnsName.MaxLabelLength)
                            return false;
                        break;
                }                
            }
            return true;
        }

        /// <summary>
        /// Classifies the <see cref="AK.Net.Dns.DnsNameKind"/> of the specified
        /// <paramref name="name"/>.
        /// </summary>
        /// <param name="name">
        /// The name whose kind is to be classified.
        /// </param>
        /// <returns>
        /// The kind classification of the specified <paramref name="name"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="name"/> is <see langword="null"/>.
        /// </exception>
        public static DnsNameKind ClassifyKind(string name) {

            Guard.NotNull(name, "name");

            return name.EndsWith(DnsName.LabelSeperator, StringComparison.Ordinal) ?
                DnsNameKind.Absolute : DnsNameKind.Relative;
        }

        #endregion

        #region Explit Interface.

        int IComparable.CompareTo(object obj) {

            return CompareTo(obj as DnsName);
        }

        #endregion

        #region Private Impl.

        private DnsName(string name) {

            _name = name;
            _kind = ClassifyKind(name);
        }
        
        /// <summary>
        /// Returns the absolute version of this name.
        /// </summary>
        /// <remarks>
        /// This is used during comparison and equality testing to ensure that kind is
        /// not considered.
        /// </remarks>
        private string AbsName {

            get {
                if(_absName == null) {                    
                    _absName = this.Kind == DnsNameKind.Absolute ? 
                        this.Name : this.Name + DnsName.LabelSeperator;
                }
                return _absName;
            }
        }

        private static bool IsAlhaNum(char ch) {

            return (ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || (ch >= '0' && ch <= '9');
        }

        #endregion
    }
}
