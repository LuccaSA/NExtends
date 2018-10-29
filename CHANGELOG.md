# Change log

## 2.1

### Removed

- DictionaryListMapper.cs
- ObjectPropertiesList.cs
- DateTime.DayText()
- Shift (only used in legacy YupieManager.cs)
- Add and ContainsKey methods : dangerous method given the unusual use case
- Json, XML, INSEE methods & extensions
- ObjectExtensions

### Add & improves

- Tests on RequiredIfAttribute
- Improve speed of method
- Expressions features from Lucca
- Refactoring of UpdateKey, Contains methods
- Specific targeting for ToDictionary & ToHashSet (api surface differs between net461 & netcoreapp2)
- String.Replace, IsGuid optimisation

### Resolved

- #17 - Aligns GetFriendlyName( ) upon Lucca implementation
- #3
- #8
- #43

### Miscellanous

- NETCore contains case insensitive replacement natively, but here we are in NETStandard ;)
- Xunit updated to 2.3.1

## 2.0 (.NET Standard 2.0)

### Changes

- Type.GetMember() needs System.Reflection in usings
- GetCustomAttributes() returns IEnumerable, so .Length nor .[0] does not work anymore => .Count(), .ElementAt(0)
- Thread.CurrentThread.CurrentCulture => CultureInfo.CurrentCulture
- Type.xx => Type.GetTypeInfo().xx
- DateTime.ToShortDateString() => use DateTime.ToString("d")
- DateTime.ToShortTimeString() => use DateTime.ToString("t")

### Breaking changes

- AgilityPack, AntiXss, ToXMLAttribute => directly use Nuget Package HtmlSanitizer https://github.com/mganss/HtmlSanitizer
- System.Net.Mail => is not supported in .NET Core yet, use MailKit instead https://github.com/jstedfast/MailKit#using-mailkit
- StringComparison.InvariantCultureIgnoreCase => does not exists anymore, closest value chosen : StringComparison.OrdinalIgnoreCase
- IndentXMLString => not supported anymore
- PluralizationService => dead in EF7
- GetClientIpAddress => not supported anymore
- NameValueCollection => seems to have disappear from System.Collections.Specialized
- ToDictionary(this NameValueCollection collection) => not supported anymore
- CultureInfo(int lcid) => seems to be deprecated
- .toShortUserFormat() => not supported anymore
- GetOriginalName uses CodeDom => not supported anymore

## 1.1.1

### Resolved issues
 - IEnumerable isNullOrEmpty cleanup

## 1.1.0

### Breaking changes
 - DecimalExtensions' namespace changing from NExtends.Primitives to NExtends.Primitives.Decimals 

## 1.0.10

### New features
 - RequiredIfAttribute : aims to specify a condition against entity validation
 - GetPublicPropertiesIncludingExplicitInterfaceImplementations : various classes for retreiving entity properties

## 1.0.9

### Resolved issues
 - EnumExtensions.GetAttributeOfType() did not work when enum value had no attribute defined

## 1.0.8

### New features
 - .NET 4.6.1 for the projects
 - RemoveDiacritics : remove accents
 - RemoveSpecialCaracters : remove everything but letters (a-Z)

### Breaking changes
 - .NET version upgrade, you have to upgrade your own projects to the same version (4.6.1)
 - Removed methods ToUpperSansAccent(), SansAccent(), IsMatch(), you have to use new methods instead. There is no equivalent for ToUpperSansAccent(), you habe to do .ToUpper() yourself

## 1.0.7

### New features
 - Standalone project, with dependencies registration

## 1.0.6

### New features
 - Handling `long` conversion

## 1.0.5 - Copy/paste from Lucca and more

### New features
 - Expression.Not() to complete the PredicateBuilder And() and Or() methods
 - (T or IEnumerable<T>).Cast<T, U, I>() for casting T into U, given that T and U both implement interface I. Only members from I are copied from T to U
 - CulturedDisplayNameAttribute, in order to avoid repeating the RESX name when using cultured attributes
 - HtmlUtility for sanitizing incoming HTML data
 - Better INSEE_NUMBER support in string extensions
 - TimeSpanExtensions
 - PropertyInfo.IsEqual(Expression exp)

## 1.0.4 - HttpRequestMessage extensions

### New features
 - Extension that gets the IP host address of the remote client

## 1.0.3 - Uri support

### New features
 - Strings can be converted to Uri using .ChangeType extension

## 1.0.2 - Integrate with CloudControl

### New features
 - IsFirstOfMonth()/IsLastOfMonth() pour tester si une date est  la première/dernière du mois
 - ToShortDate() pour avoir une sortie type : août 2015
 - GetValueFromDescription()/GetDescriptionFromValue() sur les Enum pour récupérer la valeur de l'Enum en fonction de sa description, et vice-versa

### Breaking changes

### Resolved issues

#### Dependencies
