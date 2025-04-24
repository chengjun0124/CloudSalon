using CloudSalon.Model.Enum;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CloudSalon.Common
{
    public enum CompareOperation
    {
        Equal = 0,
        Greater = 1,
        GreaterEqual = 2,
        Less = 3,
        LessEqual = 4,
        NoEqual = 5
    }

    public class ValidatorContainer
    {
        List<string> _invalidMessages = null;
        string _fieldName;
        object _value;
        bool _isValid = true;
        RequiredValidator _requiredValidator = new RequiredValidator();
        LengthValidator _lengthValidator = new LengthValidator();
        PhoneNumberValidator _phoneNumberValidator = new PhoneNumberValidator();
        MobileValidator _mobileValidator = new MobileValidator();
        CompareValidator _compareValidator = new CompareValidator();
        RangeValidator _rangeValidator = new RangeValidator();
        TimeSpanScaleValidator _timeSpanScaleValidator = new TimeSpanScaleValidator();
        RegexValidator _regexValidator = new RegexValidator();
        EnumValidator _inListValidator = new EnumValidator();
        CustomValidator _customValidator = new CustomValidator();
        ImageValidator _imageValidator = new ImageValidator();
        OneRequiredValidator _required2Validator = new OneRequiredValidator();
        DecimalValidator _decimalValidator = new DecimalValidator();

        public ValidatorContainer(List<string> invalidMessages)
        {
            this._invalidMessages = invalidMessages;
        }

        public ValidatorContainer SetValue(object o)
        {
            _isValid = true;
            _value = o;
            return this;
        }

        public ValidatorContainer SetValue(string fieldName, object o)
        {
            _isValid = true;
            _value = o;
            _fieldName = fieldName;
            return this;
        }

        private ValidatorContainer PreValidate(IValidator validator)
        {
            if (_isValid)
            {
                validator.FieldName = this._fieldName;
                _isValid = validator.PreValidate(this._value);

                if (!_isValid)
                    _invalidMessages.Add(validator.InvalidMessage);
            }
            return this;
        }


        #region 公开的验证方法，给API用，每增加一个验证器，此处增加一个对应的方法
        public ValidatorContainer IsRequired()
        {
            return this.PreValidate(_requiredValidator);
        }

        public ValidatorContainer Compare(string comparedFieldName, IComparable comparedValue, CompareOperation operation)
        {
            _compareValidator.ComparedFieldName = comparedFieldName;
            _compareValidator.ComparedValue = comparedValue;
            _compareValidator.Operation = operation;
            return this.PreValidate(_compareValidator);
        }

        public ValidatorContainer Length(int? minLength, int maxLength)
        {
            if (minLength.HasValue)
                _lengthValidator.MinLength = minLength.Value;
            else
                _lengthValidator.MinLength = 0;

            _lengthValidator.MaxLength = maxLength;
            return this.PreValidate(_lengthValidator);
        }

        public ValidatorContainer IsPhoneNumer()
        {
            return this.PreValidate(_phoneNumberValidator);
        }

        public ValidatorContainer IsMobile()
        {
            return this.PreValidate(_mobileValidator);
        }
        
        public ValidatorContainer InRange(IComparable min, IComparable max, string format)
        {
            _rangeValidator.Min = min;
            _rangeValidator.Max = max;
            _rangeValidator.Format = format;
            return this.PreValidate(_rangeValidator);
        }

        public ValidatorContainer IsInScale(int minuteScale)
        {
            _timeSpanScaleValidator.MinuteScale = minuteScale;
            return this.PreValidate(_timeSpanScaleValidator);
        }


        public ValidatorContainer Pattern(string pattern, string customInvalidMessage)
        {
            _regexValidator.CustomInvalidMessage = customInvalidMessage;
            _regexValidator.Pattern = pattern;
            return this.PreValidate(_regexValidator);
        }

        public ValidatorContainer IsInList(params Enum[] List)
        {
            _inListValidator.List = List;
            return this.PreValidate(_inListValidator);
        }

        public ValidatorContainer Custom(Func<bool> callBack, string customInvalidMessage)
        {
            _customValidator.CustomInvalidMessage = customInvalidMessage;
            _customValidator.CallBack = callBack;
            return this.PreValidate(_customValidator);
        }

        public ValidatorContainer IsImage(int width, int height)
        {
            _imageValidator.Width = width;
            _imageValidator.Height = height;
            return this.PreValidate(_imageValidator);
        }

        public ValidatorContainer IsOneRequired()
        {
            return this.PreValidate(_required2Validator);
        }

        public ValidatorContainer DecimalLength(int length)
        {
            _decimalValidator.DecimalLength = length;
            return this.PreValidate(_decimalValidator);
        }
        #endregion

        #region 验证器
        interface IValidator
        {
            string FieldName { get; set; }
            string InvalidMessage { get; }
            bool PreValidate(object value);
        }

        public abstract class BaseValidator<T> : IValidator
        {
            public string FieldName { get; set; }

            public abstract string InvalidMessage { get; }

            public abstract bool Validate(T value);

            public virtual bool PreValidate(object value)
            {
                if (value != null)
                {
                    //return this.Validate((T)Convert.ChangeType(value, typeof(T)));
                    return this.Validate((T)value);
                }

                return true;
            }

        }

        class RequiredValidator : BaseValidator<object>
        {
            public override bool PreValidate(object value)
            {
                return this.Validate(value);
            }

            public override bool Validate(object value)
            {
                if (value is System.Collections.IList)
                {
                    System.Collections.IList arr = (System.Collections.IList)value;
                    return arr.Count > 0;
                }
                else
                {
                    if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                        return false;
                    else
                        return true;
                }
            }

            public override string InvalidMessage
            {
                get { return "请输入" + this.FieldName; }
            }
        }

        class OneRequiredValidator : BaseValidator<object[]>
        {
            public override string InvalidMessage
            {
                get { return this.FieldName + "至少填写一项"; }
            }

            public override bool Validate(object[] value)
            {
                foreach(object v in value)
                {
                    if (v != null && !string.IsNullOrWhiteSpace(v.ToString()))
                        return true;
                }
                return false;
            }
        }

        class DecimalValidator : BaseValidator<decimal>
        {
            public int DecimalLength { get; set; }
            public override string InvalidMessage
            {
                get {
                    return this.FieldName + "不能超过" + this.DecimalLength + "位小数";
                }
            }

            public override bool Validate(decimal value)
            {
                string str = value.ToString();
                int index = str.IndexOf(".");
                if (index > -1)
                {
                    str = str.Substring(index + 1, str.Length - index - 1);
                    return str.Length <= DecimalLength;
                }
                else
                    return true;
            }
        }

        class PhoneNumberValidator : BaseValidator<string>
        {
            public override string InvalidMessage
            {
                get {
                    return this.FieldName + "必须为纯数字,无需横杠";
                }
            }

            public override bool Validate(string value)
            {
                return Regex.IsMatch(value, @"^\d+$");
                
            }
        }

        class MobileValidator : BaseValidator<string>
        {
            public override string InvalidMessage
            {
                get
                {
                    return this.FieldName + "必须为11位纯数字";
                }
            }

            public override bool Validate(string value)
            {
                return Regex.IsMatch(value, @"^\d{11}$");

            }
        }

        class LengthValidator : BaseValidator<string>
        {
            public int MaxLength { get; set; }
            public int MinLength { get; set; }

            public override string InvalidMessage
            {
                get {
                    if (MinLength == 0)
                        return base.FieldName + "不能超过" + MaxLength + "个字符";
                    else
                        return base.FieldName + "长度必须是" + MinLength + "至" + MaxLength + "个字符";
                }
            }

            public override bool Validate(string value)
            {
                if (MinLength == 0)
                    return value.Length <= MaxLength;
                else
                    return value.Length >= MinLength && value.Length <= MaxLength;
            }
        }

        class CompareValidator : BaseValidator<IComparable>
        {
            public string ComparedFieldName { get; set; }
            public IComparable ComparedValue { get; set; }
            public CompareOperation Operation { get; set; }

            public override string InvalidMessage
            {
                get
                {
                    string str = "";
                    if (this.Operation == CompareOperation.Equal)
                        str = "等于";
                    else if (this.Operation == CompareOperation.Greater)
                        str = "大于";
                    else if (this.Operation == CompareOperation.GreaterEqual)
                        str = "大于等于";
                    else if (this.Operation == CompareOperation.Less)
                        str = "小于";
                    else if (this.Operation == CompareOperation.LessEqual)
                        str = "小于等于";
                    else
                        str = "不等于";

                    return this.FieldName + "必须" + str + this.ComparedFieldName;
                }
            }

            public override bool Validate(IComparable value)
            {
                //CompareTo返回值含义
                //1:大于
                //0:==
                //-1小于

                int result = value.CompareTo(this.ComparedValue);

                //if (this.Operation == CompareOperation.Equal)
                //    return result == 0;
                //else if (this.Operation == CompareOperation.Greater)
                //    return result == 1;
                //else if (this.Operation == CompareOperation.GreaterEqual)
                //    return result == 1 || result == 0;
                //else if (this.Operation == CompareOperation.Less)
                //    return result == -1;
                //else if (this.Operation == CompareOperation.LessEqual)
                //    return result == -1 || result == 0;
                //else
                //    return result != 0;

                if (this.Operation == CompareOperation.Equal)
                    return result == 0;
                else if (this.Operation == CompareOperation.Greater)
                    return result > 0;
                else if (this.Operation == CompareOperation.GreaterEqual)
                    return result >= 0;
                else if (this.Operation == CompareOperation.Less)
                    return result < 0;
                else if (this.Operation == CompareOperation.LessEqual)
                    return result <= 0;
                else
                    return result != 0;
            }
        }

        class RangeValidator : BaseValidator<IComparable>
        {
            public IComparable Min { get; set; }
            public IComparable Max { get; set; }
            public string Format { get; set; }
            public override string InvalidMessage
            {
                get {
                    return this.FieldName + "必须在" + ((IFormattable)Min).ToString(Format, null) + "到" + ((IFormattable)Max).ToString(Format, null) + "之间";
                    //return this.FieldName + "必须在" + Min+ "到" + Max + "之间";
                }
            }

            public override bool Validate(IComparable value)
            {
                //CompareTo返回值含义
                //1:大于
                //0:==
                //-1小于

                if (value.CompareTo(this.Min) < 0)
                    return false;

                if (value.CompareTo(this.Max) > 0)
                    return false;

                return true;
            }
        }

        //时间刻度验证器，比如时间字段只允许是30分钟的倍数
        class TimeSpanScaleValidator : BaseValidator<TimeSpan>
        {
            public int MinuteScale { get; set; }
            public override string InvalidMessage
            {
                get
                {
                    return this.FieldName + "必须为" + MinuteScale + "分钟的倍数";
                }
            }

            public override bool Validate(TimeSpan value)
            {
                return value.Ticks % (TimeSpan.TicksPerMinute * this.MinuteScale) == 0;
            }
        }

       

        class EnumValidator : BaseValidator<Enum>
        {
            public Enum[] List { get; set; }
            public override string InvalidMessage
            {
                get {
                    return "请选择" + base.FieldName;
                }
            }

            public override bool Validate(Enum value)
            {
                return List.Contains(value);
            }
        }


        abstract class CustomInvalidMessageValidator<T> : BaseValidator<T>
        {
            public string CustomInvalidMessage { get; set; }
            public override string InvalidMessage
            {
                get
                {
                    return this.FieldName + this.CustomInvalidMessage;
                }
            }

            public override bool Validate(T value)
            {
                throw new NotImplementedException();
            }
        }

        class RegexValidator : CustomInvalidMessageValidator<string>
        {
            public string Pattern { get; set; }

            public override bool Validate(string value)
            {
                Regex regex = new Regex(Pattern);
                return regex.IsMatch(value);
            }
        }

        class CustomValidator : CustomInvalidMessageValidator<object>
        {
            public Func<bool> CallBack { get; set; }

            public override bool Validate(object value)
            {
                return CallBack();
            }
        }

        class ImageValidator : BaseValidator<string>
        {
            ValidImageStatus _status;
            public int Width { get; set; }
            public int Height { get; set; }
            public override string InvalidMessage
            {
                get {
                    if (_status == ValidImageStatus.IllegalImage)
                        return this.FieldName + "是非法图片";
                    else if (_status == ValidImageStatus.InvalidImageFormat)
                        return this.FieldName + "请上传jpg图片";
                    else
                        return this.FieldName + "图片尺寸必须为" + this.Width + "*" + this.Height;
                }
            }

            public override bool Validate(string value)
            {
                byte[] bytes = null;
                Image image = null;
                try
                {
                    bytes = Convert.FromBase64String(value);
                    MemoryStream m = new MemoryStream(bytes);
                    image = System.Drawing.Image.FromStream(m);
                }
                catch
                {
                    _status=ValidImageStatus.IllegalImage;
                    return false;
                }

                if (!image.RawFormat.Equals(ImageFormat.Jpeg))
                {
                    _status = ValidImageStatus.InvalidImageFormat;
                    return false;
                }
                if (image.Width != this.Width || image.Height != this.Height)
                {
                    _status = ValidImageStatus.InvalidImageSize;
                    return false;
                }
                _status = ValidImageStatus.OK;
                return true;
            }
        }
        #endregion
    }
}
