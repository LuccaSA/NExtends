# Change log

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
