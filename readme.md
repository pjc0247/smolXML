smolXML
====

a tiny xml parser for `xml` + `text` mixed contents.

```xml
Hello <translate><kr>월드</kr><en>World</en></translate>.
```
Should be parsed to:
```
RootElement
  TextElement
    Hello
  Element: translate
    Element: kr
      월드
    Element: en
      World
  TextElement
    .
```