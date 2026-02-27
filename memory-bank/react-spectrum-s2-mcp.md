# React Spectrum S2 MCP Server

## Connection
- **Server Name**: `React Spectrum (S2)`
- **Command**: `npx @react-spectrum/mcp@latest`

## Available Tools

### list_s2_pages
Returns a list of all available documentation pages in the S2 docs.

**Input Schema:**
```json
{
  "includeDescription": boolean  // optional, include page descriptions
}
```

**Usage:**
```
use_mcp_tool with server_name="React Spectrum (S2)", tool_name="list_s2_pages"
```

---

### get_s2_page_info
Returns page description and list of sections for a given page.

**Input Schema:**
```json
{
  "page_name": string  // required, e.g., "Button", "TextField"
}
```

**Usage:**
```
use_mcp_tool with server_name="React Spectrum (S2)", tool_name="get_s2_page_info", arguments={"page_name": "Button"}
```

---

### get_s2_page
Returns the full markdown content for a page, or a specific section if provided.

**Input Schema:**
```json
{
  "page_name": string,       // required, e.g., "Button", "Form"
  "section_name": string     // optional, e.g., "Props", "Examples"
}
```

**Usage:**
```
// Get full page
use_mcp_tool with server_name="React Spectrum (S2)", tool_name="get_s2_page", arguments={"page_name": "Button"}

// Get specific section
use_mcp_tool with server_name="React Spectrum (S2)", tool_name="get_s2_page", arguments={"page_name": "Button", "section_name": "Props"}
```

---

### search_s2_icons
Searches the S2 workflow icon set by one or more terms; returns matching icon names.

**Input Schema:**
```json
{
  "terms": string | string[]  // required, e.g., "calendar" or ["arrow", "right"]
}
```

**Usage:**
```
use_mcp_tool with server_name="React Spectrum (S2)", tool_name="search_s2_icons", arguments={"terms": "calendar"}
```

---

### search_s2_illustrations
Searches the S2 illustrations set by one or more terms; returns matching illustration names.

**Input Schema:**
```json
{
  "terms": string | string[]  // required
}
```

**Usage:**
```
use_mcp_tool with server_name="React Spectrum (S2)", tool_name="search_s2_illustrations", arguments={"terms": "error"}
```

---

### get_style_macro_property_values
Returns the allowed values for a given S2 style macro property.

**Input Schema:**
```json
{
  "propertyName": string  // required, e.g., "backgroundColor", "padding"
}
```

**Usage:**
```
use_mcp_tool with server_name="React Spectrum (S2)", tool_name="get_style_macro_property_values", arguments={"propertyName": "backgroundColor"}
```

---

## Commonly Used Components

| Component | Description | Page Name |
|-----------|-------------|-----------|
| Button | Action buttons | `Button` |
| TextField | Text input fields | `TextField` |
| Form | Form containers | `Form` |
| Picker | Dropdown selection | `Picker` |
| Checkbox | Checkbox inputs | `Checkbox` |
| RadioGroup | Radio button groups | `RadioGroup` |
| Dialog | Modal dialogs | `Dialog` |
| Menu | Dropdown menus | `Menu` |
| Tabs | Tab navigation | `Tabs` |
| Table | Data tables | `TableView` |
| Card | Card containers | `Card` |
| Badge | Status badges | `Badge` |
| Alert | Inline alerts | `InlineAlert` |
| Toast | Toast notifications | `Toast` |
| DatePicker | Date selection | `DatePicker` |
| Link | Navigation links | `Link` |
| Flex/View | Layout components | See Styling page |

---

## Workflow for Building Components

1. **Get component documentation:**
   ```
   get_s2_page("ComponentName")
   ```

2. **Get specific props/examples:**
   ```
   get_s2_page("ComponentName", "Props")
   get_s2_page("ComponentName", "Examples")
   ```

3. **Search for icons:**
   ```
   search_s2_icons("searchTerm")
   ```

4. **Get style values:**
   ```
   get_style_macro_property_values("propertyName")
   ```

---

## Key Pages for Reference

- `Getting started` - Installation and setup
- `Provider` - App wrapper for theming and locale
- `Styling` - Style macro usage
- `Style Macro` - Available style properties
- `Forms` - Form integration patterns
- `Client Side Routing` - Router integration
- `Collections` - List/grid patterns
- `Selection` - Selection patterns

---

## Example: Building a Login Form

```
1. get_s2_page("Form")           // Form container
2. get_s2_page("TextField")      // Email/password inputs
3. get_s2_page("Button")         // Submit button
4. get_s2_page("InlineAlert")    // Error display
5. search_s2_icons("user")       // User icon