<page>
---
title: App
description: >-
  The App API provides information about the app and the status of its
  extensions.

  The API returns information about two types of extensions:

- **UI extensions** (`ui_extension`): Admin, Checkout, Customer Account and
  Point of Sale extensions

- **Theme app extensions** (`theme_app_extension`): Theme app blocks and
  embeds
api_name: app-home
source_url:
  html: '<https://shopify.dev/docs/api/app-home/apis/app>'
  md: '<https://shopify.dev/docs/api/app-home/apis/app.md>'

---

# App

The App API provides information about the app and the status of its extensions.

The API returns information about two types of extensions:

- **UI extensions** (`ui_extension`): Admin, Checkout, Customer Account and Point of Sale extensions
- **Theme app extensions** (`theme_app_extension`): Theme app blocks and embeds

## Extensions method

The `app.extensions()` method asynchronously retrieves detailed information about the app's extensions, including which targets they are activated on.

It returns a Promise that resolves to an array of `ExtensionInfo` objects. Each object contains:

- `handle`: The unique identifier for the extension
- `type`: Either `'ui_extension'` or `'theme_app_extension'`
- `activations`: Activation records (shape varies by extension type)

**UI Extensions** have activations with a `target` field indicating the admin, checkout, customer account or point of sale target.

**Theme App Extensions** have activations representing individual blocks/embeds, each with:

- `handle`: Block/embed filename
- `name`: Display name from block schema
- `target`: Location type (`'section'`, `'head'`, `'body'`, or `'compliance_head'`)
- `status`: Availability status (`'active'`, `'available'`, or `'unavailable'`)
- `activations`: Array of theme-specific placements with `target` and `themeId`

The array may be empty if the app has no extensions.

- **activations**

  **Type extends "ui\_extension" ? UiExtensionActivation\[] : Type extends "theme\_app\_extension" ? ThemeExtensionActivation\[] : never**

  **required**

  List of activation records for the extension. The shape depends on the extension type:

  - UI extensions have activations with only `target`
  - Theme app extensions have nested activations representing blocks/embeds

- **handle**

  **string**

  **required**

  The unique identifier for the extension.

- **type**

  **Type**

  **required**

  The type of the extension.

### UiExtensionActivation

Represents an activation record for a UI extension (checkout, customer account).

- target

  The target identifier for the extension activation. Example: 'purchase.thank-you.block.render'

  ```ts
  string
  ```

```ts
export interface UiExtensionActivation {
  /**
   * The target identifier for the extension activation.
   * Example: 'purchase.thank-you.block.render'
   */
  target: string;
}
```

### ThemeExtensionActivation

Represents an activation record for a theme app block or embed. Each block/embed within a theme app extension has its own handle, status, and activations.

- activations

  List of theme-specific activations for this block/embed. Contains where the block is actually placed within themes.

  ```ts
  ThemeAppBlockActivation[]
  ```

- handle

  The filename of the block/embed within the theme app extension (without extension). This is configured by the developer when creating the block file.

  ```ts
  string
  ```

- name

  The developer-configured display name of this block/embed, defined in the block's schema.

  ```ts
  string
  ```

- status

  The availability status of this block/embed.

  ```ts
  ActivationStatus
  ```

- target

  The target location type for this block/embed. - 'section' for blocks - 'head', 'body', or 'compliance\_head' for embeds

  ```ts
  ThemeAppBlockTarget | ThemeAppEmbedTarget
  ```

```ts
export interface ThemeExtensionActivation {
  /**
   * The target location type for this block/embed.
   * - 'section' for blocks
   * - 'head', 'body', or 'compliance_head' for embeds
   */
  target: ThemeAppBlockTarget | ThemeAppEmbedTarget;

  /**
   * The filename of the block/embed within the theme app extension (without extension).
   * This is configured by the developer when creating the block file.
   */
  handle: string;

  /**
   * The developer-configured display name of this block/embed, defined in the block's schema.
   * @see https://shopify.dev/docs/apps/build/online-store/theme-app-extensions/configuration#schema
   */
  name: string;

  /**
   * The availability status of this block/embed.
   */
  status: ActivationStatus;

  /**
   * List of theme-specific activations for this block/embed.
   * Contains where the block is actually placed within themes.
   */
  activations: ThemeAppBlockActivation[];
}
```

### ThemeAppBlockActivation

Represents a theme-specific activation for a block/embed. Contains the specific placement within a theme.

- target

  The target identifier for the block/embed placement within the theme. Example: 'template--product.alternate/main/my\_app\_product\_rating\_GPzUYy'

  ```ts
  string
  ```

- themeId

  The theme ID where this block/embed is activated. Format: gid://shopify/OnlineStoreTheme/{id}

  ```ts
  string
  ```

```ts
export interface ThemeAppBlockActivation {
  /**
   * The target identifier for the block/embed placement within the theme.
   * Example: 'template--product.alternate/main/my_app_product_rating_GPzUYy'
   */
  target: string;

  /**
   * The theme ID where this block/embed is activated.
   * Format: gid://shopify/OnlineStoreTheme/{id}
   */
  themeId: string;
}
```

### ActivationStatus

The availability status of a theme app block or embed. - 'active': Block/embed is currently present in a theme. - 'available': Block/embed exists but is not currently active - 'unavailable': Block/embed exists but is disabled (e.g., via available\_if condition) Note that if a block is present in a theme but is not available, its status will be 'unavailable'.

```ts
'active' | 'available' | 'unavailable'
```

### ThemeAppBlockTarget

Target location for theme app blocks.

```ts
'section'
```

### ThemeAppEmbedTarget

Target location for theme app embeds.

```ts
'head' | 'body' | 'compliance_head'
```

Examples

### Examples

- #### Get Extensions Status

  ##### Default

  ```js
  const extensions = await shopify.app.extensions();

  // Example response:
  // [
  //   // UI extension
  //   {
  //     handle: 'checkout-extension-1',
  //     type: 'ui_extension',
  //     activations: [
  //       {target: 'purchase.thank-you.block.render'},
  //       {target: 'customer-account.order-status.block.render'},
  //     ],
  //   },
  //   // Theme app extension with nested blocks/embeds
  //   {
  //     handle: 'my-theme-app-extension',
  //     type: 'theme_app_extension',
  //     activations: [
  //       {
  //         target: 'section',
  //         handle: 'product-rating',
  //         name: 'Product Rating',
  //         status: 'active',
  //         activations: [
  //           {
  //             target: 'template--product.custom/main/my_app_product_rating_GPzUYy',
  //             themeId: 'gid://shopify/OnlineStoreTheme/123',
  //           },
  //         ],
  //       },
  //       {
  //         target: 'head',
  //         handle: 'analytics-widget',
  //         name: 'Analytics Widget',
  //         status: 'active',
  //         activations: [
  //           {target: 'theme', themeId: 'gid://shopify/OnlineStoreTheme/123'},
  //         ],
  //       },
  //     ],
  //   },
  // ];
  ```

</page>

<page>
---
title: Config
description: >-
  The <code>config</code> API stores the initial configuration information for
  your app and lets you synchronously retrieve it.
api_name: app-home
source_url:
  html: 'https://shopify.dev/docs/api/app-home/apis/config'
  md: 'https://shopify.dev/docs/api/app-home/apis/config.md'
---

# Config

The `config` API stores the initial configuration information for your app and lets you synchronously retrieve it.

## Config

The `config` API is available on the `shopify` global. It stores the initial configuration information for your app and shop.

- **apiKey**

  **string**

  **required**

  The client ID provided for your application in the Partner Dashboard.

  This needs to be provided by the app developer.

- **appOrigins**

  **string\[]**

  An allowlist of origins that your app can send authenticated fetch requests to.

  This is useful if your app needs to make authenticated requests to a different domain that you control.

- **debug**

  **DebugOptions**

  Configuration options for enabling debug features within the app. Includes options for monitoring performance metrics, such as web vitals.

  Recommended for use during development and debugging to aid in identifying and resolving performance issues.

  Generally not recommended for long-term use in production environments.

- **disabledFeatures**

  **string\[]**

  The features to disable in your app.

  This allows app developers to opt-out of features such as `fetch`.

- **experimentalFeatures**

  **string\[]**

  The experimental features to enable in your app.

  This allows app developers to opt-in to experiement features.

- **host**

  **string**

  The base64-encoded host of the shop that's embedding your app.

  This does not need to be provided by the app developer.

- **locale**

  **string**

  **Default: 'en-US'**

  The locale of the shop that's embedding your app.

  This does not need to be provided by the app developer.

- **shop**

  **string**

  The shop origin of the shop that's embedding your app.

  This does not need to be provided by the app developer.

### DebugOptions

- webVitals

  Enables or disables the logging of web performance metrics (Web Vitals) in the browser's console. When set to \`true\`, the app will log Core Web Vitals (such as LCP, INP, and CLS) and other relevant performance metrics to help developers understand the real-world performance of their app. This can be useful for debugging performance issues during development or in a staging environment. This field is optional and defaults to \`false\`, meaning that web vitals logging is disabled by default to avoid performance overhead and unnecessary console output in production environments.

  ```ts
  boolean
  ```

```ts
interface DebugOptions {
  /**
   * Enables or disables the logging of web performance metrics (Web Vitals) in the browser's console.
   *
   * When set to `true`, the app will log Core Web Vitals (such as LCP, INP, and CLS) and other relevant performance metrics to help developers understand the real-world performance of their app. This can be useful for debugging performance issues during development or in a staging environment.
   *
   * This field is optional and defaults to `false`, meaning that web vitals logging is disabled by default to avoid performance overhead and unnecessary console output in production environments.
   * @defaultValue false
   */
  webVitals?: boolean;
}
```

Examples

### Examples

- #### Shop

  ##### Default

  ```js
  shopify.config.shop;
  // => 'your-shop-name.myshopify.com'
  ```

- #### shop

  ##### Description

  Retrieving the shop origin

  ##### Default

  ```js
  shopify.config.shop;
  // => 'your-shop-name.myshopify.com'
  ```

- #### host

  ##### Description

  Retrieving the host

  ##### Default

  ```js
  shopify.config.host;
  ```

- #### locale

  ##### Description

  Retrieving the locale

  ##### Default

  ```js
  shopify.config.locale;
  ```

- #### apiKey

  ##### Description

  Retrieving the apiKey

  ##### Default

  ```js
  shopify.config.apiKey;
  ```

- #### disabledFeatures

  ##### Description

  Retrieving the disabledFeatures

  ##### Default

  ```js
  shopify.config.disabledFeatures;
  ```

- #### appOrigins

  ##### Description

  Retrieving the appOrigins

  ##### Default

  ```js
  shopify.config.appOrigins;
  ```

- #### debug

  ##### Description

  Configuration for debugging apps.

  ##### Default

  ```js
  shopify.config.debug;
  // => { webVitals: false }
  ```

- #### apiKey

  ##### Description

  Setting the apiKey

  ##### meta tag

  ```html
  <head>
    <meta name="shopify-api-key" content="%SHOPIFY_API_KEY%" />

    <script src="https://cdn.shopify.com/shopifycloud/app-bridge.js" />
  </head>
  ```

- #### disabledFeatures

  ##### Description

  Setting the disabledFeatures

  ##### single feature

  ```html
  <head>
    <meta name="shopify-disabled-features" content="fetch" />

    <meta name="shopify-api-key" content="%SHOPIFY_API_KEY%" />
    <script src="https://cdn.shopify.com/shopifycloud/app-bridge.js" />
  </head>
  ```

  ##### multiple features

  ```html
  <head>
    <meta name="shopify-disabled-features" content="fetch, auto-redirect" />

    <meta name="shopify-api-key" content="%SHOPIFY_API_KEY%" />
    <script src="https://cdn.shopify.com/shopifycloud/app-bridge.js" />
  </head>
  ```

- #### appOrigins

  ##### Description

  Setting the appOrigins

  ##### single origin

  ```html
  <head>
    <meta name="shopify-app-origins" content="https://example.com" />

    <meta name="shopify-api-key" content="%SHOPIFY_API_KEY%" />
    <script src="https://cdn.shopify.com/shopifycloud/app-bridge.js" />
  </head>
  ```

  ##### multiple origins

  ```html
  <head>
    <meta
      name="shopify-app-origins"
      content="https://example.com,https://example.net"
    />

    <meta name="shopify-api-key" content="%SHOPIFY_API_KEY%" />
    <script src="https://cdn.shopify.com/shopifycloud/app-bridge.js" />
  </head>
  ```

- #### debug

  ##### Description

  Enabling Debug Features for Performance Monitoring

  ##### Web Vitals

  ```html
  <head>
    <meta name="shopify-debug" content="web-vitals" />

    <meta name="shopify-api-key" content="%SHOPIFY_API_KEY%" />
    <script src="https://cdn.shopify.com/shopifycloud/app-bridge.js" />
  </head>
  ```

</page>

<page>
---
title: Environment
description: >-
  The Environment API provides utilities for information regarding the
  environment an App Home is running on.
api_name: app-home
source_url:
  html: 'https://shopify.dev/docs/api/app-home/apis/environment'
  md: 'https://shopify.dev/docs/api/app-home/apis/environment.md'
---

# Environment

The Environment API provides utilities for information regarding the environment an App Home is running on.

## Environment

The `environment` API is available on the `shopify` global. It contains information about the current environment an App Home is running on.

- **embedded**

  **boolean**

  Whether the app is embedded in the Shopify admin.

- **mobile**

  **boolean**

  Whether the app is running on Shopify Mobile.

- **pos**

  **boolean**

  Whether the app is running on Shopify POS.

Examples

### Examples

- #### Environment

  ##### Default

  ```js
  shopify.environment;
  // => { mobile: false, embedded: true, pos: false }
  ```

</page>

<page>
---
title: ID Token
description: >-
  The ID token API asynchronously retrieves an [OpenID Connect ID
  Token](https://openid.net/specs/openid-connect-core-1_0.html#IDToken%5C) from
  Shopify that can be used to ensure that requests came from a Shopify
  authenticated user. See the [ID Token
  documentation](/docs/apps/auth/oauth/session-tokens) from more information.
api_name: app-home
source_url:
  html: 'https://shopify.dev/docs/api/app-home/apis/id-token'
  md: 'https://shopify.dev/docs/api/app-home/apis/id-token.md'
---

# ID Token

The ID token API asynchronously retrieves an [OpenID Connect ID Token](https://openid.net/specs/openid-connect-core-1_0.html#IDToken%5C) from Shopify that can be used to ensure that requests came from a Shopify authenticated user. See the [ID Token documentation](https://shopify.dev/docs/apps/auth/oauth/session-tokens) from more information.

## ID Token()

The `idToken` API is available on the `shopify` global. It asynchronously retrieves an OpenID Connect ID Token from Shopify.

### Returns

- **Promise\<string>**

Examples

### Examples

- #### ID Token

  ##### Default

  ```js
  await shopify.idToken();
  ```

</page>

<page>
---
title: Intents
description: >-
  The Intents API provides a way to invoke existing admin workflows for
  creating, editing, and managing Shopify resources.
api_name: app-home
source_url:
  html: 'https://shopify.dev/docs/api/app-home/apis/intents'
  md: 'https://shopify.dev/docs/api/app-home/apis/intents.md'
---

# Intents

The Intents API provides a way to invoke existing admin workflows for creating, editing, and managing Shopify resources.

## invoke

The `invoke` API is a function that accepts either a string query or an options object describing the intent to invoke and returns a Promise that resolves to an activity handle for the workflow.

## Intent Format

Intents are invoked using a string query format: `${action}:${type},${value}`

Where:

- `action` - The operation to perform (`create` or `edit`)
- `type` - The resource type (e.g., `shopify/Product`)
- `value` - The resource identifier (only for edit actions)

## Supported Resources

### Article

| Action | Type | Value | Data |
| - | - | - | - |
| `create` | `shopify/Article` | — | — |
| `edit` | `shopify/Article` | `gid://shopify/Article/{id}` | — |

### Catalog

| Action | Type | Value | Data |
| - | - | - | - |
| `create` | `shopify/Catalog` | — | — |
| `edit` | `shopify/Catalog` | `gid://shopify/Catalog/{id}` | — |

### Collection

| Action | Type | Value | Data |
| - | - | - | - |
| `create` | `shopify/Collection` | — | — |
| `edit` | `shopify/Collection` | `gid://shopify/Collection/{id}` | — |

### Customer

| Action | Type | Value | Data |
| - | - | - | - |
| `create` | `shopify/Customer` | — | — |
| `edit` | `shopify/Customer` | `gid://shopify/Customer/{id}` | — |

### Discount

| Action | Type | Value | Data |
| - | - | - | - |
| `create` | `shopify/Discount` | — | `{ type: 'amount-off-product' \| 'amount-off-order' \| 'buy-x-get-y' \| 'free-shipping' }` |
| `edit` | `shopify/Discount` | `gid://shopify/Discount/{id}` | — |

### Market

| Action | Type | Value | Data |
| - | - | - | - |
| `create` | `shopify/Market` | — | — |
| `edit` | `shopify/Market` | `gid://shopify/Market/{id}` | — |

### Menu

| Action | Type | Value | Data |
| - | - | - | - |
| `create` | `shopify/Menu` | — | — |
| `edit` | `shopify/Menu` | `gid://shopify/Menu/{id}` | — |

### Metafield Definition

| Action | Type | Value | Data |
| - | - | - | - |
| `create` | `shopify/MetafieldDefinition` | — | `{ ownerType: 'product' }` |
| `edit` | `shopify/MetafieldDefinition` | `gid://shopify/MetafieldDefinition/{id}` | `{ ownerType: 'product' }` |

### Metaobject

| Action | Type | Value | Data |
| - | - | - | - |
| `create` | `shopify/Metaobject` | — | `{ type: 'shopify--color-pattern' }` |
| `edit` | `shopify/Metaobject` | `gid://shopify/Metaobject/{id}` | `{ type: 'shopify--color-pattern' }` |

### Metaobject Definition

| Action | Type | Value | Data |
| - | - | - | - |
| `create` | `shopify/MetaobjectDefinition` | — | — |
| `edit` | `shopify/MetaobjectDefinition` | — | `{ type: 'my_metaobject_definition_type' }` |

### Page

| Action | Type | Value | Data |
| - | - | - | - |
| `create` | `shopify/Page` | — | — |
| `edit` | `shopify/Page` | `gid://shopify/Page/{id}` | — |

### Product

| Action | Type | Value | Data |
| - | - | - | - |
| `create` | `shopify/Product` | — | — |
| `edit` | `shopify/Product` | `gid://shopify/Product/{id}` | — |

### Product Variant

| Action | Type | Value | Data |
| - | - | - | - |
| `create` | `shopify/ProductVariant` | — | `{ productId: 'gid://shopify/Product/{id}' }` |
| `edit` | `shopify/ProductVariant` | `gid://shopify/ProductVariant/{id}` | `{ productId: 'gid://shopify/Product/{id}' }` |

> **Note**: To determine whether to use the `shopify/ProductVariant` `edit` intent or the `shopify/Product` `edit` intent, query the [`product.hasOnlyDefaultVariant`](https://shopify.dev/docs/api/admin-graphql/latest/objects/Product#field-Product.fields.hasOnlyDefaultVariant) field. If the product has only the default variant (`hasOnlyDefaultVariant` is `true`), use the `shopify/Product` `edit` intent.

- **invoke**

  **{ (query: IntentQuery): Promise\<IntentActivity>; (intentURL: string, options?: IntentQueryOptions): Promise\<IntentActivity>; }**

  Invoke an intent using the object syntax.

  Invoke an intent using the URL syntax.

  URL format: `action:type[,value][?params]`.

### IntentQuery

- action

  ```ts
  IntentAction
  ```

- data

  Additional data required for certain intent types. For example: - Discount creation requires { type: 'amount-off-product' | 'amount-off-order' | 'buy-x-get-y' | 'free-shipping' } - ProductVariant creation requires { productId: 'gid://shopify/Product/123' } - Metaobject creation requires { type: 'shopify--color-pattern' }

  ```ts
  { [key: string]: unknown; }
  ```

- type

  ```ts
  IntentType
  ```

- value

  The resource identifier for edit actions (e.g., 'gid://shopify/Product/123').

  ```ts
  string
  ```

```ts
export interface IntentQuery extends IntentQueryOptions {
  action: IntentAction;
  type: IntentType;
}
```

### IntentAction

The action to perform on a resource.

```ts
'create' | 'edit'
```

### IntentType

Supported resource types that can be targeted by intents.

```ts
'shopify/Article' | 'shopify/Catalog' | 'shopify/Collection' | 'shopify/Customer' | 'shopify/Discount' | 'shopify/Market' | 'shopify/Menu' | 'shopify/MetafieldDefinition' | 'shopify/Metaobject' | 'shopify/MetaobjectDefinition' | 'shopify/Page' | 'shopify/Product' | 'shopify/ProductVariant'
```

### IntentActivity

Activity handle for tracking intent workflow progress.

- complete

  A Promise that resolves when the intent workflow completes, returning the response.

  ```ts
  Promise<IntentResponse>
  ```

```ts
export interface IntentActivity {
  /**
   * A Promise that resolves when the intent workflow completes, returning the response.
   */
  complete?: Promise<IntentResponse>;
}
```

### IntentResponse

Result of an intent activity. Discriminated union representing all possible completion outcomes.

```ts
ClosedIntentResponse | SuccessIntentResponse | ErrorIntentResponse
```

### ClosedIntentResponse

User dismissed or closed the workflow without completing it.

- code

  ```ts
  'closed'
  ```

```ts
export interface ClosedIntentResponse {
  code?: 'closed';
}
```

### SuccessIntentResponse

Successful intent completion.

- code

  ```ts
  'ok'
  ```

- data

  ```ts
  { [key: string]: unknown; }
  ```

```ts
export interface SuccessIntentResponse {
  code?: 'ok';
  data?: {[key: string]: unknown};
}
```

### ErrorIntentResponse

Failed intent completion.

- code

  ```ts
  'error'
  ```

- issues

  ```ts
  Issue[]
  ```

- message

  ```ts
  string
  ```

```ts
export interface ErrorIntentResponse {
  code?: 'error';
  message?: string;
  issues?: StandardSchemaV1.Issue[];
}
```

### IntentQueryOptions

Options for invoking intents when using the query string format.

- data

  Additional data required for certain intent types. For example: - Discount creation requires { type: 'amount-off-product' | 'amount-off-order' | 'buy-x-get-y' | 'free-shipping' } - ProductVariant creation requires { productId: 'gid://shopify/Product/123' } - Metaobject creation requires { type: 'shopify--color-pattern' }

  ```ts
  { [key: string]: unknown; }
  ```

- value

  The resource identifier for edit actions (e.g., 'gid://shopify/Product/123').

  ```ts
  string
  ```

```ts
export interface IntentQueryOptions {
  /**
   * The resource identifier for edit actions (e.g., 'gid://shopify/Product/123').
   */
  value?: string;
  /**
   * Additional data required for certain intent types.
   * For example:
   * - Discount creation requires { type: 'amount-off-product' | 'amount-off-order' | 'buy-x-get-y' | 'free-shipping' }
   * - ProductVariant creation requires { productId: 'gid://shopify/Product/123' }
   * - Metaobject creation requires { type: 'shopify--color-pattern' }
   */
  data?: {[key: string]: unknown};
}
```

## IntentAction

Supported actions that can be performed on resources.

- `create`: Opens a creation workflow for a new resource
- `edit`: Opens an editing workflow for an existing resource (requires `value` parameter)

**`'create' | 'edit'`**

## IntentType

Supported resource types that can be targeted by intents.

**`'shopify/Article' | 'shopify/Catalog' | 'shopify/Collection' | 'shopify/Customer' | 'shopify/Discount' | 'shopify/Market' | 'shopify/Menu' | 'shopify/MetafieldDefinition' | 'shopify/Metaobject' | 'shopify/MetaobjectDefinition' | 'shopify/Page' | 'shopify/Product' | 'shopify/ProductVariant'`**

## IntentQueryOptions

Options for invoking intents when using the query string format.

- **data**

  **{ \[key: string]: unknown; }**

  Additional data required for certain intent types. For example:

  - Discount creation requires { type: 'amount-off-product' | 'amount-off-order' | 'buy-x-get-y' | 'free-shipping' }
  - ProductVariant creation requires { productId: 'gid://shopify/Product/123' }
  - Metaobject creation requires { type: 'shopify--color-pattern' }

- **value**

  **string**

  The resource identifier for edit actions (e.g., 'gid://shopify/Product/123').

## IntentResponse

Response object returned when the intent workflow completes.

**`ClosedIntentResponse | SuccessIntentResponse | ErrorIntentResponse`**

### ClosedIntentResponse

- **code**

  **'closed'**

### ErrorIntentResponse

- **code**

  **'error'**

- **issues**

  **Issue\[]**

- **message**

  **string**

### SuccessIntentResponse

- **code**

  **'ok'**

- **data**

  **{ \[key: string]: unknown; }**

### ClosedIntentResponse

User dismissed or closed the workflow without completing it.

- code

  ```ts
  'closed'
  ```

```ts
export interface ClosedIntentResponse {
  code?: 'closed';
}
```

### SuccessIntentResponse

Successful intent completion.

- code

  ```ts
  'ok'
  ```

- data

  ```ts
  { [key: string]: unknown; }
  ```

```ts
export interface SuccessIntentResponse {
  code?: 'ok';
  data?: {[key: string]: unknown};
}
```

### ErrorIntentResponse

Failed intent completion.

- code

  ```ts
  'error'
  ```

- issues

  ```ts
  Issue[]
  ```

- message

  ```ts
  string
  ```

```ts
export interface ErrorIntentResponse {
  code?: 'error';
  message?: string;
  issues?: StandardSchemaV1.Issue[];
}
```

Examples

## Preview

![](https://cdn.shopify.com/shopifycloud/shopify-dev/development/assets/assets/images/templated-apis-screenshots/admin/apis/intents-bqfuEvyn.png)

### Examples

- #### Creating a collection

  ##### Default

  ```js
  const activity = await shopify.intents.invoke('create:shopify/Collection');

  const response = await activity.complete;

  if (response.code === 'ok') {
    console.log('Collection created:', response.data);
  }
  ```

- #### Create article

  ##### Description

  Create a new article. Opens the article creation workflow.

  ##### Default

  ```js
  const activity = await shopify.intents.invoke('create:shopify/Article');

  const response = await activity.complete;

  if (response.code === 'ok') {
    console.log('Article created:', response.data);
  }
  ```

- #### Edit article

  ##### Description

  Edit an existing article. Requires an article GID.

  ##### Default

  ```js
  const activity = await shopify.intents.invoke('edit:shopify/Article', {
    value: 'gid://shopify/Article/123456789',
  });

  const response = await activity.complete;

  if (response.code === 'ok') {
    console.log('Article updated:', response.data);
  }
  ```

- #### Create catalog

  ##### Description

  Create a new catalog. Opens the catalog creation workflow.

  ##### Default

  ```js
  const activity = await shopify.intents.invoke('create:shopify/Catalog');

  const response = await activity.complete;

  if (response.code === 'ok') {
    console.log('Catalog created:', response.data);
  }
  ```

- #### Edit catalog

  ##### Description

  Edit an existing catalog. Requires a catalog GID.

  ##### Default

  ```js
  const activity = await shopify.intents.invoke('edit:shopify/Catalog', {
    value: 'gid://shopify/Catalog/123456789',
  });

  const response = await activity.complete;

  if (response.code === 'ok') {
    console.log('Catalog updated:', response.data);
  }
  ```

- #### Create collection

  ##### Description

  Create a new collection. Opens the collection creation workflow.

  ##### Default

  ```js
  const activity = await shopify.intents.invoke('create:shopify/Collection');

  const response = await activity.complete;

  if (response.code === 'ok') {
    console.log('Collection created:', response.data);
  }
  ```

- #### Edit collection

  ##### Description

  Edit an existing collection. Requires a collection GID.

  ##### Default

  ```js
  const activity = await shopify.intents.invoke('edit:shopify/Collection', {
    value: 'gid://shopify/Collection/987654321',
  });

  const response = await activity.complete;

  if (response.code === 'ok') {
    console.log('Collection updated:', response.data);
  }
  ```

- #### Create customer

  ##### Description

  Create a new customer. Opens the customer creation workflow.

  ##### Default

  ```js
  const activity = await shopify.intents.invoke('create:shopify/Customer');

  const response = await activity.complete;

  if (response.code === 'ok') {
    console.log('Customer created:', response.data);
  }
  ```

- #### Edit customer

  ##### Description

  Edit an existing customer. Requires a customer GID.

  ##### Default

  ```js
  const activity = await shopify.intents.invoke('edit:shopify/Customer', {
    value: 'gid://shopify/Customer/456789123',
  });

  const response = await activity.complete;

  if (response.code === 'ok') {
    console.log('Customer updated:', response.data);
  }
  ```

- #### Create discount

  ##### Description

  Create a new discount. Opens the discount creation workflow. Requires a discount type.

  ##### Default

  ```js
  const activity = await shopify.intents.invoke('create:shopify/Discount', {
    data: {type: 'amount-off-product'},
  });

  const response = await activity.complete;

  if (response.code === 'ok') {
    console.log('Discount created:', response.data);
  }
  ```

- #### Edit discount

  ##### Description

  Edit an existing discount. Requires a discount GID.

  ##### Default

  ```js
  const activity = await shopify.intents.invoke('edit:shopify/Discount', {
    value: 'gid://shopify/Discount/123456789',
  });

  const response = await activity.complete;

  if (response.code === 'ok') {
    console.log('Discount updated:', response.data);
  }
  ```

- #### Create market

  ##### Description

  Create a new market. Opens the market creation workflow.

  ##### Default

  ```js
  const activity = await shopify.intents.invoke('create:shopify/Market');

  const response = await activity.complete;

  if (response.code === 'ok') {
    console.log('Market created:', response.data);
  }
  ```

- #### Edit market

  ##### Description

  Edit an existing market. Requires a market GID.

  ##### Default

  ```js
  const activity = await shopify.intents.invoke('edit:shopify/Market', {
    value: 'gid://shopify/Market/123456789',
  });

  const response = await activity.complete;

  if (response.code === 'ok') {
    console.log('Market updated:', response.data);
  }
  ```

- #### Create menu

  ##### Description

  Create a new menu. Opens the menu creation workflow.

  ##### Default

  ```js
  const activity = await shopify.intents.invoke('create:shopify/Menu');

  const response = await activity.complete;

  if (response.code === 'ok') {
    console.log('Menu created:', response.data);
  }
  ```

- #### Edit menu

  ##### Description

  Edit an existing menu. Requires a menu GID.

  ##### Default

  ```js
  const activity = await shopify.intents.invoke('edit:shopify/Menu', {
    value: 'gid://shopify/Menu/123456789',
  });

  const response = await activity.complete;

  if (response.code === 'ok') {
    console.log('Menu updated:', response.data);
  }
  ```

- #### Create metafield definition

  ##### Description

  Create a new metafield definition. Opens the metafield definition creation workflow.

  ##### Default

  ```js
  const activity = await shopify.intents.invoke(
    'create:shopify/MetafieldDefinition',
    {data: {ownerType: 'product'}},
  );

  const response = await activity.complete;

  if (response.code === 'ok') {
    console.log('Metafield definition created:', response.data);
  }
  ```

- #### Edit metafield definition

  ##### Description

  Edit an existing metafield definition. Requires a metafield definition GID.

  ##### Default

  ```js
  const activity = await shopify.intents.invoke(
    'edit:shopify/MetafieldDefinition',
    {
      value: 'gid://shopify/MetafieldDefinition/123456789',
      data: {ownerType: 'product'},
    },
  );

  const response = await activity.complete;

  if (response.code === 'ok') {
    console.log('Metafield definition updated:', response.data);
  }
  ```

- #### Create metaobject

  ##### Description

  Create a new metaobject. Opens the metaobject creation workflow. Requires a type.

  ##### Default

  ```js
  const activity = await shopify.intents.invoke('create:shopify/Metaobject', {
    data: {type: 'shopify--color-pattern'},
  });

  const response = await activity.complete;

  if (response.code === 'ok') {
    console.log('Metaobject created:', response.data);
  }
  ```

- #### Edit metaobject

  ##### Description

  Edit an existing metaobject. Requires a metaobject GID.

  ##### Default

  ```js
  const activity = await shopify.intents.invoke('edit:shopify/Metaobject', {
    value: 'gid://shopify/Metaobject/123456789',
    data: {type: 'shopify--color-pattern'},
  });

  const response = await activity.complete;

  if (response.code === 'ok') {
    console.log('Metaobject updated:', response.data);
  }
  ```

- #### Create metaobject definition

  ##### Description

  Create a new metaobject definition. Opens the metaobject definition creation workflow.

  ##### Default

  ```js
  const activity = await shopify.intents.invoke(
    'create:shopify/MetaobjectDefinition',
  );

  const response = await activity.complete;

  if (response.code === 'ok') {
    console.log('Metaobject definition created:', response.data);
  }
  ```

- #### Edit metaobject definition

  ##### Description

  Edit an existing metaobject definition. Requires a metaobject definition GID.

  ##### Default

  ```js
  const activity = await shopify.intents.invoke(
    'edit:shopify/MetaobjectDefinition',
    data: {type: 'my_metaobject_definition_type'},
  );

  const response = await activity.complete;

  if (response.code === 'ok') {
    console.log('Metaobject definition updated:', response.data);
  }
  ```

- #### Create page

  ##### Description

  Create a new page. Opens the page creation workflow.

  ##### Default

  ```js
  const activity = await shopify.intents.invoke('create:shopify/Page');

  const response = await activity.complete;

  if (response.code === 'ok') {
    console.log('Page created:', response.data);
  }
  ```

- #### Edit page

  ##### Description

  Edit an existing page. Requires a page GID.

  ##### Default

  ```js
  const activity = await shopify.intents.invoke('edit:shopify/Page', {
    value: 'gid://shopify/Page/123456789',
  });

  const response = await activity.complete;

  if (response.code === 'ok') {
    console.log('Page updated:', response.data);
  }
  ```

- #### Create product

  ##### Description

  Create a new product. Opens the product creation workflow.

  ##### Default

  ```js
  const activity = await shopify.intents.invoke('create:shopify/Product');

  const response = await activity.complete;

  if (response.code === 'ok') {
    console.log('Product created:', response.data);
  }
  ```

- #### Edit product

  ##### Description

  Edit an existing product. Requires a product GID.

  ##### Default

  ```js
  const activity = await shopify.intents.invoke('edit:shopify/Product', {
    value: 'gid://shopify/Product/123456789',
  });

  const response = await activity.complete;

  if (response.code === 'ok') {
    console.log('Product updated:', response.data);
  }
  ```

- #### Create variant

  ##### Description

  Create a new product variant. Opens the variant creation workflow. Requires a product ID.

  ##### Default

  ```js
  const activity = await shopify.intents.invoke('create:shopify/ProductVariant', {
    data: {productId: 'gid://shopify/Product/123456789'},
  });

  const response = await activity.complete;

  if (response.code === 'ok') {
    console.log('Product variant created:', response.data);
  }
  ```

- #### Edit variant

  ##### Description

  Edit an existing product variant. Requires a variant GID.

  ##### Default

  ```js
  const activity = await shopify.intents.invoke('edit:shopify/ProductVariant', {
    value: 'gid://shopify/ProductVariant/123456789',
    data: {productId: 'gid://shopify/Product/123456789'},
  });

  const response = await activity.complete;

  if (response.code === 'ok') {
    console.log('Product variant updated:', response.data);
  }
  ```

</page>

<page>
---
title: Loading
description: >-
  The Loading API indicates to users that a page is loading or an upload is
  processing.
api_name: app-home
source_url:
  html: 'https://shopify.dev/docs/api/app-home/apis/loading'
  md: 'https://shopify.dev/docs/api/app-home/apis/loading.md'
---

# Loading

The Loading API indicates to users that a page is loading or an upload is processing.

## Loading API(**[isLoading](#loadingapi-propertydetail-isloading)**​)

The `Loading` API is available on the `shopify` global. It displays a loading indicator in the Shopify admin.

### Parameters

- **isLoading**

  **boolean**

### Returns**void**

Examples

## Preview

![](https://cdn.shopify.com/shopifycloud/shopify-dev/development/assets/assets/images/templated-apis-screenshots/admin/apis/loading-COZFMV2R.png)

### Examples

- #### Loading

  ##### Default

  ```js
  shopify.loading(true);
  // ...
  shopify.loading(false);
  ```

</page>

<page>
---
title: Modal API
description: >-
  The Modal API allows you to display an overlay that prevents interaction with
  the rest of the app until dismissed.
api_name: app-home
source_url:
  html: 'https://shopify.dev/docs/api/app-home/apis/modal-api'
  md: 'https://shopify.dev/docs/api/app-home/apis/modal-api.md'
---

# Modal API

The Modal API allows you to display an overlay that prevents interaction with the rest of the app until dismissed.

## Modal

The `modal` API provides a `show` method to display a Modal, a `hide` method to hide a Modal, and a `toggle` method to toggle the visibility of a Modal. These are used in conjunction with the [`ui-modal` element](https://shopify.dev/docs/api/app-bridge-library/web-components/ui-modal). They are alternatives to the `show`, `hide`, and `toggle` instance methods.

- **hide**

  **(id: string) => Promise\<void>**

  Hides the modal element. An alternative to the `hide` instance method on the `ui-modal` element.

- **show**

  **(id: string) => Promise\<void>**

  Shows the modal element. An alternative to the `show` instance method on the `ui-modal` element.

- **toggle**

  **(id: string) => Promise\<void>**

  Toggles the modal element visibility. An alternative to the `toggle` instance method on the `ui-modal` element.

Examples

## Preview

![](https://cdn.shopify.com/shopifycloud/shopify-dev/development/assets/assets/images/apps/tools/app-bridge-modal-BUQeTJIG.png)

### Examples

- #### Modal

  ##### Modal

  ```html
  <ui-modal id="my-modal">
    <p>Hello, World!</p>
  </ui-modal>

  <button onclick="shopify.modal.show('my-modal')">Open Modal</button>
  ```

## Related

[Component - ui-modal](https://shopify.dev/docs/api/app-bridge-library/web-components/ui-modal)

[Component - Modal](https://shopify.dev/docs/api/app-bridge-library/react-components/modal-component)

</page>

<page>
---
title: Navigation
description: >-
  The Navigation API allows you navigate within and outside of your app using
  the [HTML anchor
  element](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/a). It also
  allows you to modify the top-level browser URL with or without navigating. It
  does this through the [History
  API](https://developer.mozilla.org/en-US/docs/Web/API/History_API) and the
  [Navigation
  API](https://developer.mozilla.org/en-US/docs/Web/API/Navigation_API).
api_name: app-home
source_url:
  html: 'https://shopify.dev/docs/api/app-home/apis/navigation'
  md: 'https://shopify.dev/docs/api/app-home/apis/navigation.md'
---

# Navigation

The Navigation API allows you navigate within and outside of your app using the [HTML anchor element](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/a). It also allows you to modify the top-level browser URL with or without navigating. It does this through the [History API](https://developer.mozilla.org/en-US/docs/Web/API/History_API) and the [Navigation API](https://developer.mozilla.org/en-US/docs/Web/API/Navigation_API).

Examples

### Examples

- #### anchor

  ##### Default

  ```html
  <a href="shopify://admin/products" target="_top">Products page</a>
  ```

- #### App navigation with relative path

  ##### Description

  Navigating to relative path within your app

  ##### HTML

  ```html
  <a href="/settings">Settings</a>
  ```

  ##### JavaScript

  ```js
  open('/settings', '_self');
  ```

- #### External URL in same window

  ##### Description

  Navigating to external URL in same window

  ##### HTML

  ```html
  <a href="https://example.com">Settings</a>
  ```

  ##### JavaScript

  ```js
  open('https://example.com', '_top');
  ```

- #### External URL in new window

  ##### Description

  Navigating to external URL in new window

  ##### HTML

  ```html
  <a href="https://example.com" target="_blank">Settings</a>
  ```

  ##### JavaScript

  ```js
  open('https://example.com', '_blank');
  ```

- #### /products page

  ##### Description

  Navigating to /products page

  ##### HTML

  ```html
  <a href="shopify://admin/products" target="_top">Products page</a>
  ```

  ##### JavaScript

  ```js
  open('shopify://admin/products', '_top');
  ```

- #### /products page with resource

  ##### Description

  Navigating to /products page with specific resource

  ##### HTML

  ```html
  <a href="shopify://admin/products/123" target="_top">Products page</a>
  ```

  ##### JavaScript

  ```js
  open('shopify://admin/products/123', '_top');
  ```

- #### /customers page

  ##### Description

  Navigating to /customers page

  ##### HTML

  ```html
  <a href="shopify://admin/customers" target="_top">Customers page</a>
  ```

  ##### JavaScript

  ```js
  open('shopify://admin/customers', '_top');
  ```

- #### /orders page

  ##### Description

  Navigating to /orders page

  ##### HTML

  ```html
  <a href="shopify://admin/orders" target="_top">Orders page</a>
  ```

  ##### JavaScript

  ```js
  open('shopify://admin/orders', '_top');
  ```

- #### History API

  ##### Description

  Using the \[History API]\(<https://developer.mozilla.org/en-US/docs/Web/API/History\_API>)

  ##### pushState

  ```js
  history.pushState(null, '', '/settings');
  ```

  ##### replaceState

  ```js
  history.replaceState(null, '', '/settings');
  ```

- #### Navigation API

  ##### Description

  Using the \[Navigation API]\(<https://developer.mozilla.org/en-US/docs/Web/API/Navigation\_API>)

  ##### pushState

  ```js
  navigation.navigate('/settings', {
    history: 'push',
  });
  ```

  ##### replaceState

  ```js
  navigation.navigate('/settings', {
    history: 'replace',
  });
  ```

</page>

<page>
---
title: Picker
description: >
  The Picker API provides a search-based interface to help users find and select
  one or more resources that you provide, such as product reviews, email
  templates, or subscription options, and then returns the selected resource
  `id`s to your app.

  > Tip:

  > If you are picking products, product variants, or collections, you should
  use the [Resource Picker](resource-picker) API instead.
api_name: app-home
source_url:
  html: '<https://shopify.dev/docs/api/app-home/apis/picker>'
  md: '<https://shopify.dev/docs/api/app-home/apis/picker.md>'
---

# Picker

The Picker API provides a search-based interface to help users find and select one or more resources that you provide, such as product reviews, email templates, or subscription options, and then returns the selected resource `id`s to your app.

**Tip:** If you are picking products, product variants, or collections, you should use the \<a href="resource-picker">Resource Picker\</a> API instead.

## picker(**[options](#picker-propertydetail-options)**​)

The `picker` API is a function that accepts an options argument for configuration and returns a Promise that resolves to the picker instance once the picker modal is opened.

### Parameters

- **options**

  **PickerOptions**

  **required**

### Returns

- **Promise\<PickerInstance>**

### PickerOptions

- headers

  The data headers for the picker. These are used to display the table headers in the picker modal.

  ```ts
  Header[]
  ```

- heading

  The heading of the picker. This is displayed in the title of the picker modal.

  ```ts
  string
  ```

- items

  The items to display in the picker. These are used to display the rows in the picker modal.

  ```ts
  PickerItem[]
  ```

- multiple

  Whether to allow selecting multiple items of a specific type or not. If a number is provided, then limit the selections to a maximum of that number of items.

  ```ts
  boolean | number
  ```

```ts
export interface PickerOptions {
  /**
   * The heading of the picker. This is displayed in the title of the picker modal.
   */
  heading: string;
  /**
   * Whether to allow selecting multiple items of a specific type or not. If a number is provided, then limit the selections to a maximum of that number of items.
   * @defaultValue false
   */
  multiple?: boolean | number;
  /**
   * The data headers for the picker. These are used to display the table headers in the picker modal.
   */
  headers?: Header[];
  /**
   * The items to display in the picker. These are used to display the rows in the picker modal.
   */
  items: PickerItem[];
}
```

### Header

- content

  The content to display in the table column header.

  ```ts
  string
  ```

- type

  The type of data to display in the column. The type is used to format the data in the column. If the type is 'number', the data in the column will be right-aligned, this should be used when referencing currency or numeric values. If the type is 'string', the data in the column will be left-aligned.

  ```ts
  'string' | 'number'
  ```

```ts
interface Header {
  /**
   * The content to display in the table column header.
   */
  content?: string;
  /**
   * The type of data to display in the column. The type is used to format the data in the column.
   * If the type is 'number', the data in the column will be right-aligned, this should be used when referencing currency or numeric values.
   * If the type is 'string', the data in the column will be left-aligned.
   * @defaultValue 'string'
   */
  type?: 'string' | 'number';
}
```

### PickerItem

- badges

  The badges to display in the first column of the row. These are used to display additional information about the item, such as progress of an action or tone indicating the status of that item.

  ```ts
  Badge[]
  ```

- data

  The additional content to display in the second and third columns of the row, if provided.

  ```ts
  DataPoint[]
  ```

- disabled

  Whether the item is disabled or not. If the item is disabled, it cannot be selected.

  ```ts
  boolean
  ```

- heading

  The primary content to display in the first column of the row.

  ```ts
  string
  ```

- id

  ```ts
  string
  ```

- selected

  Whether the item is selected or not when the picker is opened. The user may deselect the item if it is preselected.

  ```ts
  boolean
  ```

- thumbnail

  The thumbnail to display at the start of the row. This is used to display an image or icon for the item.

  ```ts
  { url: string; }
  ```

```ts
export interface PickerItem {
  /*
   * The unique identifier of the item. This will be returned by the picker if selected.
   */
  id: string;
  /**
   * The primary content to display in the first column of the row.
   */
  heading: string;
  /**
   * The additional content to display in the second and third columns of the row, if provided.
   */
  data?: DataPoint[];
  /**
   * Whether the item is disabled or not. If the item is disabled, it cannot be selected.
   * @defaultValue false
   */
  disabled?: boolean;
  /**
   * Whether the item is selected or not when the picker is opened. The user may deselect the item if it is preselected.
   */
  selected?: boolean;
  /**
   * The badges to display in the first column of the row. These are used to display additional information about the item, such as progress of an action or tone indicating the status of that item.
   */
  badges?: Badge[];
  /**
   * The thumbnail to display at the start of the row. This is used to display an image or icon for the item.
   */
  thumbnail?: { url: string };
}
```

### Badge

- content

  ```ts
  string
  ```

- progress

  ```ts
  Progress
  ```

- tone

  ```ts
  Tone
  ```

```ts
interface Badge {
  content: string;
  tone?: Tone;
  progress?: Progress;
}
```

### Progress

```ts
'incomplete' | 'partiallyComplete' | 'complete'
```

### Tone

```ts
'info' | 'success' | 'warning' | 'critical'
```

### DataPoint

```ts
string | number | undefined
```

### PickerInstance

- selected

  A Promise that resolves with the selected item IDs when the user presses the "Select" button in the picker.

  ```ts
  Promise<string[]>
  ```

```ts
export interface PickerInstance {
  /**
   * A Promise that resolves with the selected item IDs when the user presses the "Select" button in the picker.
   */
  selected?: Promise<string[]>;
}
```

Examples

## Preview

![](https://cdn.shopify.com/shopifycloud/shopify-dev/development/assets/assets/images/api/admin-extensions/2025-10/picker-DqQDb5eA.png)

### Examples

- #### Picker

  ##### Default

  ```js
  const picker = await shopify.picker({
    heading: 'Select a template',
    multiple: false,
    headers: [
      {content: 'Templates'},
      {content: 'Created by'},
      {content: 'Times used', type: 'number'},
    ],
    items: [
      {
        id: '1',
        heading: 'Full width, 1 column',
        data: ['Karine Ruby', '0'],
        badges: [{content: 'Draft', tone: 'info'}, {content: 'Marketing'}],
      },
      {
        id: '2',
        heading: 'Large graphic, 3 column',
        data: ['Charlie Mitchell', '5'],
        badges: [
          {content: 'Published', tone: 'success'},
          {content: 'New feature'},
        ],
        selected: true,
      },
      {
        id: '3',
        heading: 'Promo header, 2 column',
        data: ['Russell Winfield', '10'],
        badges: [{content: 'Published', tone: 'success'}],
      },
    ],
  });

  const selected = await picker.selected;
  ```

- #### Simple picker

  ##### Description

  Minimal required fields picker configuration. If you don't provide the required options (\`heading\` and \`items\`), the picker will not open and an error will be logged.

  ##### Default

  ```js
  const picker = await shopify.picker({
    heading: 'Simple picker configuration',
    items: [
      {
        id: '1',
        heading: 'Item 1',
      },
      {
        id: '2',
        heading: 'Item 2',
      },
    ],
  });

  const selected = await picker.selected;
  ```

- #### Limited selectable items

  ##### Description

  Setting a specific number of selectable items. In this example, the user can select up to 2 items.

  ##### Default

  ```js
  const picker = await shopify.picker({
    heading: 'Limited selectable items (up to 2)',
    multiple: 2,
    headers: [{content: 'Main heading'}],
    items: [
      {
        id: '1',
        heading: 'Item 1',
      },
      {
        id: '2',
        heading: 'Item 2',
      },
      {
        id: '3',
        heading: 'Item 3',
      },
    ],
  });

  const selected = await picker.selected;
  ```

- #### Unlimited selectable items

  ##### Description

  Setting an unlimited number of selectable items.

  ##### Default

  ```js
  const picker = await shopify.picker({
    heading: 'Unlimited selectable items',
    multiple: true,
    headers: [{content: 'Main heading'}],
    items: [
      {
        id: '1',
        heading: 'Item 1',
      },
      {
        id: '2',
        heading: 'Item 2',
      },
      {
        id: '3',
        heading: 'Item 3',
      },
    ],
  });

  const selected = await picker.selected;
  ```

- #### Preselected items

  ##### Description

  Providing preselected items in the picker. These will be selected when the picker opens but can be deselected by the user.

  ##### Default

  ```js
  const picker = await shopify.picker({
    heading: 'Preselected items',
    items: [
      {
        id: '1',
        heading: 'Item 1',
        selected: true,
      },
      {
        id: '2',
        heading: 'Item 2',
      },
    ],
  });

  const selected = await picker.selected;
  ```

- #### Disabled items

  ##### Description

  Providing disabled items in the picker. These will be disabled and cannot be selected by the user.

  ##### Default

  ```js
  const picker = await shopify.picker({
    heading: 'Disabled items',
    items: [
      {
        id: '1',
        heading: 'Item 1',
        disabled: true,
      },
      {
        id: '2',
        heading: 'Item 2',
      },
    ],
  });

  const selected = await picker.selected;
  ```

## Related

[APIs - Picker](https://shopify.dev/docs/api/admin-extensions/unstable/api/target-apis/picker)

</page>

<page>
---
title: POS
description: |2-

    The POS API provides the ability to retrieve POS user, device, and location data, while also interacting with the cart and closing the app.

    > Tip:
    > It is recommended to use POS UI extensions for your development needs as they provide a faster, more robust, and easier to use solution for merchants using apps on POS. To learn more about the benefits and implementation details, refer to [POS UI Extensions](/docs/apps/pos/ui-extensions).
    
api_name: app-home
source_url:
  html: '<https://shopify.dev/docs/api/app-home/apis/pos>'
  md: '<https://shopify.dev/docs/api/app-home/apis/pos.md>'
---

# POS

The POS API provides the ability to retrieve POS user, device, and location data, while also interacting with the cart and closing the app.

**Tip:** It is recommended to use POS UI extensions for your development needs as they provide a faster, more robust, and easier to use solution for merchants using apps on POS. To learn more about the benefits and implementation details, refer to \<a href="/docs/apps/pos/ui-extensions">POS UI Extensions\</a>.

## Cart

Retrieve cart data and perform actions.

- **addAddress**

  **(address: Address) => Promise\<void>**

  Add a new address to a customer.

- **addCartProperties**

  **(properties: Record\<string, string>) => Promise\<void>**

  Add properties for the cart.

- **addCustomSale**

  **(customSale: CustomSale) => Promise\<void>**

  Add custom sale for the cart.

- **addLineItem**

  **(variantId: number, quantity: number) => Promise\<void>**

  Add a product to the cart.

- **addLineItemProperties**

  **(uuid: string, properties: Record\<string, string>) => Promise\<void>**

  Add properties to a line item.

- **applyCartCodeDiscount**

  **(code: string) => Promise\<void>**

  Apply a code discount to the whole cart.

- **applyCartDiscount**

  **(type: DiscountType, discountDescription: string, amount: string) => Promise\<void>**

  Apply a percentage or fixed amount discount to the whole cart.

- **clear**

  **() => Promise\<void>**

  Clear all contents from the cart.

- **fetch**

  **() => Promise\<Cart>**

  Fetch the current cart.

- **removeAllDiscounts**

  **(disableAutomaticDiscounts: boolean) => Promise\<void>**

  Clears all applied discounts from the cart and optionally disables automatic discounts.

- **removeCartDiscount**

  **() => Promise\<void>**

  Remove the discount applied to the whole cart.

- **removeCartProperties**

  **(keys: string\[]) => Promise\<void>**

  Remove properties from the cart.

- **removeCustomer**

  **() => Promise\<void>**

  Remove the current customer from the cart.

- **removeLineItem**

  **(uuid: string) => Promise\<void>**

  Remove a line item in the cart.

- **removeLineItemDiscount**

  **(uuid: string) => Promise\<void>**

  Remove a discount from a line item.

- **removeLineItemProperties**

  **(uuid: string, properties: string\[]) => Promise\<void>**

  Remove properties from a line item.

- **setCustomer**

  **(customer: Customer) => Promise\<void>**

  Add a new or existing customer to the cart.

- **setLineItemDiscount**

  **(uuid: string, type: DiscountType, discountDescription: string, amount: string) => Promise\<void>**

  Apply a discount to a line item.

- **subscribe**

  **(onSubscribe: CartSubscriber) => Unsubscribe**

  Subscribe the cart changes.

- **updateAddress**

  **(index: number, address: Address) => Promise\<void>**

  Update an address for a customer.

- **updateLineItem**

  **(uuid: string, quantity: number) => Promise\<void>**

  Make changes to a line item in the cart.

### Address

- address1

  The customer's primary address.

  ```ts
  string
  ```

- address2

  Any extra information associated with the address (Apartment #, Suite #, Unit #, etc.).

  ```ts
  string
  ```

- city

  The name of the customer's city.

  ```ts
  string
  ```

- company

  The company name associated with address.

  ```ts
  string
  ```

- country

  The country of the address.

  ```ts
  string
  ```

- countryCode

  The Country Code in ISO 3166-1 (alpha-2) format.

  ```ts
  string
  ```

- firstName

  The first name of the customer.

  ```ts
  string
  ```

- lastName

  The last name of the customer.

  ```ts
  string
  ```

- name

  The name of the address.

  ```ts
  string
  ```

- phone

  The phone number of the customer.

  ```ts
  string
  ```

- province

  The province or state of the address.

  ```ts
  string
  ```

- provinceCode

  The acronym of the province or state.

  ```ts
  string
  ```

- zip

  The ZIP or postal code of the address.

  ```ts
  string
  ```

```ts
interface Address {
  /**
   * The customer's primary address.
   */
  address1?: string;
  /**
   * Any extra information associated with the address (Apartment #, Suite #, Unit #, etc.).
   */
  address2?: string;
  /**
   * The name of the customer's city.
   */
  city?: string;
  /**
   * The company name associated with address.
   */
  company?: string;
  /**
   * The first name of the customer.
   */
  firstName?: string;
  /**
   *  The last name of the customer.
   */
  lastName?: string;
  /**
   * The phone number of the customer.
   */
  phone?: string;
  /**
   * The province or state of the address.
   */
  province?: string;
  /**
   * The country of the address.
   */
  country?: string;
  /**
   * The ZIP or postal code of the address.
   */
  zip?: string;
  /**
   * The name of the address.
   */
  name?: string;
  /**
   * The acronym of the province or state.
   */
  provinceCode?: string;
  /**
   * The Country Code in ISO 3166-1 (alpha-2) format.
   */
  countryCode?: string;
}
```

### CustomSale

- price

  Price of line item

  ```ts
  number
  ```

- quantity

  Quantity of line item.

  ```ts
  number
  ```

- taxable

  If line item charges tax.

  ```ts
  boolean
  ```

- title

  Title of line item.

  ```ts
  string
  ```

```ts
interface CustomSale {
  /**
   * Price of line item
   */
  price: number;
  /**
   * Quantity of line item.
   */
  quantity: number;
  /**
   * Title of line item.
   */
  title: string;
  /**
   * If line item charges tax.
   */
  taxable: boolean;
}
```

### DiscountType

```ts
'Percentage' | 'FixedAmount'
```

### Cart

- cartDiscount

  The current discount applied to the entire cart.

  ```ts
  Discount
  ```

- cartDiscounts

  All current discounts applied to the entire cart and line items.

  ```ts
  Discount[]
  ```

- customer

  The customer associated to the current cart.

  ```ts
  Customer
  ```

- grandTotal

  The total cost of the current cart, after taxes and discounts have been applied. Value is based on the shop's existing currency settings.

  ```ts
  string
  ```

- lineItems

  A list of lineItem objects.

  ```ts
  LineItem[]
  ```

- properties

  A list of objects containing cart properties.

  ```ts
  Record<string, string>
  ```

- subTotal

  The total cost of the current cart including discounts, but before taxes and shipping. Value is based on the shop's existing currency settings.

  ```ts
  string
  ```

- taxTotal

  The sum of taxes for the current cart. Value is based on the shop's existing currency settings.

  ```ts
  string
  ```

```ts
interface Cart {
  /**
   *  The total cost of the current cart including discounts, but before taxes and shipping. Value is based on the shop's existing currency settings.
   */
  subTotal: string;
  /**
   * The sum of taxes for the current cart. Value is based on the shop's existing currency settings.
   */
  taxTotal: string;
  /**
   * The total cost of the current cart, after taxes and discounts have been applied. Value is based on the shop's existing currency settings.
   */
  grandTotal: string;
  /**
   * The current discount applied to the entire cart.
   */
  cartDiscount?: Discount;
  /**
   * All current discounts applied to the entire cart and line items.
   */
  cartDiscounts?: Discount[];
  /**
   * The customer associated to the current cart.
   */
  customer?: Customer;
  /**
   * A list of lineItem objects.
   */
  lineItems: LineItem[];
  /**
   * A list of objects containing cart properties.
   */
  properties: Record<string, string>;
}
```

### Discount

- amount

  Amount of discount. Only for fixed or percentage discounts.

  ```ts
  number
  ```

- discountDescription

  Description of discount.

  ```ts
  string
  ```

- type

  Type of discount.

  ```ts
  DiscountType
  ```

```ts
interface Discount {
  /**
   * Amount of discount. Only for fixed or percentage discounts.
   */
  amount: number;
  /**
   * Description of discount.
   */
  discountDescription?: string;
  /**
   * Type of discount.
   */
  type: DiscountType;
}
```

### Customer

- email

  The email for a new customer.

  ```ts
  string
  ```

- firstName

  The first name for new customer.

  ```ts
  string
  ```

- id

  The ID of existing customer.

  ```ts
  number
  ```

- lastName

  The last name for new customer.

  ```ts
  string
  ```

- note

  The note for new customer.

  ```ts
  string
  ```

```ts
interface Customer {
  /**
   * The ID of existing customer.
   */
  id: number;
  /**
   * The email for a new customer.
   */
  email?: string;
  /**
   * The first name for new customer.
   */
  firstName?: string;
  /**
   * The last name for new customer.
   */
  lastName?: string;
  /**
   * The note for new customer.
   */
  note?: string;
}
```

### LineItem

- discounts

  Discount applied to line item.

  ```ts
  Discount[]
  ```

- isGiftCard

  If the line item is a gift card.

  ```ts
  boolean
  ```

- price

  Price of line item

  ```ts
  number
  ```

- productId

  Product identifier for line item.

  ```ts
  number
  ```

- properties

  Properties of the line item.

  ```ts
  { [key: string]: string; }
  ```

- quantity

  Quantity of line item.

  ```ts
  number
  ```

- sku

  Stock keeping unit of the line item.

  ```ts
  string
  ```

- taxable

  If line item charges tax.

  ```ts
  boolean
  ```

- title

  Title of line item.

  ```ts
  string
  ```

- uuid

  Unique id of line item

  ```ts
  string
  ```

- variantId

  Variant identifier for line item.

  ```ts
  number
  ```

- vendor

  Vendor of line item.

  ```ts
  string
  ```

```ts
interface LineItem {
  /**
   * Unique id of line item
   */
  uuid: string;
  /**
   * Price of line item
   */
  price?: number;
  /**
   * Quantity of line item.
   */
  quantity: number;
  /**
   * Title of line item.
   */
  title?: string;
  /**
   * Variant identifier for line item.
   */
  variantId?: number;
  /**
   * Product identifier for line item.
   */
  productId?: number;
  /**
   * Discount applied to line item.
   */
  discounts: Discount[];
  /**
   * If line item charges tax.
   */
  taxable: boolean;
  /**
   * Stock keeping unit of the line item.
   */
  sku?: string;
  /**
   * Vendor of line item.
   */
  vendor?: string;
  /**
   * Properties of the line item.
   */
  properties: {[key: string]: string};
  /**
   * If the line item is a gift card.
   */
  isGiftCard: boolean;
}
```

### CartSubscriber

Callback to execute when cart updates.

- cart

  ```ts
  Cart
  ```

void

```ts
void
```

```ts
(cart: Cart) => void
```

### Unsubscribe

Callback to unsubscribe

void

```ts
void
```

```ts
() => void
```

## Close()

Close the app

### Returns

- **Promise\<void>**

## Device()

Retrieve device data

### Returns

- **Promise\<Device>**

### Device

- name

  The name of the device.

  ```ts
  string
  ```

- serialNumber

  The unique ID associated device ID and app ID..

  ```ts
  string
  ```

```ts
interface Device {
  /**
   * The name of the device.
   */
  name: string;
  /**
   * The unique ID associated device ID and app ID..
   */
  serialNumber: string;
}
```

## Location()

Retrieve location data

### Returns

- **Promise\<Location>**

### Location

- active

  The status of current location.

  ```ts
  boolean
  ```

- address1

  The primary address of current location.

  ```ts
  string
  ```

- address2

  Any extra information associated with the address (Apartment #, Suite #, Unit #, etc.).

  ```ts
  string
  ```

- city

  The name of the city.

  ```ts
  string
  ```

- countryCode

  The Country Code in ISO 3166-1 (alpha-2) format.

  ```ts
  string
  ```

- countryName

  The country of the address.

  ```ts
  string
  ```

- id

  The ID of current location.

  ```ts
  number
  ```

- locationType

  The type of current location.

  ```ts
  string
  ```

- name

  The name of current location.

  ```ts
  string
  ```

- phone

  The phone number of the location.

  ```ts
  string
  ```

- province

  TThe province or state of the address.

  ```ts
  string
  ```

- zip

  The ZIP or postal code of the address.

  ```ts
  string
  ```

```ts
interface Location {
  /**
   * The ID of current location.
   */
  id: number;
  /**
   * The status of current location.
   */
  active: boolean;
  /**
   * The name of current location.
   */
  name: string;
  /**
   * The type of current location.
   */
  locationType?: string;
  /**
   * The primary address of current location.
   */
  address1?: string;
  /**
   * Any extra information associated with the address (Apartment #, Suite #, Unit #, etc.).
   */
  address2?: string;
  /**
   * The ZIP or postal code of the address.
   */
  zip?: string;
  /**
   * The name of the city.
   */
  city?: string;
  /**
   * TThe province or state of the address.
   */
  province?: string;
  /**
   * The Country Code in ISO 3166-1 (alpha-2) format.
   */
  countryCode?: string;
  /**
   * The country of the address.
   */
  countryName?: string;
  /**
   *  The phone number of the location.
   */
  phone?: string;
}
```

## User()

Refer to the [user API](https://shopify.dev/docs/api/app-bridge-library/apis/user) to learn more about retrieving POS user data.

### Returns

- **Promise\<POSUser>**

### POSUser

- accountAccess

  The account access level of the logged-in user

  ```ts
  string
  ```

- accountType

  The user's account type.

  ```ts
  string
  ```

- email

  The user's email address.

  ```ts
  string
  ```

- firstName

  The user's first name.

  ```ts
  string
  ```

- id

  The ID of the user's staff.

  ```ts
  number
  ```

- lastName

  The user's last name.

  ```ts
  string
  ```

```ts
export interface POSUser {
  /**
   * The ID of the user's staff.
   */
  id?: number;
  /**
   * The user's first name.
   */
  firstName?: string;
  /**
   * The user's last name.
   */
  lastName?: string;
  /**
   * The user's email address.
   */
  email?: string;
  /**
   *   The account access level of the logged-in user
   */
  accountAccess?: string;
  /**
   * The user's account type.
   */
  accountType?: string;
}
```

Examples

## Preview

![](https://cdn.shopify.com/shopifycloud/shopify-dev/development/assets/assets/images/templated-apis-screenshots/admin/apis/pos-11_huAsZ.png)

### Examples

- #### Fetch the POS cart

  ##### Default

  ```js
  await shopify.pos.cart.fetch();
  ```

- #### Fetch the cart

  ##### Default

  ```js
  await shopify.pos.cart.fetch();
  ```

- #### Subscribe to cart updates

  ##### Default

  ```js
  await shopify.pos.cart.subscribe((cart) => {
    console.log(cart);
  });
  ```

- #### Clear the cart

  ##### Default

  ```js
  await shopify.pos.cart.clear();
  ```

- #### Line Items

  ##### Add line item

  ```js
  await shopify.pos.cart.addLineItem(40202439393345, 10);
  ```

  ##### Update line item

  ```js
  const cart = await shopify.pos.cart.fetch();
  const lineItemUuid = cart.lineItems[0].uuid;
  await shopify.pos.cart.updateLineItem(lineItemUuid, 4);
  ```

  ##### Remove line item

  ```js
  const cart = await shopify.pos.cart.fetch();
  const lineItemUuid = cart.lineItems[0].uuid;
  await shopify.pos.cart.removeLineItem(lineItemUuid);
  ```

- #### Custom Sale

  ##### Add custom sale

  ```js
  await shopify.pos.cart.addCustomSale({
    price: 10,
    quantity: 1,
    title: 'Custom sale',
    taxable: true,
  });
  ```

- #### Customers

  ##### Add a customer by email

  ```js
  await shopify.pos.cart.setCustomer({
    email: 'foo@shopify.com',
    firstName: 'Jane',
    lastName: 'Doe',
    note: 'Customer note',
  });
  ```

  ##### Add a customer by id

  ```js
  await shopify.pos.cart.setCustomer({
    id: 5945486803009,
    note: 'Customer note',
  });
  ```

  ##### Remove customer

  ```js
  await shopify.pos.cart.removeCustomer();
  ```

  ##### Add a customer address

  ```js
  await shopify.pos.cart.addAddress({
    address1: '123 Cherry St.',
    address2: 'Apt. 5',
    city: 'Toronto',
    company: 'Shopify',
    firstName: 'Foo',
    lastName: 'Bar',
    phone: '(613) 555-5555',
    province: 'Ontario',
    country: 'Canada',
    zip: 'M5V0G4',
    name: 'Shopify',
    provinceCode: 'M5V0G4',
    countryCode: '1',
  });
  ```

  ##### Update customer address

  ```js
  await shopify.pos.cart.updateAddress(0, {
    address1: '555 Apple St.',
    address2: 'Unit. 10',
    city: 'Vancouver',
    company: 'Shopify',
    firstName: 'Jane',
    lastName: 'Doe',
    phone: '(403) 555-5555',
    province: 'British Columbia',
    country: 'Canada',
    zip: 'M5V0G4',
    name: 'Shopify',
    provinceCode: 'M5V0G4',
    countryCode: '2',
  });
  ```

- #### Addresses

  ##### Add a customer address

  ```js
  await shopify.pos.cart.addAddress({
    address1: '123 Cherry St.',
    address2: 'Apt. 5',
    city: 'Toronto',
    company: 'Shopify',
    firstName: 'Foo',
    lastName: 'Bar',
    phone: '(613) 555-5555',
    province: 'Ontario',
    country: 'Canada',
    zip: 'M5V0G4',
    name: 'Shopify',
    provinceCode: 'M5V0G4',
    countryCode: '1',
  });
  ```

  ##### Update customer address

  ```js
  await shopify.pos.cart.updateAddress(0, {
    address1: '555 Apple St.',
    address2: 'Unit. 10',
    city: 'Vancouver',
    company: 'Shopify',
    firstName: 'Jane',
    lastName: 'Doe',
    phone: '(403) 555-5555',
    province: 'British Columbia',
    country: 'Canada',
    zip: 'M5V0G4',
    name: 'Shopify',
    provinceCode: 'M5V0G4',
    countryCode: '2',
  });
  ```

- #### Cart Discounts

  ##### Add cart discount

  ```js
  await shopify.pos.cart.applyCartDiscount('FixedAmount', 'Holiday sale', '10');
  ```

  ##### Add dicount code

  ```js
  await shopify.pos.cart.applyCartCodeDiscount('HOLIDAY SALE');
  ```

  ##### Remove cart discount

  ```js
  await shopify.pos.cart.removeCartDiscount();
  ```

  ##### Remove all discounts with automatic discounts disabled

  ```js
  await shopify.pos.cart.removeAllDiscounts(true);
  ```

- #### Line Item Discounts

  ##### Add line item discount

  ```js
  const cart = await shopify.pos.cart.fetch();
  const lineItemUuid = cart.lineItems[0].uuid;
  await shopify.pos.cart.setLineItemDiscount(
    lineItemUuid,
    'Percentage',
    'Holiday sale',
    '0.5',
  );
  ```

  ##### Remove line item discount

  ```js
  const cart = await shopify.pos.cart.fetch();
  const lineItemUuid = cart.lineItems[0].uuid;
  await shopify.pos.cart.removeLineItemDiscount(lineItemUuid);
  ```

- #### Cart Properties

  ##### Add cart properties

  ```js
  await shopify.pos.cart.addCartProperties({
    referral: 'Shopify',
    employee: '472',
  });
  ```

  ##### Remove cart properties

  ```js
  await shopify.pos.cart.removeCartProperties(['referral', 'employee']);
  ```

  ##### Add line item properties

  ```js
  const cart = await shopify.pos.cart.fetch();
  const lineItemUuid = cart.lineItems[0].uuid;
  await shopify.pos.cart.addLineItemProperties(lineItemUuid, {
    referral: 'Shopify',
    employee: '472',
  });
  ```

  ##### Remove line item properties

  ```js
  const cart = await shopify.pos.cart.fetch();
  const lineItemUuid = cart.lineItems[0].uuid;
  await shopify.pos.cart.removeLineItemProperties(lineItemUuid, [
    'referral',
    'employee',
  ]);
  ```

- #### Line Item Properties

  ##### Add line item properties

  ```js
  const cart = await shopify.pos.cart.fetch();
  const lineItemUuid = cart.lineItems[0].uuid;
  await shopify.pos.cart.addLineItemProperties(lineItemUuid, {
    referral: 'Shopify',
    employee: '472',
  });
  ```

  ##### Remove line item properties

  ```js
  const cart = await shopify.pos.cart.fetch();
  const lineItemUuid = cart.lineItems[0].uuid;
  await shopify.pos.cart.removeLineItemProperties(lineItemUuid, [
    'referral',
    'employee',
  ]);
  ```

- #### Dismiss the screen

  ##### Default

  ```js
  await shopify.pos.close();
  ```

- #### Retrieve POS device data

  ##### Default

  ```js
  await shopify.pos.device();
  ```

- #### Retrieve POS location data

  ##### Default

  ```js
  await shopify.pos.location();
  ```

</page>

<page>
---
title: Print
description: >-
  The Print API allows you to print the content from your App Home on Shopify
  Mobile and Shopify POS devices.

For more information, see the [Window `print()`](https://developer.mozilla.org/en-US/docs/Web/API/Window/print) documentation.
api_name: app-home
source_url:
  html: '<https://shopify.dev/docs/api/app-home/apis/print>'
  md: '<https://shopify.dev/docs/api/app-home/apis/print.md>'
---

# Print

The Print API allows you to print the content from your App Home on Shopify Mobile and Shopify POS devices.

For more information, see the [Window `print()`](https://developer.mozilla.org/en-US/docs/Web/API/Window/print) documentation.

Examples

### Examples

- #### Print

  ##### Default

  ```js
  print();
  ```

</page>

<page>
---
title: useAppBridge
description: >-
  The `useAppBridge` hook returns the `shopify` global variable to use App
  Bridge APIs such as `toast` and `resourcePicker`.
api_name: app-home
source_url:
  html: 'https://shopify.dev/docs/api/app-home/apis/react-hooks/useappbridge'
  md: 'https://shopify.dev/docs/api/app-home/apis/react-hooks/useappbridge.md'
---

# use​App​Bridge

**Requires \[\`@shopify/app-bridge-react\@v4\`]\(<https://www\.npmjs.com/package/@shopify/app-bridge-react>) and the \[\`app-bridge.js\` script tag]\(/docs/api/app-bridge-library#getting-started):**

The `useAppBridge` hook returns the `shopify` global variable to use App Bridge APIs such as `toast` and `resourcePicker`.

## useAppBridge hook

The `useAppBridge` hook is available for use in your app. It returns the `shopify` global or a proxy when not in a browser environment.

For more information, see the [global variable section](https://shopify.dev/docs/api/app-bridge-library#shopify-global-variable) and the individual reference pages like [Toast](https://shopify.dev/docs/api/app-bridge-library/apis/toast) and [Resource Picker](https://shopify.dev/docs/api/app-bridge-library/apis/resource-picker).

Examples

### Examples

- #### useAppBridge

  ##### Default

  ```jsx
  import {useAppBridge} from '@shopify/app-bridge-react';

  export function GenerateBlogPostButton() {
    const shopify = useAppBridge();

    function generateBlogPost() {
      // Handle generating
      shopify.toast.show('Blog post template generated');
    }

    return <button onClick={generateBlogPost}>Generate Blog Post</button>;
  }
  ```

## Related

[API - shopify global variable](https://shopify.dev/docs/api/app-bridge-library#shopify-global-variable)

</page>

<page>
---
title: Resource Fetching
description: >-
  The `fetch` API allows you to send a fetch request that is authenticated with
  an [OpenID Connect ID Token](/docs/api/app-bridge-library/apis/id-token) from
  Shopify in the `Authorization` header. This is authenticated for your
  application domain and subdomains. See the [Fetch
  API](https://developer.mozilla.org/en-US/docs/Web/API/Fetch_API) documentation
  for more details.

App Bridge injects automatic authorization into the global <code>fetch</code> function. While this is transparent and should not interfere with existing fetch code, this injection can be disabled using the [<code>disabledFeatures</code>](/docs/api/app-bridge-library/apis/config#setting-config-values-disabledfeatures) configuration option. You will need to enable [Direct API access](/docs/apps/build/cli-for-apps/app-configuration#admin) for your app to use this feature.
api_name: app-home
source_url:
  html: '<https://shopify.dev/docs/api/app-home/apis/resource-fetching>'
  md: '<https://shopify.dev/docs/api/app-home/apis/resource-fetching.md>'
---

# Resource Fetching

The `fetch` API allows you to send a fetch request that is authenticated with an [OpenID Connect ID Token](https://shopify.dev/docs/api/app-bridge-library/apis/id-token) from Shopify in the `Authorization` header. This is authenticated for your application domain and subdomains. See the [Fetch API](https://developer.mozilla.org/en-US/docs/Web/API/Fetch_API) documentation for more details.

App Bridge injects automatic authorization into the global `fetch` function. While this is transparent and should not interfere with existing fetch code, this injection can be disabled using the [`disabledFeatures`](https://shopify.dev/docs/api/app-bridge-library/apis/config#setting-config-values-disabledfeatures) configuration option. You will need to enable [Direct API access](https://shopify.dev/docs/apps/build/cli-for-apps/app-configuration#admin) for your app to use this feature.

Examples

### Examples

- #### fetch

  ##### Default

  ```js
  fetch('/api/endpoint');
  ```

- #### Fetch with custom headers

  ##### Description

  Fetch with custom headers

  ##### Default

  ```js
  fetch('/api/endpoint', {
    headers: {'accept-language': 'fr'},
  });
  ```

- #### Fetch directly from the Admin API

  ##### Description

  Fetch directly from the Admin API using Direct API access

  ##### Default

  ```js
  const res = await fetch('shopify:admin/api/2025-04/graphql.json', {
    method: 'POST',
    body: JSON.stringify({
      query: `
        query GetProduct($id: ID!) {
          product(id: $id) {
            title
          }
        }
      `,
      variables: {id: 'gid://shopify/Product/1234567890'},
    }),
  });

  const {data} = await res.json();
  console.log(data);
  ```

</page>

<page>
---
title: Resource Picker
description: >-
  The Resource Picker API provides a search-based interface to help users find
  and select one or more products, collections, or product variants, and then
  returns the selected resources to your app. Both the app and the user must
  have the necessary permissions to access the resources selected.

  > Tip:

  > If you are picking app resources such as product reviews, email templates,
  or subscription options, you should use the [Picker](picker) API instead.
api_name: app-home
source_url:
  html: '<https://shopify.dev/docs/api/app-home/apis/resource-picker>'
  md: '<https://shopify.dev/docs/api/app-home/apis/resource-picker.md>'
---

# Resource Picker

The Resource Picker API provides a search-based interface to help users find and select one or more products, collections, or product variants, and then returns the selected resources to your app. Both the app and the user must have the necessary permissions to access the resources selected.

**Tip:** If you are picking app resources such as product reviews, email templates, or subscription options, you should use the \<a href="picker">Picker\</a> API instead.

## Resource Picker Options

The `Resource Picker` accepts a variety of options to customize the picker's behavior.

- **type**

  **ResourceType**

  **required**

  The type of resource you want to pick.

- **action**

  **'add' | 'select'**

  **Default: 'add'**

  The action verb appears in the title and as the primary action of the Resource Picker.

- **filter**

  **Filters**

  Filters for what resource to show.

- **multiple**

  **boolean | number**

  **Default: false**

  Whether to allow selecting multiple items of a specific type or not. If a number is provided, then limit the selections to a maximum of that number of items. When type is Product, the user may still select multiple variants of a single product, even if multiple is false.

- **query**

  **string**

  **Default: ''**

  GraphQL initial search query for filtering resources available in the picker. See [search syntax](https://shopify.dev/docs/api/usage/search-syntax) for more information. This is displayed in the search bar when the picker is opened and can be edited by users. For most use cases, you should use the `filter.query` option instead which doesn't show the query in the UI.

- **selectionIds**

  **BaseResource\[]**

  **Default: \[]**

  Resources that should be preselected when the picker is opened.

### Filters

- archived

  Whether to show \[archived products]\(<https://help.shopify.com/en/manual/products/details?shpxid=70af7d87-E0F2-4973-8B09-B972AAF0ADFD#product-availability>). Only applies to the Product resource type picker. Setting this to undefined will show a badge on draft products.

  ```ts
  boolean | undefined
  ```

- draft

  Whether to show \[draft products]\(<https://help.shopify.com/en/manual/products/details?shpxid=70af7d87-E0F2-4973-8B09-B972AAF0ADFD#product-availability>). Only applies to the Product resource type picker. Setting this to undefined will show a badge on draft products.

  ```ts
  boolean | undefined
  ```

- hidden

  Whether to show hidden resources, referring to products that are not published on any sales channels.

  ```ts
  boolean
  ```

- query

  GraphQL initial search query for filtering resources available in the picker. See \[search syntax]\(<https://shopify.dev/docs/api/usage/search-syntax>) for more information. This is not displayed in the search bar when the picker is opened.

  ```ts
  string
  ```

- variants

  Whether to show product variants. Only applies to the Product resource type picker.

  ```ts
  boolean
  ```

```ts
interface Filters {
  /**
   * Whether to show hidden resources, referring to products that are not published on any sales channels.
   * @defaultValue true
   */
  hidden?: boolean;
  /**
   * Whether to show product variants. Only applies to the Product resource type picker.
   * @defaultValue true
   */
  variants?: boolean;
  /**
   * Whether to show [draft products](https://help.shopify.com/en/manual/products/details?shpxid=70af7d87-E0F2-4973-8B09-B972AAF0ADFD#product-availability).
   * Only applies to the Product resource type picker.
   * Setting this to undefined will show a badge on draft products.
   * @defaultValue true
   */
  draft?: boolean | undefined;
  /**
   * Whether to show [archived products](https://help.shopify.com/en/manual/products/details?shpxid=70af7d87-E0F2-4973-8B09-B972AAF0ADFD#product-availability).
   * Only applies to the Product resource type picker.
   * Setting this to undefined will show a badge on draft products.
   * @defaultValue true
   */
  archived?: boolean | undefined;
  /**
   * GraphQL initial search query for filtering resources available in the picker.
   * See [search syntax](https://shopify.dev/docs/api/usage/search-syntax) for more information.
   * This is not displayed in the search bar when the picker is opened.
   */
  query?: string;
}
```

### BaseResource

- id

  in GraphQL id format, ie 'gid://shopify/Product/1'

  ```ts
  string
  ```

- variants

  ```ts
  Resource[]
  ```

```ts
interface BaseResource extends Resource {
  variants?: Resource[];
}
```

### Resource

- id

  in GraphQL id format, ie 'gid://shopify/Product/1'

  ```ts
  string
  ```

```ts
interface Resource {
  /** in GraphQL id format, ie 'gid://shopify/Product/1' */
  id: string;
}
```

## Resource Picker Return Payload

The `Resource Picker` returns a Promise with an array of the selected resources. The object type in the array varies based on the provided `type` option.

If the picker is cancelled, the Promise resolves to `undefined`

- **when type is "collection":**

  **Collection\[]**

- **when type is "product":**

  **Product\[]**

- **when type is "variant":**

  **ProductVariant\[]**

### Collection

- availablePublicationCount

  ```ts
  number
  ```

- description

  ```ts
  string
  ```

- descriptionHtml

  ```ts
  string
  ```

- handle

  ```ts
  string
  ```

- id

  in GraphQL id format, ie 'gid://shopify/Product/1'

  ```ts
  string
  ```

- image

  ```ts
  Image | null
  ```

- productsAutomaticallySortedCount

  ```ts
  number
  ```

- productsCount

  ```ts
  number
  ```

- productsManuallySortedCount

  ```ts
  number
  ```

- publicationCount

  ```ts
  number
  ```

- ruleSet

  ```ts
  RuleSet | null
  ```

- seo

  ```ts
  { description?: string; title?: string; }
  ```

- sortOrder

  ```ts
  CollectionSortOrder
  ```

- storefrontId

  ```ts
  string
  ```

- templateSuffix

  ```ts
  string | null
  ```

- title

  ```ts
  string
  ```

- updatedAt

  ```ts
  string
  ```

```ts
export interface Collection extends Resource {
  availablePublicationCount: number;
  description: string;
  descriptionHtml: string;
  handle: string;
  id: string;
  image?: Image | null;
  productsAutomaticallySortedCount: number;
  productsCount: number;
  productsManuallySortedCount: number;
  publicationCount: number;
  ruleSet?: RuleSet | null;
  seo: {
    description?: string | null;
    title?: string | null;
  };
  sortOrder: CollectionSortOrder;
  storefrontId: string;
  templateSuffix?: string | null;
  title: string;
  updatedAt: string;
}
```

### Image

- altText

  ```ts
  string
  ```

- id

  ```ts
  string
  ```

- originalSrc

  ```ts
  string
  ```

```ts
interface Image {
  id: string;
  altText?: string;
  originalSrc: string;
}
```

### RuleSet

- appliedDisjunctively

  ```ts
  boolean
  ```

- rules

  ```ts
  CollectionRule[]
  ```

```ts
interface RuleSet {
  appliedDisjunctively: boolean;
  rules: CollectionRule[];
}
```

### CollectionRule

- column

  ```ts
  string
  ```

- condition

  ```ts
  string
  ```

- relation

  ```ts
  string
  ```

```ts
interface CollectionRule {
  column: string;
  condition: string;
  relation: string;
}
```

### CollectionSortOrder

- Manual

  ```ts
  MANUAL
  ```

- BestSelling

  ```ts
  BEST_SELLING
  ```

- AlphaAsc

  ```ts
  ALPHA_ASC
  ```

- AlphaDesc

  ```ts
  ALPHA_DESC
  ```

- PriceDesc

  ```ts
  PRICE_DESC
  ```

- PriceAsc

  ```ts
  PRICE_ASC
  ```

- CreatedDesc

  ```ts
  CREATED_DESC
  ```

- Created

  ```ts
  CREATED
  ```

- MostRelevant

  ```ts
  MOST_RELEVANT
  ```

```ts
enum CollectionSortOrder {
  Manual = 'MANUAL',
  BestSelling = 'BEST_SELLING',
  AlphaAsc = 'ALPHA_ASC',
  AlphaDesc = 'ALPHA_DESC',
  PriceDesc = 'PRICE_DESC',
  PriceAsc = 'PRICE_ASC',
  CreatedDesc = 'CREATED_DESC',
  Created = 'CREATED',
  MostRelevant = 'MOST_RELEVANT',
}
```

### Product

- availablePublicationCount

  ```ts
  number
  ```

- createdAt

  ```ts
  string
  ```

- descriptionHtml

  ```ts
  string
  ```

- handle

  ```ts
  string
  ```

- hasOnlyDefaultVariant

  ```ts
  boolean
  ```

- id

  in GraphQL id format, ie 'gid://shopify/Product/1'

  ```ts
  string
  ```

- images

  ```ts
  Image[]
  ```

- options

  ```ts
  { id: string; name: string; position: number; values: string[]; }[]
  ```

- productType

  ```ts
  string
  ```

- publishedAt

  ```ts
  string | null
  ```

- status

  ```ts
  ProductStatus
  ```

- tags

  ```ts
  string[]
  ```

- templateSuffix

  ```ts
  string | null
  ```

- title

  ```ts
  string
  ```

- totalInventory

  ```ts
  number
  ```

- totalVariants

  ```ts
  number
  ```

- tracksInventory

  ```ts
  boolean
  ```

- updatedAt

  ```ts
  string
  ```

- variants

  ```ts
  Partial<ProductVariant>[]
  ```

- vendor

  ```ts
  string
  ```

```ts
export interface Product extends Resource {
  availablePublicationCount: number;
  createdAt: string;
  descriptionHtml: string;
  handle: string;
  hasOnlyDefaultVariant: boolean;
  images: Image[];
  options: {
    id: string;
    name: string;
    position: number;
    values: string[];
  }[];
  productType: string;
  publishedAt?: string | null;
  tags: string[];
  templateSuffix?: string | null;
  title: string;
  totalInventory: number;
  totalVariants: number;
  tracksInventory: boolean;
  variants: Partial<ProductVariant>[];
  vendor: string;
  updatedAt: string;
  status: ProductStatus;
}
```

### ProductStatus

- Active

  ```ts
  ACTIVE
  ```

- Archived

  ```ts
  ARCHIVED
  ```

- Draft

  ```ts
  DRAFT
  ```

```ts
enum ProductStatus {
  Active = 'ACTIVE',
  Archived = 'ARCHIVED',
  Draft = 'DRAFT',
}
```

### ProductVariant

- availableForSale

  ```ts
  boolean
  ```

- barcode

  ```ts
  string | null
  ```

- compareAtPrice

  ```ts
  Money | null
  ```

- createdAt

  ```ts
  string
  ```

- displayName

  ```ts
  string
  ```

- fulfillmentService

  ```ts
  { id: string; inventoryManagement: boolean; productBased: boolean; serviceName: string; type: FulfillmentServiceType; }
  ```

- id

  in GraphQL id format, ie 'gid://shopify/Product/1'

  ```ts
  string
  ```

- image

  ```ts
  Image | null
  ```

- inventoryItem

  ```ts
  { id: string; }
  ```

- inventoryManagement

  ```ts
  ProductVariantInventoryManagement
  ```

- inventoryPolicy

  ```ts
  ProductVariantInventoryPolicy
  ```

- inventoryQuantity

  ```ts
  number | null
  ```

- position

  ```ts
  number
  ```

- price

  ```ts
  Money
  ```

- product

  ```ts
  Partial<Product>
  ```

- requiresShipping

  ```ts
  boolean
  ```

- selectedOptions

  ```ts
  { value?: string; }[]
  ```

- sku

  ```ts
  string | null
  ```

- taxable

  ```ts
  boolean
  ```

- title

  ```ts
  string
  ```

- updatedAt

  ```ts
  string
  ```

- weight

  ```ts
  number | null
  ```

- weightUnit

  ```ts
  WeightUnit
  ```

```ts
export interface ProductVariant extends Resource {
  availableForSale: boolean;
  barcode?: string | null;
  compareAtPrice?: Money | null;
  createdAt: string;
  displayName: string;
  fulfillmentService?: {
    id: string;
    inventoryManagement: boolean;
    productBased: boolean;
    serviceName: string;
    type: FulfillmentServiceType;
  };
  image?: Image | null;
  inventoryItem: {id: string};
  inventoryManagement: ProductVariantInventoryManagement;
  inventoryPolicy: ProductVariantInventoryPolicy;
  inventoryQuantity?: number | null;
  position: number;
  price: Money;
  product: Partial<Product>;
  requiresShipping: boolean;
  selectedOptions: {value?: string | null}[];
  sku?: string | null;
  taxable: boolean;
  title: string;
  weight?: number | null;
  weightUnit: WeightUnit;
  updatedAt: string;
}
```

### Money

```ts
string
```

### FulfillmentServiceType

- GiftCard

  ```ts
  GIFT_CARD
  ```

- Manual

  ```ts
  MANUAL
  ```

- ThirdParty

  ```ts
  THIRD_PARTY
  ```

```ts
enum FulfillmentServiceType {
  GiftCard = 'GIFT_CARD',
  Manual = 'MANUAL',
  ThirdParty = 'THIRD_PARTY',
}
```

### ProductVariantInventoryManagement

- Shopify

  ```ts
  SHOPIFY
  ```

- NotManaged

  ```ts
  NOT_MANAGED
  ```

- FulfillmentService

  ```ts
  FULFILLMENT_SERVICE
  ```

```ts
enum ProductVariantInventoryManagement {
  Shopify = 'SHOPIFY',
  NotManaged = 'NOT_MANAGED',
  FulfillmentService = 'FULFILLMENT_SERVICE',
}
```

### ProductVariantInventoryPolicy

- Deny

  ```ts
  DENY
  ```

- Continue

  ```ts
  CONTINUE
  ```

```ts
enum ProductVariantInventoryPolicy {
  Deny = 'DENY',
  Continue = 'CONTINUE',
}
```

### WeightUnit

- Kilograms

  ```ts
  KILOGRAMS
  ```

- Grams

  ```ts
  GRAMS
  ```

- Pounds

  ```ts
  POUNDS
  ```

- Ounces

  ```ts
  OUNCES
  ```

```ts
enum WeightUnit {
  Kilograms = 'KILOGRAMS',
  Grams = 'GRAMS',
  Pounds = 'POUNDS',
  Ounces = 'OUNCES',
}
```

Examples

## Preview

![](https://cdn.shopify.com/shopifycloud/shopify-dev/development/assets/assets/images/templated-apis-screenshots/admin/apis/resource-picker-DeAqlQby.png)

### Examples

- #### Product picker

  ##### Default

  ```js
  const selected = await shopify.resourcePicker({type: 'product'});
  ```

- #### Alternate resources

  ##### Description

  Alternate resources

  ##### Collection picker

  ```js
  const selected = await shopify.resourcePicker({type: 'collection'});
  ```

  ##### Product variant picker

  ```js
  const selected = await shopify.resourcePicker({type: 'variant'});
  ```

- #### Product picker with preselected resources

  ##### Description

  Preselected resources

  ##### Default

  ```js
  const selected = await shopify.resourcePicker({
    type: 'product',
    selectionIds: [
      {
        id: 'gid://shopify/Product/12345',
        variants: [
          {
            id: 'gid://shopify/ProductVariant/1',
          },
        ],
      },
      {
        id: 'gid://shopify/Product/67890',
      },
    ],
  });
  ```

- #### Product picker with action verb

  ##### Description

  Action verb

  ##### Default

  ```js
  const selected = await shopify.resourcePicker({
    type: 'product',
    action: 'select',
  });
  ```

- #### Product picker with multiple selection

  ##### Description

  Multiple selection

  ##### Unlimited selectable items

  ```js
  const selected = await shopify.resourcePicker({
    type: 'product',
    multiple: true,
  });
  ```

  ##### Maximum selectable items

  ```js
  const selected = await shopify.resourcePicker({
    type: 'product',
    multiple: 5,
  });
  ```

- #### Product picker with filters

  ##### Description

  Filters

  ##### Default

  ```js
  const selected = await shopify.resourcePicker({
    type: 'product',
    filter: {
      hidden: true,
      variants: false,
      draft: false,
      archived: false,
    },
  });
  ```

- #### Product picker with a custom filter query

  ##### Description

  Filter query

  ##### Default

  ```js
  const selected = await shopify.resourcePicker({
    type: 'product',
    filter: {
      query: 'Sweater',
    },
  });
  ```

- #### Product picker using returned selection payload

  ##### Description

  Selection

  ##### Default

  ```js
  const selected = await shopify.resourcePicker({type: 'product'});

  if (selected) {
    console.log(selected);
  } else {
    console.log('Picker was cancelled by the user');
  }
  ```

- #### Product picker with initial query provided

  ##### Description

  Initial query

  ##### Default

  ```js
  const selected = await shopify.resourcePicker({
    type: 'product',
    query: 'Sweater',
  });
  ```

</page>

<page>
---
title: Reviews
description: |2-

    The Reviews API allows you to request an app review modal overlaid on your embedded app in the Shopify admin. You control when to request a modal, but it will only be displayed to the merchant if [certain conditions](#rate-limits-restrictions) are met.
    
api_name: app-home
source_url:
  html: '<https://shopify.dev/docs/api/app-home/apis/reviews>'
  md: '<https://shopify.dev/docs/api/app-home/apis/reviews.md>'
---

# Reviews

The Reviews API allows you to request an app review modal overlaid on your embedded app in the Shopify admin. You control when to request a modal, but it will only be displayed to the merchant if [certain conditions](#rate-limits-restrictions) are met.

## Reviews

The Reviews API provides a `request()` method that allows you to request an app review modal.

- **request**

  **() => Promise\<ReviewRequestResponse>**

  **required**

### ReviewRequestResponse

```ts
ReviewRequestSuccessResponse | ReviewRequestDeclinedResponse
```

### ReviewRequestSuccessResponse

- code

  ```ts
  'success'
  ```

- message

  ```ts
  'Review modal shown successfully'
  ```

- success

  ```ts
  true
  ```

```ts
export interface ReviewRequestSuccessResponse {
  success: true;
  code: 'success';
  message: 'Review modal shown successfully';
}
```

### ReviewRequestDeclinedResponse

- code

  ```ts
  ReviewRequestDeclinedCode
  ```

- message

  ```ts
  string
  ```

- success

  ```ts
  false
  ```

```ts
export interface ReviewRequestDeclinedResponse {
  success: false;
  code: ReviewRequestDeclinedCode;
  message: string;
}
```

### ReviewRequestDeclinedCode

```ts
'mobile-app' | 'already-reviewed' | 'annual-limit-reached' | 'cooldown-period' | 'merchant-ineligible' | 'recently-installed' | 'already-open' | 'open-in-progress' | 'cancelled'
```

Examples

## Preview

![](https://cdn.shopify.com/shopifycloud/shopify-dev/development/assets/assets/images/templated-apis-screenshots/admin/apis/reviews-DTX8hadd.png)

### Examples

- #### Request an app review modal

  ##### Default

  ```js
  try {
    const result = await shopify.reviews.request();
    if (!result.success) {
      console.log(`Review modal not displayed. Reason: ${result.code}: ${result.message}`);
    }
    // if result.success *is* true, then review modal is displayed
  } catch (error) {
    console.error('Error requesting review:', error);
  }
  ```

## Response codes and messages

A request to the Reviews API will return one of the following responses:

| success | code | message |
| - | - | - |
| `true` | `success` | Review modal displayed |
| `false` | `mobile-app` | Review modal not supported on mobile devices |
| `false` | `already-reviewed` | Merchant already reviewed this app |
| `false` | `annual-limit-reached` | Review modal already displayed the maximum number of times within the last 365 days |
| `false` | `cooldown-period` | Review modal already displayed within the last 60 days |
| `false` | `merchant-ineligible` | Merchant isn't eligible to review this app |
| `false` | `recently-installed` | Merchant installed this app for less than 24 hours |
| `false` | `already-open` | Review modal is already open |
| `false` | `open-in-progress` | Review modal opening is already in progress |
| `false` | `cancelled` | Review modal opening was cancelled |

## Rate limits and restrictions

A review modal will only be displayed to the merchant if certain conditions are met. Be sure to follow the [recommended best practices](#best-practices) for requesting reviews.

### Rate limits

The Reviews API applies rate limits to ensure a good merchant experience and to prevent abuse. A review modal will only be displayed to a merchant:

- **Once** within any **60-day** period, and
- **Three** times within any **365-day** period.

### Restrictions

A review modal will never be displayed in these cases:

- The merchant already reviewed your app.
- The merchant is on a mobile device.
- The merchant is ineligible to leave a review.
- The merchant has installed your app for less than 24 hours

## Best practices for review requests

Because you can make only a [limited number](#rate-limits-restrictions) of requests for review, make sure to choose the right time:

- **Do** request a review at the end of a successful workflow.
- **Don't** request a review at any point that interrupts a merchant task.
- **Don't** request a review as soon as the merchant opens your app.
- **Don't** trigger a request with a button, link, or other call to action. Because the request might be rate-limited and the modal isn't guaranteed to display, your app UI would appear to be broken.

## Testing the Reviews API

You can use your development store to test the Reviews API, which bypasses the rate limits and restrictions. Reviews submitted from development stores are not published on the Shopify App Store.

</page>

<page>
---
title: Save Bar
description: >-
  The Save Bar API is used to indicate that a form on the current page has
  unsaved information. It can be used in 2 ways:

  1. It is automatically configured when you provide the
  <code>data-save-bar</code> attribute to a [<code>form</code>
  element](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/form) and
  will display the save bar when there are unsaved changes. The
  [<code>submit</code>](https://developer.mozilla.org/en-US/docs/Web/API/HTMLFormElement/submit_event)
  event fires when the form is submitted or when the Save button is pressed. The
  [<code>reset</code>](https://developer.mozilla.org/en-US/docs/Web/API/HTMLFormElement/reset_event)
  event fires when the form is reset or when the Discard button is pressed,
  which discards all unsaved changes in the form.

  2. It can be controlled declaratively through the `ui-save-bar` element. For
  more information, refer to the [`ui-save-bar`
  component](/docs/api/app-bridge-library/web-components/ui-save-bar).
api_name: app-home
source_url:
  html: '<https://shopify.dev/docs/api/app-home/apis/save-bar>'
  md: '<https://shopify.dev/docs/api/app-home/apis/save-bar.md>'

---

# Save Bar

The Save Bar API is used to indicate that a form on the current page has unsaved information. It can be used in 2 ways:

1. It is automatically configured when you provide the `data-save-bar` attribute to a [`form` element](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/form) and will display the save bar when there are unsaved changes. The [`submit`](https://developer.mozilla.org/en-US/docs/Web/API/HTMLFormElement/submit_event) event fires when the form is submitted or when the Save button is pressed. The [`reset`](https://developer.mozilla.org/en-US/docs/Web/API/HTMLFormElement/reset_event) event fires when the form is reset or when the Discard button is pressed, which discards all unsaved changes in the form.

2. It can be controlled declaratively through the `ui-save-bar` element. For more information, refer to the [`ui-save-bar` component](https://shopify.dev/docs/api/app-bridge-library/web-components/ui-save-bar).

## saveBar

The `saveBar` API provides a `show` method to display a save bar, a `hide` method to hide a save bar, and a `toggle` method to toggle the visibility of a save bar. These are used in conjunction with the `ui-save-bar` element. They are alternatives to the `show`, `hide`, and `toggle` instance methods.

- **hide**

  **(id: string) => Promise\<void>**

  Hides the save bar element. An alternative to the `hide` instance method on the `ui-save-bar` element.

- **leaveConfirmation**

  **() => Promise\<void>**

  Checks if saveBar is shown. This promise resolves if the save bar is not shown. Uses this method before navigating away from the page only when you have a custom routing that is not anchor tag.

- **show**

  **(id: string) => Promise\<void>**

  Shows the save bar element. An alternative to the `show` instance method on the `ui-save-bar` element.

- **toggle**

  **(id: string) => Promise\<void>**

  Toggles the save bar element visibility. An alternative to the `toggle` instance method on the `ui-save-bar` element.

Examples

## Preview

![](https://cdn.shopify.com/shopifycloud/shopify-dev/development/assets/assets/images/templated-apis-screenshots/admin/apis/contextual-save-bar-D20oMpjo.png)

### Examples

- #### Save Bar

  ##### Default

  ```html
  <form
    data-save-bar
    onsubmit="console.log('submit', new FormData(event.target)); event.preventDefault();"
  >
    <label>
      Name:
      <input name="username" />
    </label>
  </form>
  ```

- #### Subscribe to Discard

  ##### Description

  Subscribing to the \[\`HTMLFormElement: reset\`]\(<https://developer.mozilla.org/en-US/docs/Web/API/HTMLFormElement/reset\_event>) event

  ##### Event handler property

  ```html
  <form
    data-save-bar
    onreset="console.log('discarding')"
  >
    <label>
      Name:
      <input name="username" />
    </label>
  </form>
  ```

  ##### Event listener

  ```html
  <form data-save-bar>
    <label>
      Name:
      <input name="username" />
    </label>
  </form>

  <script>
    const form = document.querySelector('form');
    form.addEventListener('reset', (e) => {
      console.log('discarding');
    });
  </script>
  ```

- #### Subscribe to Save

  ##### Description

  Subscribing to the \[\`HTMLFormElement: submit\`]\(<https://developer.mozilla.org/en-US/docs/Web/API/HTMLFormElement/reset\_event>) event

  ##### Event handler property

  ```html
  <form
    data-save-bar
    onsubmit="console.log('submitting');"
  >
    <label>
      Name:
      <input name="username" />
    </label>
  </form>
  ```

  ##### Event listener

  ```html
  <form data-save-bar>
    <label>
      Name:
      <input name="username" />
    </label>
  </form>

  <script>
    const form = document.querySelector('form');
    form.addEventListener('submit', (e) => {
      console.log('submitting');
    });
  </script>
  ```

- #### Showing a discard confirmation modal

  ##### Description

  Provide the \`data-discard-confirmation\` attribute to a \`form\` with the \`data-save-bar\` attribute to show a confirmation modal after the user clicks the Discard button of the save bar

  ##### HTML

  ```html
  <form
    data-save-bar
    data-discard-confirmation
    onsubmit="console.log('submit', new FormData(event.target)); event.preventDefault();"
  >
    <label>
      Name:
      <input name="username" />
    </label>
  </form>
  ```

- #### Showing the save bar with the saveBar API

  ##### Description

  Showing the save bar with the saveBar API

  ##### Default

  ```html
  <ui-save-bar id="my-save-bar"></ui-save-bar>

  <button onclick="shopify.saveBar.show('my-save-bar')">Show</button>
  <button onclick="shopify.saveBar.hide('my-save-bar')">Hide</button>
  ```

## Related

[Component - ui-save-bar](https://shopify.dev/docs/api/app-home/app-bridge-web-components/forms)

[Component - SaveBar](https://shopify.dev/docs/api/app-home/apis/save-bar)

</page>

<page>
---
title: Scanner
description: The Scanner API allows you to use the mobile device's camera to scan barcodes.
api_name: app-home
source_url:
  html: 'https://shopify.dev/docs/api/app-home/apis/scanner'
  md: 'https://shopify.dev/docs/api/app-home/apis/scanner.md'
---

# Scanner

The Scanner API allows you to use the mobile device's camera to scan barcodes.

## Scanner

The `scanner` API provides a `capture` method that opens the mobile device's scanner to capture a barcode. It returns a Promise resolving to the scanned barcode data or an error.

- **capture**

  **() => Promise\<ScannerPayload>**

  **required**

### ScannerPayload

- data

  ```ts
  string
  ```

```ts
export interface ScannerPayload {
  data: string;
}
```

Examples

### Examples

- #### Scanner

  ##### Default

  ```js
  try {
    const payload = await shopify.scanner.capture();
    console.log('Scanner success', payload);
  } catch (error) {
    console.log('Scanner error', error);
  }
  ```

</page>

<page>
---
title: Scopes
description: >-
  The Scopes API provides the ability to dynamically manage your access scopes
  within an embedded context.

    > Tip:
    > To learn more about declaring and requesting access scopes, as well as required vs. optional scopes, refer to [manage access scopes](/docs/apps/build/authentication-authorization/app-installation/manage-access-scopes).  

api_name: app-home
source_url:
  html: '<https://shopify.dev/docs/api/app-home/apis/scopes>'
  md: '<https://shopify.dev/docs/api/app-home/apis/scopes.md>'
---

# Scopes

The Scopes API provides the ability to dynamically manage your access scopes within an embedded context.

**Tip:** To learn more about declaring and requesting access scopes, as well as required vs. optional scopes, refer to \<a href="/docs/apps/build/authentication-authorization/app-installation/manage-access-scopes">manage access scopes\</a>.

## Scopes

Provides utilities to query, request, and revoke access scopes for the app using the Admin API.

- **query**

  **() => Promise\<ScopesDetail>**

  **required**

  Queries Shopify for the scopes for this app on this shop

- **request**

  **(scopes: string\[]) => Promise\<ScopesRequestResponse>**

  **required**

  Requests the merchant to grant the provided scopes for this app on this shop

  This will open a [permission grant modal](https://shopify.dev/docs/apps/build/authentication-authorization/app-installation/manage-access-scopes#request-access-scopes-using-the-app-bridge-api-for-embedded-apps) for the merchant to accept or decline the scopes.

- **revoke**

  **(scopes: string\[]) => Promise\<ScopesRevokeResponse>**

  **required**

  Revokes the provided scopes from this app on this shop

### ScopesDetail

- granted

  The scopes that have been granted on the shop for this app

  ```ts
  string[]
  ```

- optional

  The optional scopes that the app has declared in its configuration

  ```ts
  string[]
  ```

- required

  The required scopes that the app has declared in its configuration

  ```ts
  string[]
  ```

```ts
export interface ScopesDetail {
  /**
   * The scopes that have been granted on the shop for this app
   */
  granted: Scope[];
  /**
   * The required scopes that the app has declared in its configuration
   */
  required: Scope[];
  /**
   * The optional scopes that the app has declared in its configuration
   */
  optional: Scope[];
}
```

### ScopesRequestResponse

- detail

  ```ts
  ScopesDetail
  ```

- result

  ```ts
  UserResult
  ```

```ts
export interface ScopesRequestResponse {
  result: UserResult;
  detail: ScopesDetail;
}
```

### UserResult

\`UserResult\` represents the results of a user responding to a scopes request, i.e. a merchant user’s action taken when presented with a grant modal.

```ts
'granted-all' | 'declined-all'
```

### ScopesRevokeResponse

- detail

  ```ts
  ScopesDetail
  ```

```ts
export interface ScopesRevokeResponse {
  detail: ScopesDetail;
}
```

Examples

### Examples

- #### Query access scopes granted to the app on this store

  ##### Default

  ```js
  const { granted } = await shopify.scopes.query();
  ```

- #### Request the user to grant access to the \`read\_products\` scope

  ##### Default

  ```js
  const response = await shopify.scopes.request(['read_products', 'write_discounts']);

  if (response.result === 'granted-all') {
    // merchant has been granted access — continue
    ...
  }
  else if (response.result === 'declined-all') {
    // merchant has declined access — handle accordingly
    ...
  }
  ```

- #### Revoke the granted access to the \`read\_products\` scope

  ##### Default

  ```js
  await shopify.scopes.revoke(['read_products']);
  ```

## Related

[Reference - Managing Access Scopes](https://shopify.dev/docs/apps/build/authentication-authorization/app-installation/manage-access-scopes)

[API - Remix Scopes API](https://shopify.dev/docs/api/shopify-app-remix/v3/apis/scopes)

</page>

<page>
---
title: Share
description: >-
  The Share API allows you to invoke the "share sheet" to share content from
  your App Home on an iOS or Android device.

For more information, see the [`navigator.share()`](https://developer.mozilla.org/en-US/docs/Web/API/Navigator/share) documentation. When using the `navigator.share()` method in an App Home, the `files` value within the `data` parameter is not supported.
api_name: app-home
source_url:
  html: '<https://shopify.dev/docs/api/app-home/apis/share>'
  md: '<https://shopify.dev/docs/api/app-home/apis/share.md>'
---

# Share

The Share API allows you to invoke the "share sheet" to share content from your App Home on an iOS or Android device.

For more information, see the [`navigator.share()`](https://developer.mozilla.org/en-US/docs/Web/API/Navigator/share) documentation. When using the `navigator.share()` method in an App Home, the `files` value within the `data` parameter is not supported.

Examples

### Examples

- #### Share

  ##### Default

  ```js
  try {
    const shareData = {
      text: 'Learn more about Shopify App Bridge',
      url: 'https://shopify.dev/docs/api/app-bridge',
    };
    await navigator.share(shareData);
  } catch (err) {
    console.log('Share error', err);
  }
  ```

</page>

<page>
---
title: Support
description: |2-

    The Support API allows you to optionally register a custom handler when support requests are made directly through App Bridge. This interaction is triggered when a merchant clicks the get support button at the top of the app.

    > Tip:
    > To register a custom support callback, you must define a [Support link extension](/docs/apps/launch/distribution/support-your-customers#custom-support-events) and the link extension must point to a page within your app. This is to ensure consistent behavior when a merchant clicks a support button outside of the app. Without a support link extension, the support callback will be ignored.
    
api_name: app-home
source_url:
  html: '<https://shopify.dev/docs/api/app-home/apis/support>'
  md: '<https://shopify.dev/docs/api/app-home/apis/support.md>'
---

# Support

The Support API allows you to optionally register a custom handler when support requests are made directly through App Bridge. This interaction is triggered when a merchant clicks the get support button at the top of the app.

**Tip:** To register a custom support callback, you must define a \<a href="/docs/apps/launch/distribution/support-your-customers#custom-support-events">Support link extension\</a> and the link extension must point to a page within your app. This is to ensure consistent behavior when a merchant clicks a support button outside of the app. Without a support link extension, the support callback will be ignored.

## Support

The Support API provides a registerHandler method that registers a handler to call when support is requested. It allows you to provide bespoke, in-app support such as opening a live chat widget.

- **registerHandler**

  **(callback: SupportCallback) => Promise\<void>**

### SupportCallback

void | Promise\<void>

```ts
void | Promise<void>
```

```ts
() => void | Promise<void>
```

Examples

### Examples

- #### Register support handle

  ##### Default

  ```js
  // Define the callback function
  const handler = () => {
    // implement your custom functionality
    openLiveChat();
  };

  // Register the callback
  shopify.support.registerHandler(handler);
  ```

</page>

<page>
---
title: Toast
description: >-
  The Toast API displays a non-disruptive message that appears at the bottom of
  the interface to provide quick and short feedback on the outcome of an action.
  This API is modeled after the [Web Notification
  API](https://developer.mozilla.org/en-US/docs/Web/API/Notification).
api_name: app-home
source_url:
  html: 'https://shopify.dev/docs/api/app-home/apis/toast'
  md: 'https://shopify.dev/docs/api/app-home/apis/toast.md'
---

# Toast

The Toast API displays a non-disruptive message that appears at the bottom of the interface to provide quick and short feedback on the outcome of an action. This API is modeled after the [Web Notification API](https://developer.mozilla.org/en-US/docs/Web/API/Notification).

## show method(**[message](#showmethod-propertydetail-message)**​,**[opts](#showmethod-propertydetail-opts)**​)

The `Toast.show` method displays a Toast notification in the Shopify admin. It accepts a variety of options to customize the behavior.

### Parameters

- **message**

  **string**

  **required**

- **opts**

  **ToastOptions**

### Returns**string**

### ToastOptions

- action

  Content of an action button.

  ```ts
  string
  ```

- duration

  The length of time in milliseconds the toast message should persist.

  ```ts
  number
  ```

- isError

  Display an error-styled toast.

  ```ts
  boolean
  ```

- onAction

  Callback fired when the action button is clicked.

  ```ts
  () => void
  ```

- onDismiss

  Callback fired when the dismiss icon is clicked

  ```ts
  () => void
  ```

```ts
export interface ToastOptions {
  /**
   * The length of time in milliseconds the toast message should persist.
   * @defaultValue 5000
   */
  duration?: number;
  /**
   * Display an error-styled toast.
   * @defaultValue false
   */
  isError?: boolean;
  /**
   * Content of an action button.
   */
  action?: string;
  /**
   * Callback fired when the action button is clicked.
   */
  onAction?: () => void;
  /**
   * Callback fired when the dismiss icon is clicked
   */
  onDismiss?: () => void;
}
```

## hide method(**[id](#hidemethod-propertydetail-id)**​)

The `Toast.hide` method hides a Toast notification. This is not required to be called as the Toast notification will automatically hide after the `duration` has elapsed.

### Parameters

- **id**

  **string**

  **required**

### Returns**void**

Examples

## Preview

![](https://cdn.shopify.com/shopifycloud/shopify-dev/development/assets/assets/images/apps/tools/app-bridge-toast-BMc-izxL.png)

### Examples

- #### Toast

  ##### Default

  ```js
  shopify.toast.show('Message sent');
  ```

- #### Toast with duration

  ##### Description

  Toast with duration

  ##### Default

  ```js
  shopify.toast.show('Product saved', {
    duration: 5000,
  });
  ```

- #### Toast with action

  ##### Description

  Toast with action

  ##### Default

  ```js
  shopify.toast.show('Product saved', {
    action: 'Undo',
    onAction: () => {}, // Undo logic
  });
  ```

- #### Toast with dismiss callback

  ##### Description

  Toast with dismiss callback

  ##### Default

  ```js
  shopify.toast.show('Product saved', {
    onDismiss: () => {}, // Dismiss logic
  });
  ```

</page>

<page>
---
title: User
description: >-
  The User API lets you asynchronously retrieve information about the currently
  logged-in user.

The API returns a `Promise`, which contains user information, and the payload
  varies based on whether the user is logged into the Shopify admin or Shopify
  POS.
api_name: app-home
source_url:
  html: '<https://shopify.dev/docs/api/app-home/apis/user>'
  md: '<https://shopify.dev/docs/api/app-home/apis/user.md>'
---

# User

The User API lets you asynchronously retrieve information about the currently logged-in user.

The API returns a `Promise`, which contains user information, and the payload varies based on whether the user is logged into the Shopify admin or Shopify POS.

## Admin User()

The `user` API, which is available on the `shopify` global, asynchronously retrieves information about the user that's logged into the Shopify admin.

### Returns

- **Promise\<AdminUser>**

### AdminUser

- accountAccess

  The account access level of the logged-in user

  ```ts
  string
  ```

```ts
export interface AdminUser {
  /**
   * The account access level of the logged-in user
   */
  accountAccess?: string;
}
```

## POS User()

The `user` API, which is available on the `shopify` global, asynchronously retrieves information about the current user logged into Shopify POS.

### Returns

- **Promise\<POSUser>**

### POSUser

- accountAccess

  The account access level of the logged-in user

  ```ts
  string
  ```

- accountType

  The user's account type.

  ```ts
  string
  ```

- email

  The user's email address.

  ```ts
  string
  ```

- firstName

  The user's first name.

  ```ts
  string
  ```

- id

  The ID of the user's staff.

  ```ts
  number
  ```

- lastName

  The user's last name.

  ```ts
  string
  ```

```ts
export interface POSUser {
  /**
   * The ID of the user's staff.
   */
  id?: number;
  /**
   * The user's first name.
   */
  firstName?: string;
  /**
   * The user's last name.
   */
  lastName?: string;
  /**
   * The user's email address.
   */
  email?: string;
  /**
   *   The account access level of the logged-in user
   */
  accountAccess?: string;
  /**
   * The user's account type.
   */
  accountType?: string;
}
```

Examples

### Examples

- #### User

  ##### Default

  ```js
  await shopify.user();
  ```

</page>

<page>
---
title: Web Vitals
description: >-
  The Web Vitals API allows you to access performance metrics for your app
  directly through App Bridge.
api_name: app-home
source_url:
  html: 'https://shopify.dev/docs/api/app-home/apis/web-vitals'
  md: 'https://shopify.dev/docs/api/app-home/apis/web-vitals.md'
---

# Web Vitals

The Web Vitals API allows you to access performance metrics for your app directly through App Bridge.

## Web Vitals

The Web Vitals API provides an onReport method that registers a callback function to receive Web Vitals data. It allows you to monitor and analyze your app's performance in the Shopify admin.

- **onReport**

  **(callback: WebVitalsCallback) => Promise\<void>**

### WebVitalsCallback

- payload

  ```ts
  WebVitalsReport
  ```

void | Promise\<void>

```ts
void | Promise<void>
```

```ts
(
  payload: WebVitalsReport,
) => void | Promise<void>
```

### WebVitalsReport

- metrics

  ```ts
  WebVitalsMetric[]
  ```

```ts
export interface WebVitalsReport {
  metrics: WebVitalsMetric[];
}
```

### WebVitalsMetric

WebVitals API

- id

  ```ts
  string
  ```

- name

  ```ts
  string
  ```

- value

  ```ts
  number
  ```

```ts
export interface WebVitalsMetric {
  id: string;
  name: string;
  value: number;
}
```

Examples

### Examples

- #### Callback onReport

  ##### Default

  ```js
  // Define the callback function
  const callback = async (metrics) => {
      const monitorUrl = 'https://yourserver.com/web-vitals-metrics';
      const data = JSON.stringify(metrics);

      navigator.sendBeacon(monitorUrl, data);
  };

  // Register the callback
  shopify.webVitals.onReport(callback);
  ```

## Related

[Reference - App Performance Guidelines](https://shopify.dev/docs/apps/build/performance/admin-installation-oauth)

[API - Web Vitals Debug](https://shopify.dev/docs/api/app-bridge-library/apis/config#config-propertydetail-debug)

</page>

<page>
---
title: App Nav
description: >-
  The `s-app-nav` component creates a navigation menu for your app. On desktop
  web browsers, the navigation menu appears as part of the app nav, on the left
  of the screen. On Shopify mobile, the navigation menu appears in a dropdown
  from the TitleBar. This is modeled after the [HTML nav
  element](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/nav). Note
  that nested navigation items are not supported.
api_name: app-home
source_url:
  html: 'https://shopify.dev/docs/api/app-home/app-bridge-web-components/app-nav'
  md: 'https://shopify.dev/docs/api/app-home/app-bridge-web-components/app-nav.md'
---

# App Nav

The `s-app-nav` component creates a navigation menu for your app. On desktop web browsers, the navigation menu appears as part of the app nav, on the left of the screen. On Shopify mobile, the navigation menu appears in a dropdown from the TitleBar. This is modeled after the [HTML nav element](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/nav). Note that nested navigation items are not supported.

## s-app-nav element

The `s-app-nav` element is available for use in your app. It configures the app nav in the Shopify admin.

You may configure the home route of the app by adding the `rel="home"` attribute to a child element. If it is provided, it will not be rendered as a link in the app nav. It needs to have `rel="home"` set along with the `href` set to the root path.

- **children**

  **any**

Examples

## Preview

![](https://cdn.shopify.com/shopifycloud/shopify-dev/development/assets/assets/images/apps/tools/app-bridge-navigation-menu-DdMXEPrX.png)

### Examples

- #### Navigation Menu

  ##### Default

  ```html
  <s-app-nav>
    <s-link href="/" rel="home">Home</s-link>
    <s-link href="/templates">Templates</s-link>
    <s-link href="/settings">Settings</s-link>
  </s-app-nav>
  ```

## Related

[React components - Navigation API](https://shopify.dev/docs/api/app-home/apis/navigation)

</page>

<page>
---
title: App Window
description: >-
  The `s-app-window` component displays a fullscreen modal window. It allows you
  to open up a page in your app specified by the `src` property. You can use
  this when you have larger or complex workflows that you want to display. The
  app window covers the entirety of the screen. The top bar of the app window is
  controlled by the admin and allows the user to exit if needed.
api_name: app-home
source_url:
  html: 'https://shopify.dev/docs/api/app-home/app-bridge-web-components/app-window'
  md: >-
    https://shopify.dev/docs/api/app-home/app-bridge-web-components/app-window.md
---

# App Window

The `s-app-window` component displays a fullscreen modal window. It allows you to open up a page in your app specified by the `src` property. You can use this when you have larger or complex workflows that you want to display. The app window covers the entirety of the screen. The top bar of the app window is controlled by the admin and allows the user to exit if needed.

## s-app-window element

The `s-app-window` element is available for use in your app. It configures a App Window to display in the Shopify Admin.

The content of the app window is specified by the src property and should point to a route within your app.

- **src**

  **string**

  **required**

  The URL of the content to display within the S-App-Window. S-App-Window only supports src-based content (required).

- **id**

  **string**

  A unique identifier for the S-App-Window

## s-app-window instance

The `s-app-window` element provides instance properties and methods to control the App Window.

- **content**

  **undefined**

  **required**

  Always returns undefined for s-app-window (src-only)

- **addEventListener**

  **(type: "show" | "hide", listener: EventListenerOrEventListenerObject) => void**

  Add 'show' | 'hide' event listeners.

- **contentWindow**

  **Window | null**

  A getter for the Window object of the s-app-window iframe. Only accessible when the s-app-window is open.

- **hide**

  **() => Promise\<void>**

  Hides the s-app-window element

- **removeEventListener**

  **(type: "show" | "hide", listener: EventListenerOrEventListenerObject) => void**

  Remove 'show' | 'hide' event listeners.

- **show**

  **() => Promise\<void>**

  Shows the s-app-window element

- **src**

  **string**

  A getter/setter for the s-app-window src URL

- **toggle**

  **() => Promise\<void>**

  Toggles the s-app-window element between showing and hidden states

Examples

## Preview

![](https://cdn.shopify.com/shopifycloud/shopify-dev/development/assets/assets/images/templated-apis-screenshots/admin/app-bridge-web-components/s-app-window-jyBt5mAs.png)

### Examples

- #### App Window

  ##### App Window

  ```html
  <s-app-window id="app-window" src="/app-window-content.html"></s-app-window>

  <s-button command="--show" commandFor="app-window">Open App Window</s-button>
  ```

- #### Title bar heading

  ##### Description

  App Window title

  ##### Default

  ```html
  <s-app-window src="/app-window-content.html"></s-app-window>

  // app-window-content.html
  <s-page heading="App Window Title"></s-page>
  ```

- #### Title bar actions

  ##### Description

  App Window title bar actions

  ##### Default

  ```html
  <s-app-window src="/app-window-content.html"></s-app-window>

  // app-window-content.html
  <s-page heading="App Window Title">
    <s-button slot="primary-action" onclick="shopify.toast.show('Save')">Save</s-button>
    <s-button slot="secondary-actions" onclick="shopify.toast.show('Close')">Close</s-button>
  </s-page>
  ```

- #### Title bar accessory badge

  ##### Description

  Display a status badge in the title bar using the accessory slot. The \`tone\` attribute controls the badge color (\`info\`, \`success\`, \`warning\`, or \`critical\`).

  ##### Default

  ```html
  <s-app-window src="/app-window-content.html"></s-app-window>

  // app-window-content.html
  <s-page heading="Edit Product">
    <s-badge slot="accessory" tone="warning">Draft</s-badge>
    <s-button slot="primary-action">Save</s-button>
  </s-page>
  ```

- #### Icons and menu actions

  ##### Description

  Add icons to action buttons and use \`commandfor\` to create dropdown menus. Menu buttons support the \`tone\` attribute for destructive actions.

  ##### Default

  ```html
  <s-app-window src="/app-window-content.html"></s-app-window>

  // app-window-content.html
  <s-page heading="Product Details">
    <s-button slot="primary-action" icon="save">Save</s-button>
    <s-button slot="secondary-actions" icon="view">Preview</s-button>
    <s-button slot="secondary-actions" commandfor="actions-menu" icon="menu">More</s-button>
    <s-menu id="actions-menu">
      <s-button icon="duplicate">Duplicate</s-button>
      <s-button icon="archive">Archive</s-button>
      <s-button icon="delete" tone="critical">Delete</s-button>
    </s-menu>
  </s-page>
  ```

- #### Instance methods

  ##### Description

  Controlling the App Window with the show and hide methods

  ##### Default

  ```html
  <s-app-window id="app-window" src="/app-window-content.html"></s-app-window>

  <s-button onclick="document.getElementById('app-window').show()">Show App Window</s-button>
  <s-button onclick="document.getElementById('app-window').hide()">Hide App Window</s-button>
  ```

- #### Command attribute

  ##### Description

  Controlling the App Window with the command attribute

  ##### Default

  ```html
  <s-app-window id="app-window" src="/app-window-content.html"></s-app-window>

  <s-button command="--show" commandFor="app-window">Open App Window</s-button>
  <s-button command="--hide" commandFor="app-window">Hide App Window</s-button>
  <s-button command="--toggle" commandFor="app-window">Toggle App Window</s-button>
  ```

- #### Form with save bar

  ##### Description

  Forms with the \`data-save-bar\` attribute automatically integrate with the save bar. Changes to form inputs are tracked and the save bar appears when there are unsaved changes.

  ##### Default

  ```html
  <s-app-window src="/app-window-content.html"></s-app-window>

  // app-window-content.html
  <s-page heading="Edit Settings">
    <s-button slot="primary-action" type="submit" form="settings-form">Save</s-button>
    <form id="settings-form" data-save-bar>
      <s-text-field label="Store Name" name="storeName"></s-text-field>
      <s-checkbox label="Enable Notifications" name="notifications"></s-checkbox>
    </form>
  </s-page>
  ```

## Related

[API - Modal](https://shopify.dev/docs/api/app-home/apis/modal-api)

</page>

<page>
---
title: Forms
description: >-
  Enable automatic save bar integration for HTML forms by adding the
  `data-save-bar` attribute to your form element. When form data changes, a save
  bar automatically appears, prompting users to save or discard their changes.

  Alternatively, use the global `shopify.saveBar` API for programmatic control
  over the save bar behavior. Programmatic control of the save bar is available
  as `shopify.saveBar.show()`, `shopify.saveBar.hide()`, and
  `shopify.saveBar.toggle()`.

**Note:** The save bar functionality requires the full App Bridge UI library
  to be loaded via a [script tag](/docs/api/app-home/using-polaris-components).
api_name: app-home
source_url:
  html: '<https://shopify.dev/docs/api/app-home/app-bridge-web-components/forms>'
  md: '<https://shopify.dev/docs/api/app-home/app-bridge-web-components/forms.md>'
---

# Forms

Enable automatic save bar integration for HTML forms by adding the `data-save-bar` attribute to your form element. When form data changes, a save bar automatically appears, prompting users to save or discard their changes.

Alternatively, use the global `shopify.saveBar` API for programmatic control over the save bar behavior. Programmatic control of the save bar is available as `shopify.saveBar.show()`, `shopify.saveBar.hide()`, and `shopify.saveBar.toggle()`.

**Note:** The save bar functionality requires the full App Bridge UI library to be loaded via a [script tag](https://shopify.dev/docs/api/app-home/using-polaris-components).

## Enable save bar on forms

Simply add `data-save-bar` to your `<form>` element:

```html
<form data-save-bar>
  <!-- Your form fields -->
</form>
```

- **data-discard-confirmation**

  **boolean**

- **data-save-bar**

  **boolean**

- **onreset**

  **(event: Event) => void**

- **onsubmit**

  **(event: SubmitEvent) => void**

Examples

## Preview

![](https://cdn.shopify.com/shopifycloud/shopify-dev/development/assets/assets/images/templated-apis-screenshots/admin/app-bridge-web-components/forms-alt-BR0YaM5v.png)

### Examples

- #### Form with automatic save bar

  ##### Default

  ```html
  <form data-save-bar>
    <s-text-field
      label="Product Title"
      name="title"
      required
    ></s-text-field>

    <s-text-area
      label="Description"
      name="description"
      rows="4"
    ></s-text-area>

    <s-text-field
      label="Price"
      name="price"
      type="number"
      step="0.01"
      min="0"
    ></s-text-field>
  </form>
  ```

- #### Simple form with save bar

  ##### Description

  Basic form with automatic save bar functionality.

  ##### Default

  ```html
  <form data-save-bar>
    <s-text-field
      label="Product Title"
      name="title"
      required
    ></s-text-field>

    <s-text-area
      label="Description"
      name="description"
      rows="4"
    ></s-text-area>

    <s-text-field
      label="Price"
      name="price"
      type="number"
      step="0.01"
      min="0"
    ></s-text-field>
  </form>
  ```

- #### Programmatic save bar control

  ##### Description

  Using the programmatic API for custom save logic.

  ##### Default

  ```html
  <form id="custom-form">
    <s-text-field
      id="settings-name"
      label="Store Name"
    ></s-text-field>

    <s-checkbox
      id="settings-notifications"
      label="Enable email notifications"
    ></s-checkbox>
  </form>

  <script>
    // Track form changes manually
    const form = document.getElementById('custom-form');
    let hasChanges = false;

    form.addEventListener('input', () => {
      if (!hasChanges) {
        hasChanges = true;
        // Show save bar programmatically
        shopify.saveBar.show({
          onSave: async () => {
            // Custom save logic
            console.log('Saving form data...');
            // Simulate API call
            await new Promise(resolve => setTimeout(resolve, 1000));
            hasChanges = false;
            shopify.saveBar.hide();
          },
          onDiscard: () => {
            // Reset form
            form.reset();
            hasChanges = false;
            shopify.saveBar.hide();
          }
        });
      }
    });
  </script>
  ```

- #### Form with onsubmit and onreset events

  ##### Description

  Using the onsubmit and onreset events to handle form submission and reset.

  ##### Default

  ```html
  <form
    data-save-bar
    onsubmit="console.log('submit');"
    onreset="console.log('reset');"
  >
    <s-text-field label="Name" name="name"></s-text-field>
    <s-button type="submit">Submit</s-button>
    <s-button type="reset">Reset</s-button>
  </form>
  ```

## Related

[API - Save Bar](https://shopify.dev/docs/api/app-home/apis/save-bar)

</page>

<page>
---
title: Title bar
description: >-
  The admin title bar is a critical part of the Shopify Admin experience. It
  provides a way to display the current page title and actions for the user to
  take. This guide will show you how to work with the admin title bar using the
  App Bridge UI library.
api_name: app-home
source_url:
  html: 'https://shopify.dev/docs/api/app-home/app-bridge-web-components/title-bar'
  md: 'https://shopify.dev/docs/api/app-home/app-bridge-web-components/title-bar.md'
---

# Title bar

The admin title bar is a critical part of the Shopify Admin experience. It provides a way to display the current page title and actions for the user to take. This guide will show you how to work with the admin title bar using the App Bridge UI library.

## Use the s-page component

The `s-page` component is available for use in your app. It configures the title bar in the Shopify admin in addition to managing the page layout. Note that you do not need the full App Bridge UI library to use this component. You can still use `s-page` (and its required child components) in your app.

- **children**

  **SPageChildren**

- **heading**

  **string**

### SPageChildren

- breadcrumbActions

  ```ts
  HTMLElement
  ```

- primaryAction

  ```ts
  HTMLElement
  ```

- secondaryActions

  ```ts
  HTMLElement[]
  ```

```ts
interface SPageChildren {
  primaryAction?: HTMLElement;
  secondaryActions?: HTMLElement[];
  breadcrumbActions?: HTMLElement;
}
```

Examples

## Preview

![](https://cdn.shopify.com/shopifycloud/shopify-dev/development/assets/assets/images/templated-apis-screenshots/admin/app-bridge-web-components/title-bar-app-home-BfbrPNeF.png)

### Examples

- #### Simple s-page component

  ##### Default

  ```html
  <s-page heading="Page Title">
    <s-button slot="primary-action" onclick="shopify.toast.show('Save')">Save</s-button>
    <s-button slot="secondary-actions" onclick="shopify.toast.show('Close')">Close</s-button>
    <s-button slot="secondary-actions" onclick="shopify.toast.show('Cancel')">Cancel</s-button>
  </s-page>
  ```

- #### Simple s-page component

  ##### Description

  The \`s-page\` component accepts the following properties: - \`heading\`: The heading for the page. This is the title of the page. And the following slots: - \`primary-action\`: The primary action for the page. This is the main action for the page. - \`secondary-actions\`: The secondary actions for the page. This is a group of actions that are related to the page. - \`breadcrumb-actions\`: The breadcrumb actions for the page. This is a link to the home page or the previous page. - \`accessory\`: A status badge displayed next to the title. Use with \`s-badge\` and the \`tone\` attribute (\`info\`, \`success\`, \`warning\`, or \`critical\`).

  ##### Default

  ```html
  <s-page heading="Page Title">
    <s-button slot="primary-action" onclick="shopify.toast.show('Save')">Save</s-button>
    <s-button slot="secondary-actions" onclick="shopify.toast.show('Close')">Close</s-button>
    <s-button slot="secondary-actions" onclick="shopify.toast.show('Cancel')">Cancel</s-button>
  </s-page>
  ```

- #### Accessory badge

  ##### Description

  Display a status badge next to the page title using the \`accessory\` slot. Use \`s-badge\` with the \`tone\` attribute to indicate status (\`info\`, \`success\`, \`warning\`, or \`critical\`).

  ##### Default

  ```html
  <s-page heading="Edit Product">
    <s-badge slot="accessory" tone="warning">Draft</s-badge>
    <s-button slot="primary-action">Save</s-button>
  </s-page>
  ```

- #### Grouped secondary actions

  ##### Description

  You can group secondary actions together using \`s-menu\` and the \`commandfor\` attribute. This will create a dropdown menu with the actions. The text content of the \`s-button\` used with the \`commandfor\` attribute will display a label for the group of actions.

  ##### Default

  ```html
  <s-page heading="Page Title">
    <s-button slot="primary-action" onclick="shopify.toast.show('Save')">Save</s-button>
    <s-button slot="secondary-actions" commandfor="more-actions-id">More actions</s-button>
    <s-menu id="more-actions-id">
      <s-button onclick="shopify.toast.show('Action 1')">Action 1</s-button>
      <s-button onclick="shopify.toast.show('Action 2')">Action 2</s-button>
      <s-button onclick="shopify.toast.show('Action 3')">Action 3</s-button>
    </s-menu>
  </s-page>
  ```

- #### Breadcrumb actions

  ##### Description

  You can add breadcrumb actions using the \`breadcrumb-actions\` slot. This will add a link to the breadcrumb actions. You can use this to add a link to the home page or to add a link to the previous page.

  ##### Default

  ```html
  <s-page heading="Page Title">
    <s-button slot="primary-action" onclick="shopify.toast.show('Save')">Save</s-button>
    <s-button slot="secondary-actions" onclick="shopify.toast.show('Cancel')">Cancel</s-button>
    <s-link slot="breadcrumb-actions" href="/">Home</s-link>
  </s-page>
  ```

- #### Complete example

  ##### Description

  Here is a complete example of how to use the \`s-page\` component to interact with the admin title bar.

  ##### Default

  ```html
  <s-page heading="Page Title">
    <s-button slot="primary-action" onclick="shopify.toast.show('Save')">Save</s-button>
    <s-button slot="secondary-actions" onclick="shopify.toast.show('Close')">Close</s-button>
    <s-button slot="secondary-actions" commandfor="more-actions-id">More actions</s-button>
    <s-menu id="more-actions-id">
      <s-button onclick="shopify.toast.show('Action 1')">Action 1</s-button>
      <s-button onclick="shopify.toast.show('Action 2')">Action 2</s-button>
      <s-button onclick="shopify.toast.show('Action 3')">Action 3</s-button>
    </s-menu>
    <s-link slot="breadcrumb-actions" href="/">Home</s-link>
  </s-page>
  ```

## Related

[API - Navigation](https://shopify.dev/docs/api/app-home/apis/navigation)

</page>

<page>
---
title: Account connection
description: >
  The account connection component is used so merchants can connect or
  disconnect their store to various accounts. For example, if merchants want to
  use the Facebook sales channel, they need to connect their Facebook account to
  their Shopify store.

    | Used to | Examples |
    | --- | --- |
    | Display connection status | Show if a sales channel is connected or disconnected |
    | Allow merchants to disconnect accounts  | Enable merchants to disconnect from a marketing platform |

    ---
    

    
api_name: app-home
source_url:
  html: >-
    <https://shopify.dev/docs/api/app-home/patterns/compositions/account-connection>
  md: >-
    <https://shopify.dev/docs/api/app-home/patterns/compositions/account-connection.md>
---

# Account connection

The account connection component is used so merchants can connect or disconnect their store to various accounts. For example, if merchants want to use the Facebook sales channel, they need to connect their Facebook account to their Shopify store.

| Used to | Examples |
| - | - |
| Display connection status | Show if a sales channel is connected or disconnected |
| Allow merchants to disconnect accounts | Enable merchants to disconnect from a marketing platform |

***

Examples

### Examples

- #### Account connection

  ##### jsx

  ```jsx
  <s-section>
    <s-stack gap="base">
      <s-grid gridTemplateColumns="1fr auto" gap="base" alignItems="center">
        <s-grid-item>
          <s-stack>
            <s-heading>Puzzlify</s-heading>
            <s-text color="subdued">No account connected</s-text>
          </s-stack>
        </s-grid-item>
        <s-grid-item>
          <s-button variant="primary">Connect</s-button>
        </s-grid-item>
      </s-grid>
      <s-text>
        By clicking Connect, you agree to accept Sample App's terms and
        conditions. You'll pay a commission rate of 15% on sales made through
        Sample App.
      </s-text>
    </s-stack>
  </s-section>
  ```

  ##### html

  ```html
  <s-section>
    <s-stack gap="base">
    <s-grid gridTemplateColumns="1fr auto" gap="base" alignItems="center">
      <s-grid-item>
        <s-stack>
          <s-heading>Puzzlify</s-heading>
          <s-text color="subdued">No account connected</s-text>
        </s-stack>
      </s-grid-item>
      <s-grid-item>
        <s-button variant="primary">Connect</s-button>
      </s-grid-item>
    </s-grid>
    <s-text>By clicking Connect, you agree to accept Sample App's terms and conditions. You'll pay a commission rate of 15% on sales made through Sample App.</s-text>
  </s-stack>
  </s-section>
  ```

## Related

[Requirements - Built for Shopify](https://shopify.dev/docs/apps/launch/built-for-shopify/requirements)

</page>

<page>
---
title: App card
description: >
  App cards provide a consistent layout for presenting another app to merchants.
  They are used to highlight apps that can extend functionality or help
  merchants accomplish related tasks. App cards should educate merchants about
  the value of the suggested app and provide a clear, actionable way to learn
  more or install it.

    | Used to | Examples |
    | --- | --- |
    | Suggest complementary apps | Recommend an email marketing app to subscription service users |
    | Promote integrations | Highlight a social media scheduler that connects with your app |
    | Guide merchants to explore new solutions | Introduce a reporting tool for advanced analytics |

    ---
    

    
api_name: app-home
source_url:
  html: '<https://shopify.dev/docs/api/app-home/patterns/compositions/app-card>'
  md: '<https://shopify.dev/docs/api/app-home/patterns/compositions/app-card.md>'
---

# App card

App cards provide a consistent layout for presenting another app to merchants. They are used to highlight apps that can extend functionality or help merchants accomplish related tasks. App cards should educate merchants about the value of the suggested app and provide a clear, actionable way to learn more or install it.

| Used to | Examples |
| - | - |
| Suggest complementary apps | Recommend an email marketing app to subscription service users |
| Promote integrations | Highlight a social media scheduler that connects with your app |
| Guide merchants to explore new solutions | Introduce a reporting tool for advanced analytics |

***

Examples

### Examples

- #### App card

  ##### jsx

  ```jsx
  <s-clickable
    href="https://apps.shopify.com/planet"
    border="base"
    borderRadius="base"
    padding="base"
    inlineSize="100%"
  >
    <s-grid gridTemplateColumns="auto 1fr auto" alignItems="stretch" gap="base">
      <s-thumbnail
        size="small"
        src="https://cdn.shopify.com/app-store/listing_images/87176a11f3714753fdc2e1fc8bbf0415/icon/CIqiqqXsiIADEAE=.png"
        alt="Shopify Planet icon"
       />
      <s-box>
        <s-heading>Shopify Planet</s-heading>
        <s-paragraph>Free</s-paragraph>
        <s-paragraph>
          Offer carbon-neutral shipping and showcase your commitment.
        </s-paragraph>
      </s-box>
      <s-stack justifyContent="start">
        <s-button
          href="https://apps.shopify.com/planet"
          icon="download"
          accessibilityLabel="Download Shopify Planet"
         />
      </s-stack>
    </s-grid>
  </s-clickable>
  ```

  ##### html

  ```html
  <s-clickable
    href="https://apps.shopify.com/planet"
    border="base"
    borderRadius="base"
    padding="base"
    inlineSize="100%"
  >
    <s-grid gridTemplateColumns="auto 1fr auto" alignItems="stretch" gap="base">
        <s-thumbnail
          size="small"
          src="https://cdn.shopify.com/app-store/listing_images/87176a11f3714753fdc2e1fc8bbf0415/icon/CIqiqqXsiIADEAE=.png"
          alt="Shopify Planet icon"
        ></s-thumbnail>
      <s-box>
        <s-heading>Shopify Planet</s-heading>
        <s-paragraph>Free</s-paragraph>
        <s-paragraph>
          Offer carbon-neutral shipping and showcase your commitment.
        </s-paragraph>
      </s-box>
      <s-stack justifyContent="start">
        <s-button
          href="https://apps.shopify.com/planet"
          icon="download"
          accessibilityLabel="Download Shopify Planet"
        ></s-button>
      </s-stack>
    </s-grid>
  </s-clickable>
  ```

## Related

[Requirements - Built for Shopify](https://shopify.dev/docs/apps/launch/built-for-shopify/requirements)

</page>

<page>
---
title: Callout card
description: >
  Callout cards are used to encourage merchants to take an action related to a
  new feature or opportunity. They are most commonly displayed in the sales
  channels section of Shopify.

    | Used to | Examples |
    | --- | --- |
    | Promote new features or integrations | Dismissible feature announcement |
    | Drive adoption of app functionality | Common first actions |
    ---

api_name: app-home
source_url:
  html: '<https://shopify.dev/docs/api/app-home/patterns/compositions/callout-card>'
  md: '<https://shopify.dev/docs/api/app-home/patterns/compositions/callout-card.md>'
---

# Callout card

Callout cards are used to encourage merchants to take an action related to a new feature or opportunity. They are most commonly displayed in the sales channels section of Shopify.

| Used to | Examples |
| - | - |
| Promote new features or integrations | Dismissible feature announcement |
| Drive adoption of app functionality | Common first actions |

***

Examples

### Examples

- #### Callout card

  ##### jsx

  ```jsx
  <s-section>
    <s-grid gridTemplateColumns="1fr auto" gap="small-400" alignItems="start">
      <s-grid
        gridTemplateColumns="@container (inline-size <= 480px) 1fr, auto auto"
        gap="base"
        alignItems="center"
      >
        <s-grid gap="small-200">
          <s-heading>Ready to create your custom puzzle?</s-heading>
          <s-paragraph>
            Start by uploading an image to your gallery or choose from one of our
            templates.
          </s-paragraph>
          <s-stack direction="inline" gap="small-200">
            <s-button> Upload image </s-button>
            <s-button tone="neutral" variant="tertiary">
              {" "}
              Browse templates{" "}
            </s-button>
          </s-stack>
        </s-grid>
        <s-stack alignItems="center">
          <s-box maxInlineSize="200px" borderRadius="base" overflow="hidden">
            <s-image
              src="https://cdn.shopify.com/static/images/polaris/patterns/callout.png"
              alt="Customize checkout illustration"
              aspectRatio="1/0.5"
             />
          </s-box>
        </s-stack>
      </s-grid>
      <s-button
        icon="x"
        tone="neutral"
        variant="tertiary"
        accessibilityLabel="Dismiss card"
       />
    </s-grid>
  </s-section>
  ```

  ##### html

  ```html
  <s-section>
    <s-grid gridTemplateColumns="1fr auto" gap="small-400" alignItems="start">
      <s-grid
        gridTemplateColumns="@container (inline-size <= 480px) 1fr, auto auto"
        gap="base"
        alignItems="center"
      >
        <s-grid gap="small-200">
          <s-heading>Ready to create your custom puzzle?</s-heading>
          <s-paragraph>
            Start by uploading an image to your gallery or choose from one of our templates.
          </s-paragraph>
          <s-stack direction="inline" gap="small-200">
            <s-button> Upload image </s-button>
            <s-button tone="neutral" variant="tertiary"> Browse templates </s-button>
          </s-stack>
        </s-grid>
        <s-stack alignItems="center">
          <s-box maxInlineSize="200px" borderRadius="base" overflow="hidden">
            <s-image
              src="https://cdn.shopify.com/static/images/polaris/patterns/callout.png"
              alt="Customize checkout illustration"
              aspectRatio="1/0.5"
            ></s-image>
          </s-box>
        </s-stack>
      </s-grid>
      <s-button
        icon="x"
        tone="neutral"
        variant="tertiary"
        accessibilityLabel="Dismiss card"
      ></s-button>
    </s-grid>
  </s-section>
  ```

## Related

[Requirements - Built for Shopify](https://shopify.dev/docs/apps/launch/built-for-shopify/requirements)

</page>

<page>
---
title: Empty state
description: >
  Empty states are used when a list, table, or chart has no items or data to
  show. This is an opportunity to provide explanation or guidance to help
  merchants progress. The empty state component is intended for use when a full
  page in the admin is empty, and not for individual elements or areas in the
  interface.

    | Used to | Examples |
    | --- | --- |
    | Offer a clear next step when no data is present | Prompt merchants to create their first campaign |
    | Encourage activation of features | Suggest setting up a subscription plan when none exist |
    ---

api_name: app-home
source_url:
  html: '<https://shopify.dev/docs/api/app-home/patterns/compositions/empty-state>'
  md: '<https://shopify.dev/docs/api/app-home/patterns/compositions/empty-state.md>'
---

# Empty state

Empty states are used when a list, table, or chart has no items or data to show. This is an opportunity to provide explanation or guidance to help merchants progress. The empty state component is intended for use when a full page in the admin is empty, and not for individual elements or areas in the interface.

| Used to | Examples |
| - | - |
| Offer a clear next step when no data is present | Prompt merchants to create their first campaign |
| Encourage activation of features | Suggest setting up a subscription plan when none exist |

***

Examples

### Examples

- #### Empty state

  ##### jsx

  ```jsx
  <s-section accessibilityLabel="Empty state section">
    <s-grid gap="base" justifyItems="center" paddingBlock="large-400">
      <s-box maxInlineSize="200px" maxBlockSize="200px">
        {/* aspectRatio should match the actual image dimensions (width/height) */}
        <s-image
          aspectRatio="1/0.5"
          src="https://cdn.shopify.com/static/images/polaris/patterns/callout.png"
          alt="A stylized graphic of four characters, each holding a puzzle piece"
        />
      </s-box>
      <s-grid justifyItems="center" maxInlineSize="450px" gap="base">
        <s-stack alignItems="center">
          <s-heading>Start creating puzzles</s-heading>
          <s-paragraph>
            Create and manage your collection of puzzles for players to enjoy.
          </s-paragraph>
        </s-stack>
        <s-button-group>
          <s-button
            slot="secondary-actions"
            aria-label="Learn more about creating puzzles"
          >
            {" "}
            Learn more{" "}
          </s-button>
          <s-button slot="primary-action" aria-label="Add a new puzzle">
            {" "}
            Create puzzle{" "}
          </s-button>
        </s-button-group>
      </s-grid>
    </s-grid>
  </s-section>
  ```

  ##### html

  ```html
  <s-section accessibilityLabel="Empty state section">
    <s-grid gap="base" justifyItems="center" paddingBlock="large-400">
      <s-box maxInlineSize="200px" maxBlockSize="200px">
        <!-- aspectRatio should match the actual image dimensions (width/height) -->
        <s-image
          aspectRatio="1/0.5"
          src="https://cdn.shopify.com/static/images/polaris/patterns/callout.png"
          alt="A stylized graphic of four characters, each holding a puzzle piece"
        />
      </s-box>
      <s-grid
        justifyItems="center"
        maxInlineSize="450px"
        gap="base"
      >
      <s-stack alignItems="center">
        <s-heading>Start creating puzzles</s-heading>
        <s-paragraph>
          Create and manage your collection of puzzles for players to enjoy.
        </s-paragraph>
      </s-stack>
      <s-button-group>
        <s-button slot="secondary-actions" aria-label="Learn more about creating puzzles"> Learn more </s-button>
        <s-button slot="primary-action" aria-label="Add a new puzzle"> Create puzzle </s-button>
      </s-button-group>
      </s-grid>
    </s-grid>
  </s-section>
  ```

## Related

[Requirements - Built for Shopify](https://shopify.dev/docs/apps/launch/built-for-shopify/requirements)

</page>

<page>
---
title: Footer help
description: >
  Footer help is used to refer merchants to more information related to the
  product or feature they’re using.

    | Used to | Examples |
    | --- | --- |
    | Refer merchants to related help docs |  Learn more about [shipping zones]|
    | Offer support as a secondary option  | [Contact us] about email marketing |

    ---
    

    
api_name: app-home
source_url:
  html: '<https://shopify.dev/docs/api/app-home/patterns/compositions/footer-help>'
  md: '<https://shopify.dev/docs/api/app-home/patterns/compositions/footer-help.md>'
---

# Footer help

Footer help is used to refer merchants to more information related to the product or feature they’re using.

| Used to | Examples |
| - | - |
| Refer merchants to related help docs | Learn more about \[shipping zones] |
| Offer support as a secondary option | \[Contact us] about email marketing |

***

Examples

### Examples

- #### Footer help

  ##### jsx

  ```jsx
  <s-stack alignItems="center">
    <s-text>Learn more about <s-link href="">creating puzzles</s-link>.</s-text>
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack alignItems="center">
    <s-text>Learn more about <s-link href="">creating puzzles</s-link>.</s-text>
  </s-stack>
  ```

## Related

[Requirements - Built for Shopify](https://shopify.dev/docs/apps/launch/built-for-shopify/requirements)

</page>

<page>
---
title: Index table
description: >-
  An index table displays a collection of objects of the same type, like orders
  or products. The main job of an index table is to help merchants get an
  at-a-glance of the objects to perform actions or navigate to a full-page
  representation of it.
    | Used to | Examples |
    | --- | --- |
    | Display collections of similar objects | Products, orders, customers, discounts |
    | Perform bulk actions | Delete products, pause/activate campaigns |
    ---

api_name: app-home
source_url:
  html: '<https://shopify.dev/docs/api/app-home/patterns/compositions/index-table>'
  md: '<https://shopify.dev/docs/api/app-home/patterns/compositions/index-table.md>'
---

# Index table

An index table displays a collection of objects of the same type, like orders or products. The main job of an index table is to help merchants get an at-a-glance of the objects to perform actions or navigate to a full-page representation of it.

| Used to | Examples |
| - | - |
| Display collections of similar objects | Products, orders, customers, discounts |
| Perform bulk actions | Delete products, pause/activate campaigns |

***

Examples

### Examples

- #### Index table

  ##### jsx

  ```jsx
  <s-section padding="none" accessibilityLabel="Puzzles table section">
    <s-table>
      <s-grid slot="filters" gap="small-200" gridTemplateColumns="1fr auto">
        <s-text-field
          label="Search puzzles"
          labelAccessibilityVisibility="exclusive"
          icon="search"
          placeholder="Searching all puzzles"
         />
        <s-button
          icon="sort"
          variant="secondary"
          accessibilityLabel="Sort"
          interestFor="sort-tooltip"
          commandFor="sort-actions"
         />
        <s-tooltip id="sort-tooltip">
          <s-text>Sort</s-text>
        </s-tooltip>
        <s-popover id="sort-actions">
          <s-stack gap="none">
            <s-box padding="small">
              <s-choice-list label="Sort by" name="Sort by">
                <s-choice value="puzzle-name" selected>
                  Puzzle name
                </s-choice>
                <s-choice value="pieces">Pieces</s-choice>
                <s-choice value="created">Created</s-choice>
                <s-choice value="status">Status</s-choice>
              </s-choice-list>
            </s-box>
            <s-divider />
            <s-box padding="small">
              <s-choice-list label="Order by" name="Order by">
                <s-choice value="product-title" selected>
                  A-Z
                </s-choice>
                <s-choice value="created">Z-A</s-choice>
              </s-choice-list>
            </s-box>
          </s-stack>
        </s-popover>
      </s-grid>
      <s-table-header-row>
        <s-table-header listSlot="primary">Puzzle</s-table-header>
        <s-table-header format="numeric">Pieces</s-table-header>
        <s-table-header>Created</s-table-header>
        <s-table-header listSlot="secondary">Status</s-table-header>
      </s-table-header-row>
      <s-table-body>
        <s-table-row clickDelegate="mountain-view-checkbox">
          <s-table-cell>
            <s-stack direction="inline" gap="small" alignItems="center">
              <s-checkbox id="mountain-view-checkbox" />
              <s-clickable
                href=""
                accessibilityLabel="Mountain View puzzle thumbnail"
                border="base"
                borderRadius="base"
                overflow="hidden"
                inlineSize="40px"
                blockSize="40px"
              >
                <s-image
                  objectFit="cover"
                  src="https://picsum.photos/id/29/80/80"
                 />
              </s-clickable>
              <s-link href="">Mountain View</s-link>
            </s-stack>
          </s-table-cell>
          <s-table-cell>16</s-table-cell>
          <s-table-cell>Today</s-table-cell>
          <s-table-cell>
            <s-badge color="base" tone="success">
              Active
            </s-badge>
          </s-table-cell>
        </s-table-row>
        <s-table-row clickDelegate="ocean-sunset-checkbox">
          <s-table-cell>
            <s-stack direction="inline" gap="small" alignItems="center">
              <s-checkbox id="ocean-sunset-checkbox" />
              <s-clickable
                href=""
                accessibilityLabel="Ocean Sunset puzzle thumbnail"
                border="base"
                borderRadius="base"
                overflow="hidden"
                inlineSize="40px"
                blockSize="40px"
              >
                <s-image
                  objectFit="cover"
                  src="https://picsum.photos/id/12/80/80"
                 />
              </s-clickable>
              <s-link href="">Ocean Sunset</s-link>
            </s-stack>
          </s-table-cell>
          <s-table-cell>9</s-table-cell>
          <s-table-cell>Yesterday</s-table-cell>
          <s-table-cell>
            <s-badge color="base" tone="success">
              Active
            </s-badge>
          </s-table-cell>
        </s-table-row>
        <s-table-row clickDelegate="forest-animals-checkbox">
          <s-table-cell>
            <s-stack direction="inline" gap="small" alignItems="center">
              <s-checkbox id="forest-animals-checkbox" />
              <s-clickable
                href=""
                accessibilityLabel="Forest Animals puzzle thumbnail"
                border="base"
                borderRadius="base"
                overflow="hidden"
                inlineSize="40px"
                blockSize="40px"
              >
                <s-image
                  objectFit="cover"
                  src="https://picsum.photos/id/324/80/80"
                 />
              </s-clickable>
              <s-link href="">Forest Animals</s-link>
            </s-stack>
          </s-table-cell>
          <s-table-cell>25</s-table-cell>
          <s-table-cell>Last week</s-table-cell>
          <s-table-cell>
            <s-badge color="base" tone="neutral">
              Draft
            </s-badge>
          </s-table-cell>
        </s-table-row>
      </s-table-body>
    </s-table>
  </s-section>
  ```

  ##### html

  ```html
  <s-section padding="none" accessibilityLabel="Puzzles table section">
    <s-table>
      <s-grid slot="filters" gap="small-200" gridTemplateColumns="1fr auto">
        <s-text-field
          label="Search puzzles"
          labelAccessibilityVisibility="exclusive"
          icon="search"
          placeholder="Searching all puzzles"
        ></s-text-field>
        <s-button
          icon="sort"
          variant="secondary"
          accessibilityLabel="Sort"
          interestFor="sort-tooltip"
          commandFor="sort-actions"
        ></s-button>
        <s-tooltip id="sort-tooltip">
          <s-text>Sort</s-text>
        </s-tooltip>
        <s-popover id="sort-actions">
          <s-stack gap="none">
            <s-box padding="small">
              <s-choice-list label="Sort by" name="Sort by">
                <s-choice value="puzzle-name" selected>Puzzle name</s-choice>
                <s-choice value="pieces">Pieces</s-choice>
                <s-choice value="created">Created</s-choice>
                <s-choice value="status">Status</s-choice>
              </s-choice-list>
            </s-box>
            <s-divider></s-divider>
            <s-box padding="small">
              <s-choice-list label="Order by" name="Order by">
                <s-choice value="product-title" selected>A-Z</s-choice>
                <s-choice value="created">Z-A</s-choice>
              </s-choice-list>
            </s-box>
          </s-stack>
        </s-popover>
      </s-grid>
      <s-table-header-row>
        <s-table-header listSlot="primary">Puzzle</s-table-header>
        <s-table-header format="numeric">Pieces</s-table-header>
        <s-table-header>Created</s-table-header>
        <s-table-header listSlot="secondary">Status</s-table-header>
      </s-table-header-row>
      <s-table-body>
        <s-table-row clickDelegate="mountain-view-checkbox">
          <s-table-cell>
            <s-stack direction="inline" gap="small" alignItems="center">
              <s-checkbox id="mountain-view-checkbox"></s-checkbox>
              <s-clickable
                href=""
                accessibilityLabel="Mountain View puzzle thumbnail"
                border="base"
                borderRadius="base"
                overflow="hidden"
                inlineSize="40px"
                blockSize="40px"
              >
                <s-image
                  objectFit="cover"
                  src="https://picsum.photos/id/29/80/80"
                ></s-image>
              </s-clickable>
              <s-link href="">Mountain View</s-link>
            </s-stack>
          </s-table-cell>
          <s-table-cell>16</s-table-cell>
          <s-table-cell>Today</s-table-cell>
          <s-table-cell>
            <s-badge color="base" tone="success">Active</s-badge>
          </s-table-cell>
        </s-table-row>
        <s-table-row clickDelegate="ocean-sunset-checkbox">
          <s-table-cell>
            <s-stack direction="inline" gap="small" alignItems="center">
              <s-checkbox id="ocean-sunset-checkbox"></s-checkbox>
              <s-clickable
                href=""
                accessibilityLabel="Ocean Sunset puzzle thumbnail"
                border="base"
                borderRadius="base"
                overflow="hidden"
                inlineSize="40px"
                blockSize="40px"
              >
                <s-image
                  objectFit="cover"
                  src="https://picsum.photos/id/12/80/80"
                ></s-image>
              </s-clickable>
              <s-link href="">Ocean Sunset</s-link>
            </s-stack>
          </s-table-cell>
          <s-table-cell>9</s-table-cell>
          <s-table-cell>Yesterday</s-table-cell>
          <s-table-cell>
            <s-badge color="base" tone="success">Active</s-badge>
          </s-table-cell>
        </s-table-row>
        <s-table-row clickDelegate="forest-animals-checkbox">
          <s-table-cell>
            <s-stack direction="inline" gap="small" alignItems="center">
              <s-checkbox id="forest-animals-checkbox"></s-checkbox>
              <s-clickable
                href=""
                accessibilityLabel="Forest Animals puzzle thumbnail"
                border="base"
                borderRadius="base"
                overflow="hidden"
                inlineSize="40px"
                blockSize="40px"
              >
                <s-image
                  objectFit="cover"
                  src="https://picsum.photos/id/324/80/80"
                ></s-image>
              </s-clickable>
              <s-link href="">Forest Animals</s-link>
            </s-stack>
          </s-table-cell>
          <s-table-cell>25</s-table-cell>
          <s-table-cell>Last week</s-table-cell>
          <s-table-cell>
            <s-badge color="base" tone="neutral">Draft</s-badge>
          </s-table-cell>
        </s-table-row>
      </s-table-body>
    </s-table>
  </s-section>
  ```

## Related

[Requirements - Built for Shopify](https://shopify.dev/docs/apps/launch/built-for-shopify/requirements)

[Component - Table](https://shopify.dev/docs/api/app-home/polaris-web-components/structure/table)

</page>

<page>
---
title: Interstitial nav
description: >
  Interstitial navigation is used to connect merchants to deeper pages—such as
  settings, features, or resources—within a section of your app. It helps keep
  navigation clean and focused by avoiding multiple nested items, making it
  easier for merchants to discover and access important functionality.

    | Used to | Examples |
    | --- | --- |
    | Link to individual settings pages | Navigate from a settings overview to product settings or notification preferences |
    | Connect to feature-specific pages | Direct merchants from campaign overview to reporting or automation setup |
    | Guide merchants to supporting resources | Link to help documentation or integration guides from a central section |
    | Simplify navigation structure | Reduce clutter by providing access to deeper pages without multi-level menus |
    ---

api_name: app-home
source_url:
  html: '<https://shopify.dev/docs/api/app-home/patterns/compositions/interstitial-nav>'
  md: >-
    <https://shopify.dev/docs/api/app-home/patterns/compositions/interstitial-nav.md>
---

# Interstitial nav

Interstitial navigation is used to connect merchants to deeper pages—such as settings, features, or resources—within a section of your app. It helps keep navigation clean and focused by avoiding multiple nested items, making it easier for merchants to discover and access important functionality.

| Used to | Examples |
| - | - |
| Link to individual settings pages | Navigate from a settings overview to product settings or notification preferences |
| Connect to feature-specific pages | Direct merchants from campaign overview to reporting or automation setup |
| Guide merchants to supporting resources | Link to help documentation or integration guides from a central section |
| Simplify navigation structure | Reduce clutter by providing access to deeper pages without multi-level menus |

***

Examples

### Examples

- #### Interstitial nav

  ##### jsx

  ```jsx
  <s-section heading="Preferences">
    <s-box border="base" borderRadius="base">
      <s-clickable
        padding="small-100"
        href="#"
        accessibilityLabel="Configure shipping methods, rates, and fulfillment options"
      >
        <s-grid gridTemplateColumns="1fr auto" alignItems="center" gap="base">
          <s-box>
            <s-heading>Shipping & fulfillment</s-heading>
            <s-paragraph color="subdued">
              Shipping methods, rates, zones, and fulfillment preferences.
            </s-paragraph>
          </s-box>
          <s-icon type="chevron-right" />
        </s-grid>
      </s-clickable>
      <s-box paddingInline="small-100">
        <s-divider />
      </s-box>
      <s-clickable
        padding="small-100"
        href="#"
        accessibilityLabel="Configure product defaults, customer experience, and catalog settings"
      >
        <s-grid gridTemplateColumns="1fr auto" alignItems="center" gap="base">
          <s-box>
            <s-heading>Products & catalog</s-heading>
            <s-paragraph color="subdued">
              Product defaults, customer experience, and catalog display options.
            </s-paragraph>
          </s-box>
          <s-icon type="chevron-right" />
        </s-grid>
      </s-clickable>
      <s-box paddingInline="small-100">
        <s-divider />
      </s-box>
      <s-clickable
        padding="small-100"
        href="#"
        accessibilityLabel="Manage customer support settings and help resources"
      >
        <s-grid gridTemplateColumns="1fr auto" alignItems="center" gap="base">
          <s-box>
            <s-heading>Customer support</s-heading>
            <s-paragraph color="subdued">
              Support settings, help resources, and customer service tools.
            </s-paragraph>
          </s-box>
          <s-icon type="chevron-right" />
        </s-grid>
      </s-clickable>
    </s-box>
  </s-section>
  ```

  ##### html

  ```html
  <s-section heading="Preferences">
    <s-box
      border="base"
      borderRadius="base"
    >
      <s-clickable
        padding="small-100"
        href="#"
        accessibilityLabel="Configure shipping methods, rates, and fulfillment options"
      >
        <s-grid
          gridTemplateColumns="1fr auto"
          alignItems="center"
          gap="base"
        >
          <s-box>
            <s-heading>Shipping & fulfillment</s-heading>
            <s-paragraph color="subdued">
              Shipping methods, rates, zones, and fulfillment preferences.
            </s-paragraph>
          </s-box>
          <s-icon type="chevron-right"></s-icon>
        </s-grid>
      </s-clickable>
      <s-box paddingInline="small-100">
        <s-divider></s-divider>
      </s-box>
      <s-clickable
        padding="small-100"
        href="#"
        accessibilityLabel="Configure product defaults, customer experience, and catalog settings"
      >
        <s-grid
          gridTemplateColumns="1fr auto"
          alignItems="center"
          gap="base"
        >
          <s-box>
            <s-heading>Products & catalog</s-heading>
            <s-paragraph color="subdued">
              Product defaults, customer experience, and catalog display
              options.
            </s-paragraph>
          </s-box>
          <s-icon type="chevron-right"></s-icon>
        </s-grid>
      </s-clickable>
      <s-box paddingInline="small-100">
        <s-divider></s-divider>
      </s-box>
      <s-clickable
        padding="small-100"
        href="#"
        accessibilityLabel="Manage customer support settings and help resources"
      >
        <s-grid
          gridTemplateColumns="1fr auto"
          alignItems="center"
          gap="base"
        >
          <s-box>
            <s-heading>Customer support</s-heading>
            <s-paragraph color="subdued">
              Support settings, help resources, and customer service
              tools.
            </s-paragraph>
          </s-box>
          <s-icon type="chevron-right"></s-icon>
        </s-grid>
      </s-clickable>
    </s-box>
  </s-section>
  ```

## Related

[Requirements - Built for Shopify](https://shopify.dev/docs/apps/launch/built-for-shopify/requirements)

</page>

<page>
---
title: Media card
description: >
  Media cards provide a consistent layout to present visual information to
  merchants. Visual media is used to provide additional context to the written
  information it's paired with.

    | Used to | Examples |
    | --- | --- |
    | Educate merchants on key actions | Show how to connect a social account with a demo image |
    | Provide clear calls to action | Show campaign preview with a "Send campaign" button |

    ---
    

    
api_name: app-home
source_url:
  html: '<https://shopify.dev/docs/api/app-home/patterns/compositions/media-card>'
  md: '<https://shopify.dev/docs/api/app-home/patterns/compositions/media-card.md>'
---

# Media card

Media cards provide a consistent layout to present visual information to merchants. Visual media is used to provide additional context to the written information it's paired with.

| Used to | Examples |
| - | - |
| Educate merchants on key actions | Show how to connect a social account with a demo image |
| Provide clear calls to action | Show campaign preview with a "Send campaign" button |

***

Examples

### Examples

- #### Media card

  ##### jsx

  ```jsx
  <s-box
    border="base"
    borderRadius="base"
    overflow="hidden"
    maxInlineSize="216px"
  >
    <s-clickable href="">
      <s-image
        aspectRatio="1/1"
        objectFit="cover"
        alt="Illustration of characters with a 4-piece puzzle"
        src="https://cdn.shopify.com/static/images/polaris/patterns/4-pieces.png"
       />
    </s-clickable>
    <s-divider />
    <s-grid
      gridTemplateColumns="1fr auto"
      background="base"
      padding="small"
      gap="small"
      alignItems="center"
    >
      <s-heading>4-Pieces</s-heading>
      <s-button href="" accessibilityLabel="View 4-pieces puzzle template">
        View
      </s-button>
    </s-grid>
  </s-box>
  ```

  ##### html

  ```html
  <s-box border="base" borderRadius="base" overflow="hidden" maxInlineSize="216px">
    <s-clickable href="">
      <s-image
        aspectRatio="1/1"
        objectFit="cover"
        alt="Illustration of characters with a 4-piece puzzle"
        src="https://cdn.shopify.com/static/images/polaris/patterns/4-pieces.png"
      ></s-image>
    </s-clickable>
    <s-divider></s-divider>
    <s-grid
      gridTemplateColumns="1fr auto"
      background="base"
      padding="small"
      gap="small"
      alignItems="center"
    >
      <s-heading>4-Pieces</s-heading>
      <s-button href="" accessibilityLabel="View 4-pieces puzzle template">
        View
      </s-button>
    </s-grid>
  </s-box>
  ```

## Related

[Requirements - Built for Shopify](https://shopify.dev/docs/apps/launch/built-for-shopify/requirements)

</page>

<page>
---
title: Metrics card
description: >
  Metrics cards are used to highlight important numbers, statistics, or trends
  from your app, so merchants can quickly understand their activity and
  performance.

    | Used to | Examples |
    | --- | --- |
    | Show app-specific metrics | Email open rates, active subscribers |
    | Visualize user engagement | Social media followers, campaign clicks |
    ---

api_name: app-home
source_url:
  html: '<https://shopify.dev/docs/api/app-home/patterns/compositions/metrics-card>'
  md: '<https://shopify.dev/docs/api/app-home/patterns/compositions/metrics-card.md>'
---

# Metrics card

Metrics cards are used to highlight important numbers, statistics, or trends from your app, so merchants can quickly understand their activity and performance.

| Used to | Examples |
| - | - |
| Show app-specific metrics | Email open rates, active subscribers |
| Visualize user engagement | Social media followers, campaign clicks |

***

Examples

### Examples

- #### Metrics card

  ##### jsx

  ```jsx
  <s-section padding="base">
    <s-grid
      gridTemplateColumns="@container (inline-size <= 400px) 1fr, 1fr auto 1fr auto 1fr"
      gap="small"
    >
      <s-clickable
        href=""
        paddingBlock="small-400"
        paddingInline="small-100"
        borderRadius="base"
      >
        <s-grid gap="small-300">
          <s-heading>Total Designs</s-heading>
          <s-stack direction="inline" gap="small-200">
            <s-text>156</s-text>
            <s-badge tone="success" icon="arrow-up">
              {" "}
              12%{" "}
            </s-badge>
          </s-stack>
        </s-grid>
      </s-clickable>
      <s-divider direction="block" />
      <s-clickable
        href=""
        paddingBlock="small-400"
        paddingInline="small-100"
        borderRadius="base"
      >
        <s-grid gap="small-300">
          <s-heading>Units Sold</s-heading>
          <s-stack direction="inline" gap="small-200">
            <s-text>2,847</s-text>
            <s-badge tone="warning">0%</s-badge>
          </s-stack>
        </s-grid>
      </s-clickable>
      <s-divider direction="block" />
      <s-clickable
        href=""
        paddingBlock="small-400"
        paddingInline="small-100"
        borderRadius="base"
      >
        <s-grid gap="small-300">
          <s-heading>Return Rate</s-heading>
          <s-stack direction="inline" gap="small-200">
            <s-text>3.2%</s-text>
            <s-badge tone="critical" icon="arrow-down">
              {" "}
              0.8%{" "}
            </s-badge>
          </s-stack>
        </s-grid>
      </s-clickable>
    </s-grid>
  </s-section>
  ```

  ##### html

  ```html
  <s-section padding="small">
      <s-grid
        gridTemplateColumns="@container (inline-size <= 400px) 1fr, 1fr auto 1fr auto 1fr"
        gap="small"
      >
        <s-clickable
          href=""
          paddingBlock="small-400"
          paddingInline="small-100"
          borderRadius="base"
        >
          <s-grid gap="small-300">
            <s-heading>Total Designs</s-heading>
            <s-stack direction="inline" gap="small-200">
              <s-text>156</s-text>
              <s-badge tone="success" icon="arrow-up"> 12% </s-badge>
            </s-stack>
          </s-grid>
        </s-clickable>
        <s-divider direction="block"></s-divider>
        <s-clickable
          href=""
          paddingBlock="small-400"
          paddingInline="small-100"
          borderRadius="base"
        >
          <s-grid gap="small-300">
            <s-heading>Units Sold</s-heading>
            <s-stack direction="inline" gap="small-200">
              <s-text>2,847</s-text>
              <s-badge tone="warning">0%</s-badge>
            </s-stack>
          </s-grid>
        </s-clickable>
        <s-divider direction="block"></s-divider>
        <s-clickable
          href=""
          paddingBlock="small-400"
          paddingInline="small-100"
          borderRadius="base"
        >
          <s-grid gap="small-300">
            <s-heading>Return Rate</s-heading>
            <s-stack direction="inline" gap="small-200">
              <s-text>3.2%</s-text>
              <s-badge tone="critical" icon="arrow-down"> 0.8% </s-badge>
            </s-stack>
          </s-grid>
        </s-clickable>
      </s-grid>
  </s-section>
  ```

## Related

[Requirements - Built for Shopify](https://shopify.dev/docs/apps/launch/built-for-shopify/requirements)

</page>

<page>
---
title: Resource list
description: >
  A resource list displays a collection of objects of the same type, like
  products or customers. The main job of a resource list is to help merchants
  find an object and navigate to a full-page representation of it.

  .

    | Used to | Examples |
    | --- | --- |
    | Display collections of similar resources |  Campaigns, subscribers, social posts, templates|
    | Help merchants find and select items  | Search subscribers by email; Filter campaigns by status |
    | Perform bulk actions on selected items  | Tag subscribers; Archive campaigns; Publish selected posts |

    ---
    

    
api_name: app-home
source_url:
  html: '<https://shopify.dev/docs/api/app-home/patterns/compositions/resource-list>'
  md: '<https://shopify.dev/docs/api/app-home/patterns/compositions/resource-list.md>'
---

# Resource list

A resource list displays a collection of objects of the same type, like products or customers. The main job of a resource list is to help merchants find an object and navigate to a full-page representation of it. .

| Used to | Examples |
| - | - |
| Display collections of similar resources | Campaigns, subscribers, social posts, templates |
| Help merchants find and select items | Search subscribers by email; Filter campaigns by status |
| Perform bulk actions on selected items | Tag subscribers; Archive campaigns; Publish selected posts |

***

Examples

### Examples

- #### Resource list

  ##### jsx

  ```jsx
  <s-section padding="none">
    <s-stack gap="small-200">
      <s-grid gridTemplateColumns="1fr auto" gap="base" alignItems="center" paddingInline="base" paddingBlockStart="base">
        <s-grid gridTemplateColumns="1fr auto" gap="small-200" alignItems="center">
          <s-text-field icon="search" placeholder="Filter customers"></s-text-field>
          <s-button commandFor="tagged-with">Tagged with</s-button>
          <s-popover id="tagged-with">
            <s-stack gap="small-200" padding="small-200">
              <s-text-field value="VIP" placeholder="Add tag"></s-text-field>
              <s-link href="">Clear</s-link>
            </s-stack>
          </s-popover>
        </s-grid>
        <s-button variant="secondary">Save</s-button>
      </s-grid>

      <s-stack direction="inline"  gap="small-400" paddingInline="base">
        <s-clickable-chip removable>Tagged with VIP</s-clickable-chip>
      </s-stack>
      
      <s-grid gridTemplateColumns="1fr auto" gap="base" alignItems="center" paddingInline="base">
        <s-checkbox label="Showing 2 customers"></s-checkbox>
        <s-select>
          <s-option value="newest">Newest update</s-option>
          <s-option value="oldest">Oldest update</s-option>
        </s-select>
      </s-grid>
      
      <s-stack>
        <s-clickable borderStyle="solid none none none" border="base" paddingInline="base" paddingBlock="small">
          <s-grid gridTemplateColumns="1fr auto" gap="base" alignItems="center">
            <s-stack direction="inline" gap="small" alignItems="center">
              <s-checkbox></s-checkbox>
              <s-avatar></s-avatar>
              <s-stack>
                <s-heading>Mae Jemison</s-heading>
                <s-text>Decatur, USA</s-text>
              </s-stack>
            </s-stack>
            <s-button icon="menu-horizontal" variant="tertiary" accessibilityLabel="Actions for Mae Jemison"></s-button>
          </s-grid>
        </s-clickable>
        <s-clickable borderStyle="solid none none none" border="base" paddingInline="base" paddingBlock="small">
          <s-grid gridTemplateColumns="1fr auto" gap="base" alignItems="center">
            <s-stack direction="inline" gap="small" alignItems="center">
              <s-checkbox></s-checkbox>
              <s-avatar></s-avatar>
              <s-stack>
                <s-heading>Ellen Ochoa</s-heading>
                <s-text>Los Angeles, USA</s-text>
              </s-stack>
            </s-stack>
            <s-button icon="menu-horizontal" variant="tertiary" accessibilityLabel="Actions for Ellen Ochoa"></s-button>
          </s-grid>
        </s-clickable>
      </s-stack>
    </s-stack>
  </s-section>
  ```

  ##### html

  ```html
  <s-section padding="none">
    <s-stack gap="small-200">
      <s-grid gridTemplateColumns="1fr auto" gap="base" alignItems="center" paddingInline="base" paddingBlockStart="base">
        <s-grid gridTemplateColumns="1fr auto" gap="small-200" alignItems="center">
          <s-text-field icon="search" placeholder="Filter customers"></s-text-field>
          <s-button commandFor="tagged-with">Tagged with</s-button>
          <s-popover id="tagged-with">
            <s-stack gap="small-200" padding="small-200">
              <s-text-field value="VIP" placeholder="Add tag"></s-text-field>
              <s-link href="">Clear</s-link>
            </s-stack>
          </s-popover>
        </s-grid>
        <s-button variant="secondary">Save</s-button>
      </s-grid>

      <s-stack direction="inline"  gap="small-400" paddingInline="base">
        <s-clickable-chip removable>Tagged with VIP</s-clickable-chip>
      </s-stack>
      
      <s-grid gridTemplateColumns="1fr auto" gap="base" alignItems="center" paddingInline="base">
        <s-checkbox label="Showing 2 customers"></s-checkbox>
        <s-select>
          <s-option value="newest">Newest update</s-option>
          <s-option value="oldest">Oldest update</s-option>
        </s-select>
      </s-grid>
      
      <s-stack>
        <s-clickable borderStyle="solid none none none" border="base" paddingInline="base" paddingBlock="small">
          <s-grid gridTemplateColumns="1fr auto" gap="base" alignItems="center">
            <s-stack direction="inline" gap="small" alignItems="center">
              <s-checkbox></s-checkbox>
              <s-avatar></s-avatar>
              <s-stack>
                <s-heading>Mae Jemison</s-heading>
                <s-text>Decatur, USA</s-text>
              </s-stack>
            </s-stack>
            <s-button icon="menu-horizontal" variant="tertiary" accessibilityLabel="Actions for Mae Jemison"></s-button>
          </s-grid>
        </s-clickable>
        <s-clickable borderStyle="solid none none none" border="base" paddingInline="base" paddingBlock="small">
          <s-grid gridTemplateColumns="1fr auto" gap="base" alignItems="center">
            <s-stack direction="inline" gap="small" alignItems="center">
              <s-checkbox></s-checkbox>
              <s-avatar></s-avatar>
              <s-stack>
                <s-heading>Ellen Ochoa</s-heading>
                <s-text>Los Angeles, USA</s-text>
              </s-stack>
            </s-stack>
            <s-button icon="menu-horizontal" variant="tertiary" accessibilityLabel="Actions for Ellen Ochoa"></s-button>
          </s-grid>
        </s-clickable>
      </s-stack>
    </s-stack>
  </s-section>
  ```

## Related

[Requirements - Built for Shopify](https://shopify.dev/docs/apps/launch/built-for-shopify/requirements)

</page>

<page>
---
title: Setup guide
description: >
  Setup guide provides an interactive checklist to guide merchants through
  essential onboarding or configuration tasks. Progress is tracked visually,
  helping merchants complete all required steps and understand what remains.
    | Used to | Examples |
    | --- | --- |
    | Onboard merchants | Initial app setup |
    | Track completion of multi-step processes | Necessary setup steps |
    ---
api_name: app-home
source_url:
  html: 'https://shopify.dev/docs/api/app-home/patterns/compositions/setup-guide'
  md: 'https://shopify.dev/docs/api/app-home/patterns/compositions/setup-guide.md'
---

# Setup guide

Setup guide provides an interactive checklist to guide merchants through essential onboarding or configuration tasks. Progress is tracked visually, helping merchants complete all required steps and understand what remains.

| Used to | Examples |
| - | - |
| Onboard merchants | Initial app setup |
| Track completion of multi-step processes | Necessary setup steps |

***

Examples

### Examples

- #### Setup guide

  ##### jsx

  ```jsx
  <s-section>
    <s-grid gap="base">
      <s-grid gap="small-200">
        <s-grid
          gridTemplateColumns="1fr auto auto"
          gap="small-300"
          alignItems="center"
        >
          <s-heading>Setup Guide</s-heading>
          <s-button
            accessibilityLabel="Dismiss Guide"
            variant="tertiary"
            tone="neutral"
            icon="x"
           />
          <s-button
            accessibilityLabel="Toggle setup guide"
            variant="tertiary"
            tone="neutral"
            icon="chevron-up"
           />
        </s-grid>
        <s-paragraph>
          Use this personalized guide to get your store ready for sales.
        </s-paragraph>
        <s-paragraph color="subdued">0 out of 3 steps completed</s-paragraph>
      </s-grid>
      <s-box borderRadius="base" border="base" background="base">
        <s-box>
          <s-grid gridTemplateColumns="1fr auto" gap="base" padding="small">
            <s-checkbox label="Upload an image for your puzzle" />
            <s-button
              accessibilityLabel="Toggle step 1 details"
              variant="tertiary"
              icon="chevron-up"
             />
          </s-grid>
          <s-box padding="small" paddingBlockStart="none">
            <s-box padding="base" background="subdued" borderRadius="base">
              <s-grid
                gridTemplateColumns="1fr auto"
                gap="base"
                alignItems="center"
              >
                <s-grid gap="small-200">
                  <s-paragraph>
                    Start by uploading a high-quality image that will be used to
                    create your puzzle. For best results, use images that are at
                    least 1200x1200 pixels.
                  </s-paragraph>
                  <s-stack direction="inline" gap="small-200">
                    <s-button variant="primary">Upload image</s-button>
                    <s-button variant="tertiary" tone="neutral">
                      {" "}
                      Image requirements{" "}
                    </s-button>
                  </s-stack>
                </s-grid>
                <s-box maxBlockSize="80px" maxInlineSize="80px">
                  <s-image
                    src="https://cdn.shopify.com/s/assets/admin/checkout/settings-customizecart-705f57c725ac05be5a34ec20c05b94298cb8afd10aac7bd9c7ad02030f48cfa0.svg"
                    alt="Customize checkout illustration"
                   />
                </s-box>
              </s-grid>
            </s-box>
          </s-box>
        </s-box>
        <s-divider />
        <s-box>
          <s-grid gridTemplateColumns="1fr auto" gap="base" padding="small">
            <s-checkbox label="Choose a puzzle template" />
            <s-button
              accessibilityLabel="Toggle step 2 details"
              variant="tertiary"
              icon="chevron-down"
             />
          </s-grid>
          <s-box
            padding="small"
            paddingBlockStart="none"
           />
        </s-box>
        <s-divider />
        <s-box>
          <s-grid gridTemplateColumns="1fr auto" gap="base" padding="small">
            <s-checkbox label="Customize puzzle piece shapes" />
            <s-button
              accessibilityLabel="Toggle step 3 details"
              variant="tertiary"
              icon="chevron-down"
             />
          </s-grid>
          <s-box
            padding="small"
            paddingBlockStart="none"
           />
        </s-box>
      </s-box>
    </s-grid>
  </s-section>
  ```

  ##### html

  ```html
  <s-section>
    <s-grid gap="base">
      <s-grid gap="small-200">
        <s-grid gridTemplateColumns="1fr auto auto" gap="small-300" alignItems="center">
          <s-heading>Setup Guide</s-heading>
          <s-button
            accessibilityLabel="Dismiss Guide"
            variant="tertiary"
            tone="neutral"
            icon="x"
          ></s-button>
          <s-button
            accessibilityLabel="Toggle setup guide"
            variant="tertiary"
            tone="neutral"
            icon="chevron-up"
          ></s-button>
        </s-grid>
        <s-paragraph>
          Use this personalized guide to get your store ready for sales.
        </s-paragraph>
          <s-paragraph tone="subdued">0 out of 3 steps completed</s-paragraph>
      </s-grid>
      <s-box borderRadius="base" border="base" background="base">
        <s-box>
            <s-grid gridTemplateColumns="1fr auto" gap="base" padding="small">
              <s-checkbox
                label="Upload an image for your puzzle"
              ></s-checkbox>
              <s-button
                accessibilityLabel="Toggle step 1 details"
                variant="tertiary"
                icon="chevron-up"
              ></s-button>
            </s-grid>
          <s-box padding="small" paddingBlockStart="none">
            <s-box padding="base" background="subdued" borderRadius="base">
              <s-grid gridTemplateColumns="1fr auto" gap="base" alignItems="center">
                <s-grid gap="small-200">
                  <s-paragraph>
                    Start by uploading a high-quality image that will be used to create your
                    puzzle. For best results, use images that are at least 1200x1200 pixels.
                  </s-paragraph>
                  <s-stack direction="inline" gap="small-200">
                    <s-button variant="primary">
                      Upload image
                    </s-button>
                    <s-button variant="tertiary" tone="neutral"> Image requirements </s-button>
                  </s-stack>
                </s-grid>
                <s-box maxBlockSize="80px" maxInlineSize="80px">
                  <s-image
                    src="https://cdn.shopify.com/s/assets/admin/checkout/settings-customizecart-705f57c725ac05be5a34ec20c05b94298cb8afd10aac7bd9c7ad02030f48cfa0.svg"
                    alt="Customize checkout illustration"
                  ></s-image>
                </s-box>
              </s-grid>
            </s-box>
          </s-box>
        </s-box>
        <s-divider></s-divider>
        <s-box>
            <s-grid gridTemplateColumns="1fr auto" gap="base" padding="small">
              <s-checkbox
                label="Choose a puzzle template"
              ></s-checkbox>
              <s-button
                accessibilityLabel="Toggle step 2 details"
                variant="tertiary"
                icon="chevron-down"
              ></s-button>
            </s-grid>
          <s-box padding="small" paddingBlockStart="none" style="display: none;"></s-box>
        </s-box>
        <s-divider></s-divider>
        <s-box>
            <s-grid gridTemplateColumns="1fr auto" gap="base" padding="small">
              <s-checkbox
                label="Customize puzzle piece shapes"
              ></s-checkbox>
              <s-button
                accessibilityLabel="Toggle step 3 details"
                variant="tertiary"
                icon="chevron-down"
              ></s-button>
            </s-grid>
          <s-box padding="small" paddingBlockStart="none" style="display: none;"></s-box>
        </s-box>
      </s-box>
    </s-grid>
  </s-section>
  ```

## Related

[Requirements - Built for Shopify](https://shopify.dev/docs/apps/launch/built-for-shopify/requirements)

</page>

<page>
---
title: Details
description: >-
  The details page allows merchants to view, create and edit objects. Use the
  right column to provide editable fields, and the right column for supporting
  information such as status, metadata, and summaries.

    | Used to | Examples |
    | --- | --- |
    | View, edit and create objects  | Discounts, shipping labels, newsletters, templates. |

    ![Preview of the details page pattern](https://cdn.shopify.com/shopifycloud/shopify-dev/development/assets/assets/images/templated-apis-screenshots/admin/patterns/details-example-Ds2ssAcy.png)

    This pattern uses `Badge`, `Box`, `Button`, `Grid`, `Heading`, `Image`, `Link`, `MoneyField`, `NumberField`, `SearchField`, `Section`, `Select`, `Stack`, `Switch`, `Table`, `TextArea`, `TextField`, `UnorderedList`, and `URLField` components.

    ---

    ## Design guidelines
    Design details pages that enable users to create, view, and edit resource objects.

    ### Navigation

    * Users must be able to return to the previous page without using the browser button. To achieve this, your app can provide breadcrumbs or a Back button on the page.
    * Use tabs sparingly for secondary navigation purposes when the nav menu isn't sufficient.
      * Clicking a tab should only change the content below it, not above.
      * Tabs should never wrap onto multiple lines.
      * Navigating between tabs shouldn't cause the tabs to change position or move.
      * Offer users clear and predictable action labels.

    ---

    ### Layout

    * Design your app to be responsive and adapt to different screen sizes and devices. This ensures a seamless user experience across various platforms.
    * Use looser spacing for low-density layouts. Use tighter spacing for high-density layouts.
    * Always use the default width. Full width tends to waste space and make the page harder to parse
    * In the primary column: Put information that defines the resource object
    * In the secondary column: Put supporting information such as status, metadata, and summaries
    * Arrange content in order of importance
    * Group similar content in the same card
    * Place unique page actions at the top of the actions list and typical object actions at the bottom

    ---

    ### Forms

    * For more than five inputs, use sections with titles in one card or use multiple cards with headers.
    * Form inputs should be saved using the App Bridge Contextual Save Bar API. This also applies to forms within max modals. Continuous data validation or auto-save for forms is consistent with the standard Shopify admin save UX.

    ---

    <style>
      div[class*="CodeBlock-module-CodeBlock_"] {
        max-height: calc(100vh - 80px) !important;
      }
      div[class*="Tabs-module-TabsContent_"] {
        overflow: auto !important;
      }
      div[class*="Screenshot-module-Screenshot_"] {
        display: none !important;
      }
    </style>

api_name: app-home
source_url:
  html: '<https://shopify.dev/docs/api/app-home/patterns/templates/details>'
  md: '<https://shopify.dev/docs/api/app-home/patterns/templates/details.md>'
---

# Details

The details page allows merchants to view, create and edit objects. Use the right column to provide editable fields, and the right column for supporting information such as status, metadata, and summaries.

| Used to | Examples |
| - | - |
| View, edit and create objects | Discounts, shipping labels, newsletters, templates. |

![Preview of the details page pattern](https://cdn.shopify.com/shopifycloud/shopify-dev/development/assets/assets/images/templated-apis-screenshots/admin/patterns/details-example-Ds2ssAcy.png)

This pattern uses `Badge`, `Box`, `Button`, `Grid`, `Heading`, `Image`, `Link`, `MoneyField`, `NumberField`, `SearchField`, `Section`, `Select`, `Stack`, `Switch`, `Table`, `TextArea`, `TextField`, `UnorderedList`, and `URLField` components.

***

## Design guidelines

Design details pages that enable users to create, view, and edit resource objects.

### Navigation

- Users must be able to return to the previous page without using the browser button. To achieve this, your app can provide breadcrumbs or a Back button on the page.

- Use tabs sparingly for secondary navigation purposes when the nav menu isn't sufficient.

  - Clicking a tab should only change the content below it, not above.
  - Tabs should never wrap onto multiple lines.
  - Navigating between tabs shouldn't cause the tabs to change position or move.
  - Offer users clear and predictable action labels.

***

### Layout

- Design your app to be responsive and adapt to different screen sizes and devices. This ensures a seamless user experience across various platforms.
- Use looser spacing for low-density layouts. Use tighter spacing for high-density layouts.
- Always use the default width. Full width tends to waste space and make the page harder to parse
- In the primary column: Put information that defines the resource object
- In the secondary column: Put supporting information such as status, metadata, and summaries
- Arrange content in order of importance
- Group similar content in the same card
- Place unique page actions at the top of the actions list and typical object actions at the bottom

***

### Forms

- For more than five inputs, use sections with titles in one card or use multiple cards with headers.
- Form inputs should be saved using the App Bridge Contextual Save Bar API. This also applies to forms within max modals. Continuous data validation or auto-save for forms is consistent with the standard Shopify admin save UX.

***

Examples

### Examples

- #### Details

  ##### jsx

  ```jsx
  <form
    data-save-bar
    onSubmit={(event) => {
      event.preventDefault();
      const formData = new FormData(event.target);
      const formEntries = Object.fromEntries(formData);
      console.log("Form data", formEntries);
    }}
    onReset={(event) => {
      console.log("Handle discarded changes if necessary");
    }}
  >
    <s-page heading="Mountain view">
      <s-link slot="breadcrumb-actions" href="/app/puzzles">
        Puzzles
      </s-link>
          <s-button slot="secondary-actions">Duplicate</s-button>
          <s-button slot="secondary-actions">Delete</s-button>
          {/* === */}
          {/* Puzzle information */}
          {/* === */}
          <s-section heading="Puzzle information">
            <s-grid gap="base">
              <s-text-field
                label="Puzzle name"
                name="name"
                labelAccessibilityVisibility="visible"
                placeholder="Enter puzzle name"
                value="Mountain view"
                details="Players will see this name when browsing puzzles."
               />
              <s-text-area
                label="Description"
                name="description"
                labelAccessibilityVisibility="visible"
                placeholder="Brief description of your puzzle"
                value="A beautiful mountain landscape puzzle"
                details="Help players understand what your puzzle features"
               />
              <s-money-field
                label="Price"
                name="price"
                labelAccessibilityVisibility="visible"
                placeholder="0.00"
                value="9.99"
                details="Set the price for this puzzle"
               />
              <s-url-field
                label="Reference image URL"
                name="reference-image-url"
                labelAccessibilityVisibility="visible"
                placeholder="https://example.com/image.jpg"
                details="Optional link to original image"
               />
            </s-grid>
          </s-section>

          {/* === */}
          {/* Puzzle templates */}
          {/* === */}
          <s-section heading="Puzzle templates">
            <s-grid gap="base">
              <s-grid
                gridTemplateColumns="1fr auto"
                gap="base"
                alignItems="center"
              >
                <s-grid-item>
                  <s-search-field
                    label="Search templates"
                    labelAccessibilityVisibility="exclusive"
                    placeholder="Search templates"
                   />
                </s-grid-item>
                <s-grid-item>
                  <s-button>Browse</s-button>
                </s-grid-item>
              </s-grid>
              <s-box
                background="strong"
                border="base"
                borderRadius="base"
                borderStyle="solid"
                overflow="hidden"
              >
                <s-table>
                  <s-table-header-row>
                    <s-table-header listSlot="primary">Template</s-table-header>
                    <s-table-header>
                      <s-stack alignItems="end">Actions</s-stack>
                    </s-table-header>
                    <s-table-header listSlot="secondary">
                      <s-stack direction="inline" alignItems="end" />
                    </s-table-header>
                  </s-table-header-row>
                  <s-table-body>
                    <s-table-row>
                      <s-table-cell>
                        <s-stack
                          direction="inline"
                          gap="base"
                          alignItems="center"
                        >
                          <s-box
                            border="base"
                            borderRadius="base"
                            overflow="hidden"
                            maxInlineSize="40px"
                            maxBlockSize="40px"
                          >
                            <s-image
                              alt="16-pieces puzzle template"
                              src="https://cdn.shopify.com/static/images/polaris/patterns/16-pieces.png"
                             />
                          </s-box>
                          16-pieces puzzle
                        </s-stack>
                      </s-table-cell>
                      <s-table-cell>
                        <s-stack alignItems="end">
                          <s-link>Preview</s-link>
                        </s-stack>
                      </s-table-cell>
                      <s-table-cell>
                        <s-stack alignItems="end">
                          <s-button
                            icon="x"
                            tone="neutral"
                            variant="tertiary"
                            accessibilityLabel="Remove 16-Pieces Puzzle template"
                           />
                        </s-stack>
                      </s-table-cell>
                    </s-table-row>
                    <s-table-row>
                      <s-table-cell>
                        <s-stack
                          direction="inline"
                          gap="base"
                          alignItems="center"
                        >
                          <s-box
                            border="base"
                            borderRadius="base"
                            overflow="hidden"
                            maxInlineSize="40px"
                            maxBlockSize="40px"
                          >
                            <s-image
                              alt="9-pieces puzzle template"
                              src="https://cdn.shopify.com/static/images/polaris/patterns/9-pieces.png"
                             />
                          </s-box>
                          9-pieces puzzle
                        </s-stack>
                      </s-table-cell>
                      <s-table-cell>
                        <s-stack
                          direction="inline"
                          gap="base"
                          justifyContent="end"
                        >
                          <s-link>Preview</s-link>
                        </s-stack>
                      </s-table-cell>
                      <s-table-cell>
                        <s-stack alignItems="end">
                          <s-button
                            icon="x"
                            tone="neutral"
                            variant="tertiary"
                            accessibilityLabel="Remove 9-Pieces Puzzle template"
                           />
                        </s-stack>
                      </s-table-cell>
                    </s-table-row>
                    {/* Add more rows as needed here */}
                    {/* If more than 10 rows are needed, details page tables should use the paginate, hasPreviousPage, hasNextPage, onPreviousPage, and onNextPage attributes to display and handle pagination) */}
                  </s-table-body>
                </s-table>
              </s-box>
            </s-grid>
          </s-section>

          {/* === */}
          {/* Settings */}
          {/* === */}
          <s-section heading="Settings">
            <s-grid gap="base">
              <s-select label="Puzzle size" name="puzzle-size">
                <s-option value="small">Small (9" x 9")</s-option>
                <s-option value="medium" selected>
                  Medium (18" x 24")
                </s-option>
                <s-option value="large">Large (24" x 36")</s-option>
              </s-select>
              <s-select label="Piece count" name="piece-count">
                <s-option value="250">250 pieces (Easy)</s-option>
                <s-option value="500" selected>
                  500 pieces (Medium)
                </s-option>
                <s-option value="1000">1000 pieces (Hard)</s-option>
                <s-option value="2000">2000 pieces (Expert)</s-option>
              </s-select>
              <s-select label="Material" name="material">
                <s-option value="standard" selected>
                  Standard cardboard
                </s-option>
                <s-option value="premium">Premium cardboard</s-option>
                <s-option value="wooden">Wooden pieces</s-option>
              </s-select>
              <s-number-field
                label="Quantity in stock"
                name="quantity-in-stock"
                labelAccessibilityVisibility="visible"
                value="50"
                min={0}
                placeholder="0"
                details="Current inventory quantity"
               />
              <s-switch
                label="Include reference image"
                name="include-reference-image"
                details="Ship a reference image with the puzzle"
               />
            </s-grid>
          </s-section>
          {/* Use the aside slot for sidebar content */}
          <s-box slot="aside">
            {/* === */}
            {/* Puzzle summary */}
            {/* === */}
            <s-section heading="Puzzle summary">
              <s-heading>Mountain view</s-heading>
              <s-unordered-list>
                <s-list-item>16-piece puzzle with medium difficulty</s-list-item>
                <s-list-item>Pieces can be rotated</s-list-item>
                <s-list-item>No time limit</s-list-item>
                <s-list-item>
                  <s-stack direction="inline" gap="small">
                    <s-text>Current status:</s-text>
                    <s-badge color="base" tone="success">
                      Active
                    </s-badge>
                  </s-stack>
                </s-list-item>
              </s-unordered-list>
            </s-section>
      </s-box>
    </s-page>
  </form>
  ```

  ##### html

  ```html
  <!DOCTYPE html>
  <html lang="en">
    <head>
      <meta charset="UTF-8" />
      <meta name="viewport" content="width=device-width, initial-scale=1.0" />
      <script src="https://cdn.shopify.com/shopifycloud/polaris.js"></script>
      <title>Pattern</title>
    </head>
    <body>
      <!-- === -->
      <!-- Details page pattern -->
      <!-- === -->
      <form data-save-bar>
        <s-page heading="Mountain view">
          <s-link slot="breadcrumb-actions" href="/app/puzzles">Puzzles</s-link>
          <s-button slot="secondary-actions">Duplicate</s-button>
          <s-button slot="secondary-actions">Delete</s-button>
          <!-- === -->
          <!-- Puzzle information -->
          <!-- === -->
          <s-section heading="Puzzle information">
            <s-grid gap="base">
              <s-text-field
                label="Puzzle name"
                name="name"
                labelAccessibilityVisibility="visible"
                placeholder="Enter puzzle name"
                value="Mountain view"
                details="Players will see this name when browsing puzzles."
              ></s-text-field>
              <s-text-area
                label="Description"
                name="description"
                labelAccessibilityVisibility="visible"
                placeholder="Brief description of your puzzle"
                value="A beautiful mountain landscape puzzle"
                details="Help players understand what your puzzle features"
              ></s-text-area>
              <s-money-field
                label="Price"
                name="price"
                labelAccessibilityVisibility="visible"
                placeholder="0.00"
                value="9.99"
                details="Set the price for this puzzle"
              ></s-money-field>
              <s-url-field
                label="Reference image URL"
                name="reference-image-url"
                labelAccessibilityVisibility="visible"
                placeholder="https://example.com/image.jpg"
                details="Optional link to original image"
              ></s-url-field>
            </s-grid>
          </s-section>
          <!-- === -->
          <!-- Puzzle templates -->
          <!-- === -->
          <s-section heading="Puzzle templates">
            <s-grid gap="base">
              <s-grid gridTemplateColumns="1fr auto" gap="base" alignItems="center">
                <s-grid-item>
                  <s-search-field
                    label="Search templates"
                    labelAccessibilityVisibility="exclusive"
                    placeholder="Search templates"
                  ></s-search-field>
                </s-grid-item>
                <s-grid-item>
                  <s-button>Browse</s-button>
                </s-grid-item>
              </s-grid>
              <s-box
                background="strong"
                border="base"
                borderRadius="base"
                borderStyle="solid"
                overflow="hidden"
              >
                <s-table border="base" borderRadius="base" borderStyle="solid">
                  <s-table-header-row>
                    <s-table-header listSlot="primary">Template</s-table-header>
                    <s-table-header>
                      <s-stack alignItems="end">Actions</s-stack>
                    </s-table-header>
                    <s-table-header listSlot="secondary">
                      <s-stack direction="inline" alignItems="end"></s-stack>
                    </s-table-header>
                  </s-table-header-row>
                  <s-table-body>
                    <s-table-row>
                      <s-table-cell listSlot="primary">
                        <s-stack direction="inline" gap="base" alignItems="center">
                          <s-box
                            border="base"
                            borderRadius="base"
                            overflow="hidden"
                            maxInlineSize="40px"
                            maxBlockSize="40px"
                          >
                            <s-image
                              alt="16-pieces puzzle template"
                              src="https://cdn.shopify.com/static/images/polaris/patterns/16-pieces.png"
                            ></s-image>
                          </s-box>
                          16-pieces puzzle
                        </s-stack>
                      </s-table-cell>
                      <s-table-cell>
                        <s-stack alignItems="end">
                          <s-link>Preview</s-link>
                        </s-stack>
                      </s-table-cell>
                      <s-table-cell>
                        <s-stack alignItems="end">
                          <s-button
                            icon="x"
                            tone="neutral"
                            variant="tertiary"
                            accessibilityLabel="Remove 16-Pieces Puzzle template"
                          ></s-button>
                        </s-stack>
                      </s-table-cell>
                    </s-table-row>
                    <s-table-row>
                      <s-table-cell listSlot="primary">
                        <s-stack direction="inline" gap="base" alignItems="center">
                          <s-box
                            border="base"
                            borderRadius="base"
                            overflow="hidden"
                            maxInlineSize="40px"
                            maxBlockSize="40px"
                          >
                            <s-image
                              alt="9-pieces puzzle template"
                              src="https://cdn.shopify.com/static/images/polaris/patterns/9-pieces.png"
                            ></s-image>
                          </s-box>
                          9-pieces puzzle
                        </s-stack>
                      </s-table-cell>
                      <s-table-cell>
                        <s-stack direction="inline" gap="base" justifyContent="end">
                          <s-link>Preview</s-link>
                        </s-stack>
                      </s-table-cell>
                      <s-table-cell listSlot="secondary">
                        <s-stack alignItems="end">
                          <s-button
                            icon="x"
                            tone="neutral"
                            variant="tertiary"
                            accessibilityLabel="Remove 9-Pieces Puzzle template"
                          ></s-button>
                        </s-stack>
                      </s-table-cell>
                    </s-table-row>
                    <!-- Add more rows as needed here -->
                    <!-- If more than 10 rows are needed, details page tables should use the paginate, hasPreviousPage, hasNextPage, onPreviousPage, and onNextPage attributes to display and handle pagination) -->
                  </s-table-body>
                </s-table>
              </s-box>
            </s-grid>
          </s-section>
          <!-- === -->
          <!-- Settings -->
          <!-- === -->
          <s-section heading="Settings">
            <s-grid gap="base">
              <s-select label="Puzzle size" name="puzzle-size">
                <s-option value="small">Small (9" x 9")</s-option>
                <s-option value="medium" selected> Medium (18" x 24") </s-option>
                <s-option value="large">Large (24" x 36")</s-option>
              </s-select>
              <s-select label="Piece count" name="piece-count">
                <s-option value="250">250 pieces (Easy)</s-option>
                <s-option value="500" selected> 500 pieces (Medium) </s-option>
                <s-option value="1000">1000 pieces (Hard)</s-option>
                <s-option value="2000">2000 pieces (Expert)</s-option>
              </s-select>
              <s-select label="Material" name="material">
                <s-option value="standard" selected> Standard cardboard </s-option>
                <s-option value="premium">Premium cardboard</s-option>
                <s-option value="wooden">Wooden pieces</s-option>
              </s-select>
              <s-number-field
                label="Quantity in stock"
                name="quantity-in-stock"
                labelAccessibilityVisibility="visible"
                value="50"
                min="0"
                placeholder="0"
                details="Current inventory quantity"
              ></s-number-field>
              <s-switch
                label="Include reference image"
                name="include-reference-image"
                details="Ship a reference image with the puzzle"
              ></s-switch>
            </s-grid>
          </s-section>
          <!-- Use the aside slot for sidebar content -->
          <s-box slot="aside">
            <!-- === -->
            <!-- Puzzle summary -->
            <!-- === -->
            <s-section heading="Puzzle summary">
              <s-heading>Mountain view</s-heading>
              <s-unordered-list>
                <s-list-item>16-piece puzzle with medium difficulty</s-list-item>
                <s-list-item>Pieces can be rotated</s-list-item>
                <s-list-item>No time limit</s-list-item>
                <s-list-item>
                  <s-stack direction="inline" gap="small">
                    <s-text>Current status:</s-text>
                    <s-badge color="base" tone="success">
                      Active
                    </s-badge>
                  </s-stack>
                </s-list-item>
              </s-unordered-list>
            </s-section>
          </s-box>
        </s-page>
      </form>
    </body>
  </html>
  ```

## Related

[Requirements - Built for Shopify](https://shopify.dev/docs/apps/launch/built-for-shopify/requirements)

</page>

<page>
---
title: Homepage
description: >-
  The app URL specified in the Partner Dashboard should point to your app
  homepage. The home page of your app is the first thing merchants will see, and
  it should provide daily value to them. Design the page to provide status
  updates and show merchants what actions they can take.

    | Used to | Examples |
    | --- | --- |
    | Teach merchants how to use the app | Onboarding, how-to guides |
    | Display app functionalities | Call-to-actions to app features, resource tables |
    | Show updates | Status banners, company news |

    ![Preview of the homepage pattern](https://cdn.shopify.com/shopifycloud/shopify-dev/development/assets/assets/images/templated-apis-screenshots/admin/patterns/homepage-example-BfBsJ6G0.png)

    This pattern uses `Badge`, `Banner`, `Box`, `Button`, `Checkbox`, `Clickable`, `Divider`, `Grid`, `Heading`, `Image`, `Link`, `Paragraph`, `Section`, `Stack`, and `Text` components.

    ---

    ## Design guidelines
    Your app home page should be designed to provide users with relevant, timely information like quick statistics, status updates, or information that’s immediately actionable.

    ### Onboarding

    The onboarding experience quickly introduces users to your app's essential features. A good onboarding should be self-guided, easy to follow and make users feel they understand how the app works after finishing it. If the onboarding is long or complex, give users the option to complete it at a later time to avoid stopping their workflow.

    * Onboarding must be brief and direct. Provide clear instructions and guide users to completion
    * Only request information from users if it's necessary
    * If your onboarding isn't essential, then make it dismissible
    * Don't have more than five steps in your onboarding process. This can lead users to drop off and not use your app

    ---

    ### Visual design

    * Design your app to be responsive and adapt to different screen sizes and devices. This ensures a seamless user experience across various platforms.
    * Use looser spacing for low-density layouts. Use tighter spacing for high-density layouts.
    * Use high-resolution photos and images to ensure a professional, high-quality experience.

    ---

    <style>
      div[class*="CodeBlock-module-CodeBlock_"] {
        max-height: calc(100vh - 80px) !important;
      }
      div[class*="Tabs-module-TabsContent_"] {
        overflow: auto !important;
      }
      div[class*="Screenshot-module-Screenshot_"] {
        display: none !important;
      }
    </style>

api_name: app-home
source_url:
  html: '<https://shopify.dev/docs/api/app-home/patterns/templates/homepage>'
  md: '<https://shopify.dev/docs/api/app-home/patterns/templates/homepage.md>'
---

# Homepage

The app URL specified in the Partner Dashboard should point to your app homepage. The home page of your app is the first thing merchants will see, and it should provide daily value to them. Design the page to provide status updates and show merchants what actions they can take.

| Used to | Examples |
| - | - |
| Teach merchants how to use the app | Onboarding, how-to guides |
| Display app functionalities | Call-to-actions to app features, resource tables |
| Show updates | Status banners, company news |

![Preview of the homepage pattern](https://cdn.shopify.com/shopifycloud/shopify-dev/development/assets/assets/images/templated-apis-screenshots/admin/patterns/homepage-example-BfBsJ6G0.png)

This pattern uses `Badge`, `Banner`, `Box`, `Button`, `Checkbox`, `Clickable`, `Divider`, `Grid`, `Heading`, `Image`, `Link`, `Paragraph`, `Section`, `Stack`, and `Text` components.

***

## Design guidelines

Your app home page should be designed to provide users with relevant, timely information like quick statistics, status updates, or information that’s immediately actionable.

### Onboarding

The onboarding experience quickly introduces users to your app's essential features. A good onboarding should be self-guided, easy to follow and make users feel they understand how the app works after finishing it. If the onboarding is long or complex, give users the option to complete it at a later time to avoid stopping their workflow.

- Onboarding must be brief and direct. Provide clear instructions and guide users to completion
- Only request information from users if it's necessary
- If your onboarding isn't essential, then make it dismissible
- Don't have more than five steps in your onboarding process. This can lead users to drop off and not use your app

***

### Visual design

- Design your app to be responsive and adapt to different screen sizes and devices. This ensures a seamless user experience across various platforms.
- Use looser spacing for low-density layouts. Use tighter spacing for high-density layouts.
- Use high-resolution photos and images to ensure a professional, high-quality experience.

***

Examples

### Examples

- #### Homepage

  ##### jsx

  ```jsx
  const [visible, setVisible] = useState({
    banner: true,
    setupGuide: true,
    calloutCard: true,
    featuredApps: true,
  });
  const [expanded, setExpanded] = useState({
    setupGuide: true,
    step1: false,
    step2: false,
    step3: false,
  });
  const [progress, setProgress] = useState(0);

  return (
    <s-page>
        <s-button slot="primary-action">Create puzzle</s-button>
        <s-button slot="secondary-actions">Browse templates</s-button>
        <s-button slot="secondary-actions">Import image</s-button>

        {/* === */}
        {/* Banner */}
        {/* Use banners sparingly. Only one banner should be visible at a time. */}
        {/* If dismissed, use local storage or a database entry to avoid showing this section again to the same user. */}
        {/* === */}
        {visible.banner && (
          <s-banner
            dismissible
            onDismiss={() => setVisible({ ...visible, banner: false })}
          >
            3 of 5 puzzles created.{" "}
            <s-link href="#">Upgrade to Puzzlify Pro</s-link> to create unlimited
            puzzles.
          </s-banner>
        )}

        {/* === */}
        {/* Setup Guide */}
        {/* Keep instructions brief and direct. Only ask merchants for required information. */}
        {/* If dismissed, use local storage or a database entry to avoid showing this section again to the same user. */}
        {/* === */}
        {visible.setupGuide && (
          <s-section>
            <s-grid gap="small">
              {/* Header */}
              <s-grid gap="small-200">
                <s-grid
                  gridTemplateColumns="1fr auto auto"
                  gap="small-300"
                  alignItems="center"
                >
                  <s-heading>Setup Guide</s-heading>
                  <s-button
                    accessibilityLabel="Dismiss Guide"
                    onClick={() => setVisible({ ...visible, setupGuide: false })}
                    variant="tertiary"
                    tone="neutral"
                    icon="x"
                  ></s-button>
                  <s-button
                    accessibilityLabel="Toggle setup guide"
                    onClick={(e) =>
                      setExpanded({
                        ...expanded,
                        setupGuide: !expanded.setupGuide,
                      })
                    }
                    variant="tertiary"
                    tone="neutral"
                    icon={expanded.setupGuide ? "chevron-up" : "chevron-down"}
                  ></s-button>
                </s-grid>
                <s-paragraph>
                  Use this personalized guide to get your store ready for sales.
                </s-paragraph>
                <s-paragraph color="subdued">
                  {progress} out of 3 steps completed
                </s-paragraph>
              </s-grid>
              {/* Steps Container */}
              <s-box
                borderRadius="base"
                border="base"
                background="base"
                display={expanded.setupGuide ? "auto" : "none"}
              >
                {/* Step 1 */}
                <s-box>
                  <s-grid
                    gridTemplateColumns="1fr auto"
                    gap="base"
                    padding="small"
                  >
                    <s-checkbox
                      label="Upload an image for your puzzle"
                      onInput={(e) =>
                        setProgress(e.currentTarget.checked ? progress + 1 : progress - 1)
                      }
                    ></s-checkbox>
                    <s-button
                      onClick={(e) => {
                        setExpanded({ ...expanded, step1: !expanded.step1 });
                      }}
                      accessibilityLabel="Toggle step 1 details"
                      variant="tertiary"
                      icon={expanded.step1 ? "chevron-up" : "chevron-down"}
                    ></s-button>
                  </s-grid>
                  <s-box
                    padding="small"
                    paddingBlockStart="none"
                    display={expanded.step1 ? "auto" : "none"}
                  >
                    <s-box
                      padding="base"
                      background="subdued"
                      borderRadius="base"
                    >
                      <s-grid
                        gridTemplateColumns="1fr auto"
                        gap="base"
                        alignItems="center"
                      >
                        <s-grid gap="small-200">
                          <s-paragraph>
                            Start by uploading a high-quality image that will be
                            used to create your puzzle. For best results, use
                            images that are at least 1200x1200 pixels.
                          </s-paragraph>
                          <s-stack direction="inline" gap="small-200">
                            <s-button variant="primary">Upload image</s-button>
                            <s-button variant="tertiary" tone="neutral">
                              Image requirements
                            </s-button>
                          </s-stack>
                        </s-grid>
                        <s-box maxBlockSize="80px" maxInlineSize="80px">
                          <s-image
                            src="https://cdn.shopify.com/s/assets/admin/checkout/settings-customizecart-705f57c725ac05be5a34ec20c05b94298cb8afd10aac7bd9c7ad02030f48cfa0.svg"
                            alt="Customize checkout illustration"
                           />
                        </s-box>
                      </s-grid>
                    </s-box>
                  </s-box>
                </s-box>
                {/* Step 2 */}
                <s-divider />
                <s-box>
                  <s-grid
                    gridTemplateColumns="1fr auto"
                    gap="base"
                    padding="small"
                  >
                    <s-checkbox
                      label="Choose a puzzle template"
                      onInput={(e) =>
                        setProgress(e.currentTarget.checked ? progress + 1 : progress - 1)
                      }
                    ></s-checkbox>
                    <s-button
                      onClick={(e) =>
                        setExpanded({ ...expanded, step2: !expanded.step2 })
                      }
                      accessibilityLabel="Toggle step 2 details"
                      variant="tertiary"
                      icon={expanded.step2 ? "chevron-up" : "chevron-down"}
                    ></s-button>
                  </s-grid>
                  <s-box
                    padding="small"
                    paddingBlockStart="none"
                    display={expanded.step2 ? "auto" : "none"}
                  >
                    <s-box
                      padding="base"
                      background="subdued"
                      borderRadius="base"
                    >
                      <s-grid
                        gridTemplateColumns="1fr auto"
                        gap="base"
                        alignItems="center"
                      >
                        <s-grid gap="small-200">
                          <s-paragraph>
                            Select a template for your puzzle - choose between
                            9-piece (beginner), 16-piece (intermediate), or
                            25-piece (advanced) layouts.
                          </s-paragraph>
                          <s-stack direction="inline" gap="small-200">
                            <s-button variant="primary">Choose template</s-button>
                            <s-button variant="tertiary" tone="neutral">
                              See all templates
                            </s-button>
                          </s-stack>
                        </s-grid>
                        <s-box maxBlockSize="80px" maxInlineSize="80px">
                          <s-image
                            src="https://cdn.shopify.com/s/assets/admin/checkout/settings-customizecart-705f57c725ac05be5a34ec20c05b94298cb8afd10aac7bd9c7ad02030f48cfa0.svg"
                            alt="Customize checkout illustration"
                           />
                        </s-box>
                      </s-grid>
                    </s-box>
                  </s-box>
                </s-box>
                {/* Step 3 */}
                <s-divider />
                <s-box>
                  <s-grid
                    gridTemplateColumns="1fr auto"
                    gap="base"
                    padding="small"
                  >
                    <s-checkbox
                      label="Customize puzzle piece shapes"
                      onInput={(e) =>
                        setProgress(e.currentTarget.checked ? progress + 1 : progress - 1)
                      }
                    ></s-checkbox>
                    <s-button
                      onClick={(e) =>
                        setExpanded({ ...expanded, step3: !expanded.step3 })
                      }
                      accessibilityLabel="Toggle step 3 details"
                      variant="tertiary"
                      icon={expanded.step3 ? "chevron-up" : "chevron-down"}
                    ></s-button>
                  </s-grid>
                  <s-box
                    padding="small"
                    paddingBlockStart="none"
                    display={expanded.step3 ? "auto" : "none"}
                  >
                    <s-box
                      padding="base"
                      background="subdued"
                      borderRadius="base"
                    >
                      <s-grid
                        gridTemplateColumns="1fr auto"
                        gap="base"
                        alignItems="center"
                      >
                        <s-grid gap="small-200">
                          <s-paragraph>
                            Make your puzzle unique by customizing the shapes of
                            individual pieces. Choose from classic, curved, or
                            themed piece styles.
                          </s-paragraph>
                          <s-stack direction="inline" gap="small-200">
                            <s-button variant="primary">
                              Customize pieces
                            </s-button>
                            <s-button variant="tertiary" tone="neutral">
                              Learn about piece styles
                            </s-button>
                          </s-stack>
                        </s-grid>
                        <s-box maxBlockSize="80px" maxInlineSize="80px">
                          <s-image
                            src="https://cdn.shopify.com/s/assets/admin/checkout/settings-customizecart-705f57c725ac05be5a34ec20c05b94298cb8afd10aac7bd9c7ad02030f48cfa0.svg"
                            alt="Customize checkout illustration"
                           />
                        </s-box>
                      </s-grid>
                    </s-box>
                  </s-box>
                </s-box>
                {/* Add additional steps here... */}
              </s-box>
            </s-grid>
          </s-section>
        )}

        {/* === */}
        {/* Metrics cards */}
        {/* Your app homepage should provide merchants with quick statistics or status updates that help them understand how the app is performing for them. */}
        {/* === */}
        <s-section padding="base">
          <s-grid
            gridTemplateColumns="@container (inline-size <= 400px) 1fr, 1fr auto 1fr auto 1fr"
            gap="small"
          >
            <s-clickable
              href="#"
              paddingBlock="small-400"
              paddingInline="small-100"
              borderRadius="base"
            >
              <s-grid gap="small-300">
                <s-heading>Total Designs</s-heading>
                <s-stack direction="inline" gap="small-200">
                  <s-text>156</s-text>
                  <s-badge tone="success" icon="arrow-up">
                    12%
                  </s-badge>
                </s-stack>
              </s-grid>
            </s-clickable>
            <s-divider direction="block" />
            <s-clickable
              href="#"
              paddingBlock="small-400"
              paddingInline="small-100"
              borderRadius="base"
            >
              <s-grid gap="small-300">
                <s-heading>Units Sold</s-heading>
                <s-stack direction="inline" gap="small-200">
                  <s-text>2,847</s-text>
                  <s-badge tone="warning">0%</s-badge>
                </s-stack>
              </s-grid>
            </s-clickable>
            <s-divider direction="block" />
            <s-clickable
              href="#"
              paddingBlock="small-400"
              paddingInline="small-100"
              borderRadius="base"
            >
              <s-grid gap="small-300">
                <s-heading>Return Rate</s-heading>
                <s-stack direction="inline" gap="small-200">
                  <s-text>3.2%</s-text>
                  <s-badge tone="critical" icon="arrow-down">
                    0.8%
                  </s-badge>
                </s-stack>
              </s-grid>
            </s-clickable>
          </s-grid>
        </s-section>

        {/* === */}
        {/* Callout Card */}
        {/* If dismissed, use local storage or a database entry to avoid showing this section again to the same user. */}
        {/* === */}
        {visible.calloutCard && (
          <s-section>
            <s-grid
              gridTemplateColumns="1fr auto"
              gap="small-400"
              alignItems="start"
            >
              <s-grid
                gridTemplateColumns="@container (inline-size <= 480px) 1fr, auto auto"
                gap="base"
                alignItems="center"
              >
                <s-grid gap="small-200">
                  <s-heading>Ready to create your custom puzzle?</s-heading>
                  <s-paragraph>
                    Start by uploading an image to your gallery or choose from one
                    of our templates.
                  </s-paragraph>
                  <s-stack direction="inline" gap="small-200">
                    <s-button> Upload image </s-button>
                    <s-button tone="neutral" variant="tertiary">
                      {" "}
                      Browse templates{" "}
                    </s-button>
                  </s-stack>
                </s-grid>
                <s-stack alignItems="center">
                  <s-box
                    maxInlineSize="200px"
                    borderRadius="base"
                    overflow="hidden"
                  >
                    <s-image
                      src="https://cdn.shopify.com/static/images/polaris/patterns/callout.png"
                      alt="Customize checkout illustration"
                      aspectRatio="1/0.5"
                     />
                  </s-box>
                </s-stack>
              </s-grid>
              <s-button
                onClick={() => setVisible({ ...visible, calloutCard: false })}
                icon="x"
                tone="neutral"
                variant="tertiary"
                accessibilityLabel="Dismiss card"
              ></s-button>
            </s-grid>
          </s-section>
        )}

        {/* === */}
        {/* Puzzle templates */}
        {/* === */}
        <s-section>
          <s-heading>Puzzle Templates</s-heading>
          <s-grid
            gridTemplateColumns="repeat(auto-fit, minmax(155px, 1fr))"
            gap="base"
          >
            {/* Featured template 1 */}
            <s-box border="base" borderRadius="base" overflow="hidden">
              <s-clickable
                href="/puzzles/4-piece"
                accessibilityLabel="4-pieces puzzle template"
              >
                <s-image
                  aspectRatio="1/1"
                  objectFit="cover"
                  alt="4-pieces puzzle template"
                  src="https://cdn.shopify.com/static/images/polaris/patterns/4-pieces.png"
                 />
              </s-clickable>
              <s-divider />
              <s-grid
                gridTemplateColumns="1fr auto"
                background="base"
                padding="small"
                gap="small"
                alignItems="center"
              >
                <s-heading>4-Pieces</s-heading>
                <s-button
                  href="/puzzles/4-piece"
                  accessibilityLabel="View 4-pieces puzzle template"
                >
                  View
                </s-button>
              </s-grid>
            </s-box>
            {/* Featured template 2 */}
            <s-box
              border="base"
              borderRadius="base"
              background="transparent"
              overflow="hidden"
            >
              <s-clickable
                href="/puzzles/9-piece"
                accessibilityLabel="9-pieces puzzle template"
              >
                <s-image
                  aspectRatio="1/1"
                  objectFit="cover"
                  alt="9-pieces puzzle template"
                  src="https://cdn.shopify.com/static/images/polaris/patterns/9-pieces.png"
                 />
              </s-clickable>
              <s-divider />
              <s-grid
                gridTemplateColumns="1fr auto"
                background="base"
                padding="small"
                gap="small"
                alignItems="center"
              >
                <s-heading>9-Pieces</s-heading>
                <s-button
                  href="/puzzles/9-piece"
                  accessibilityLabel="View 9-pieces puzzle template"
                >
                  View
                </s-button>
              </s-grid>
            </s-box>
            {/* Featured template 3 */}
            <s-box
              border="base"
              borderRadius="base"
              background="transparent"
              overflow="hidden"
            >
              <s-clickable
                href="/puzzles/16-piece"
                accessibilityLabel="16-pieces puzzle template"
              >
                <s-image
                  aspectRatio="1/1"
                  objectFit="cover"
                  alt="16-pieces puzzle template"
                  src="https://cdn.shopify.com/static/images/polaris/patterns/16-pieces.png"
                 />
              </s-clickable>
              <s-divider />
              <s-grid
                gridTemplateColumns="1fr auto"
                background="base"
                padding="small"
                gap="small"
                alignItems="center"
              >
                <s-heading>16-Pieces</s-heading>
                <s-button
                  href="/puzzles/16-piece"
                  accessibilityLabel="View 16-pieces puzzle template"
                >
                  View
                </s-button>
              </s-grid>
            </s-box>
          </s-grid>
          <s-stack
            direction="inline"
            alignItems="center"
            justifyContent="center"
            paddingBlockStart="base"
          >
            <s-link href="/puzzles">See all puzzle templates</s-link>
          </s-stack>
        </s-section>

        {/* === */}
        {/* News */}
        {/* === */}
        <s-section>
          <s-heading>News</s-heading>
          <s-grid
            gridTemplateColumns="repeat(auto-fit, minmax(240px, 1fr))"
            gap="base"
          >
            {/* News item 1 */}
            <s-grid
              background="base"
              border="base"
              borderRadius="base"
              padding="base"
              gap="small-400"
            >
              <s-text>Jan 21, 2025</s-text>
              <s-link href="/news/new-shapes-and-themes">
                <s-heading>New puzzle shapes and themes added</s-heading>
              </s-link>
              <s-paragraph>
                We've added 5 new puzzle piece shapes and 3 seasonal themes to
                help you create more engaging and unique puzzles for your
                customers.
              </s-paragraph>
            </s-grid>
            {/* News item 2 */}
            <s-grid
              background="base"
              border="base"
              borderRadius="base"
              padding="base"
              gap="small-400"
            >
              <s-text>Nov 6, 2024</s-text>
              <s-link href="/news/puzzle-difficulty-customization">
                <s-heading>Puzzle difficulty customization features</s-heading>
              </s-link>
              <s-paragraph>
                Now you can fine-tune the difficulty of your puzzles with new
                rotation controls, edge highlighting options, and piece
                recognition settings.
              </s-paragraph>
            </s-grid>
          </s-grid>
          <s-stack
            direction="inline"
            alignItems="center"
            justifyContent="center"
            paddingBlockStart="base"
          >
            <s-link href="/news">See all news items</s-link>
          </s-stack>
        </s-section>

        {/* === */}
        {/* Featured apps */}
        {/* If dismissed, use local storage or a database entry to avoid showing this section again to the same user. */}
        {/* === */}
        {visible.featuredApps && (
          <s-section>
            <s-grid
              gridTemplateColumns="1fr auto"
              alignItems="center"
              paddingBlockEnd="small-400"
            >
              <s-heading>Featured apps</s-heading>
              <s-button
                onClick={() => setVisible({ ...visible, featuredApps: false })}
                icon="x"
                tone="neutral"
                variant="tertiary"
                accessibilityLabel="Dismiss featured apps section"
              ></s-button>
            </s-grid>
            <s-grid
              gridTemplateColumns="repeat(auto-fit, minmax(240px, 1fr))"
              gap="base"
            >
              {/* Featured app 1 */}
              <s-clickable
                href="https://apps.shopify.com/flow"
                border="base"
                borderRadius="base"
                padding="base"
                inlineSize="100%"
                accessibilityLabel="Download Shopify Flow"
              >
                <s-grid
                  gridTemplateColumns="auto 1fr auto"
                  alignItems="stretch"
                  gap="base"
                >
                  <s-thumbnail
                    size="small"
                    src="https://cdn.shopify.com/app-store/listing_images/15100ebca4d221b650a7671125cd1444/icon/CO25r7-jh4ADEAE=.png"
                    alt="Shopify Flow icon"
                   />
                  <s-box>
                    <s-heading>Shopify Flow</s-heading>
                    <s-paragraph>Free</s-paragraph>
                    <s-paragraph>
                      Automate everything and get back to business.
                    </s-paragraph>
                  </s-box>
                  <s-stack justifyContent="start">
                    <s-button
                      href="https://apps.shopify.com/flow"
                      icon="download"
                      accessibilityLabel="Download Shopify Flow"
                     />
                  </s-stack>
                </s-grid>
              </s-clickable>
              {/* Featured app 2 */}
              <s-clickable
                href="https://apps.shopify.com/planet"
                border="base"
                borderRadius="base"
                padding="base"
                inlineSize="100%"
                accessibilityLabel="Download Shopify Planet"
              >
                <s-grid
                  gridTemplateColumns="auto 1fr auto"
                  alignItems="stretch"
                  gap="base"
                >
                  <s-thumbnail
                    size="small"
                    src="https://cdn.shopify.com/app-store/listing_images/87176a11f3714753fdc2e1fc8bbf0415/icon/CIqiqqXsiIADEAE=.png"
                    alt="Shopify Planet icon"
                   />
                  <s-box>
                    <s-heading>Shopify Planet</s-heading>
                    <s-paragraph>Free</s-paragraph>
                    <s-paragraph>
                      Offer carbon-neutral shipping and showcase your commitment.
                    </s-paragraph>
                  </s-box>
                  <s-stack justifyContent="start">
                    <s-button
                      href="https://apps.shopify.com/planet"
                      icon="download"
                      accessibilityLabel="Download Shopify Planet"
                     />
                  </s-stack>
                </s-grid>
              </s-clickable>
            </s-grid>
          </s-section>
        )}
  </s-page>
  )
  ```

  ##### html

  ```html
  <!DOCTYPE html>
  <html lang="en">
    <head>
      <meta charset="UTF-8" />
      <meta name="viewport" content="width=device-width, initial-scale=1.0" />
      <script src="https://cdn.shopify.com/shopifycloud/polaris.js"></script>
      <title>Pattern</title>
      <script>
        // Simple global object to store handlers
        window.puzzleApp = {
          progress: 0,

          // Banner handlers
          dismissBanner: function(bannerElement) {
            if (bannerElement) {
              bannerElement.style.display = 'none';
            }
          },

          // Guide handlers
          dismissGuide: function(guideSection) {
            if (guideSection) {
              guideSection.style.display = 'none';
            }
          },

          toggleGuide: function(button, container) {
            if (button && container) {
              const isExpanded = container.style.display !== 'none';
              container.style.display = isExpanded ? 'none' : 'block';
              button.setAttribute('icon', isExpanded ? 'chevron-down' : 'chevron-up');
            }
          },

          // Step handlers
          toggleStep: function(button, detailsContainer) {
            if (button && detailsContainer) {
              const isExpanded = detailsContainer.style.display !== 'none';
              detailsContainer.style.display = isExpanded ? 'none' : 'block';
              button.setAttribute('icon', isExpanded ? 'chevron-down' : 'chevron-up');
            }
          },

          // Checkbox handlers
          updateProgress: function(checkbox, progressElement) {
            if (checkbox && progressElement) {
              this.progress += checkbox.checked ? 1 : -1;
              progressElement.textContent = `${this.progress} out of 3 steps completed`;
            }
          },

          // Section dismissal handlers
          dismissSection: function(section) {
            if (section) {
              section.style.display = 'none';
            }
          }
        };
      </script>
    </head>
    <body>
      <!-- === -->
      <!-- Home page pattern -->
      <!-- === -->
      <s-page>
        <s-button slot="primary-action">Create puzzle</s-button>
        <s-button slot="secondary-actions">Browse templates</s-button>
        <s-button slot="secondary-actions">Import image</s-button>
        <!-- === -->
        <!-- Banner -->
        <!-- Use banners sparingly. Only one banner should be visible at a time. -->
        <!-- If dismissed, use local storage or a database entry to avoid showing this section again to the same user. -->
        <!-- === -->
        <s-banner
          id="upgrade-banner"
          dismissible
          onDismiss="window.puzzleApp.dismissBanner(this)"
        >
          3 of 5 puzzles created.
          <s-link href="#">Upgrade to Puzzlify Pro</s-link> to create unlimited puzzles.
        </s-banner>
        <!-- === -->
        <!-- Setup Guide -->
        <!-- Keep instructions brief and direct. Only ask merchants for required information. -->
        <!-- If dismissed, use local storage or a database entry to avoid showing this section again to the same user. -->
        <!-- === -->
        <s-section id="setup-guide-section">
          <s-grid gap="base">
            <!-- Header -->
            <s-grid gap="small-200">
              <s-grid gridTemplateColumns="1fr auto auto" gap="small-300" alignItems="center">
                <s-heading>Setup Guide</s-heading>
                <s-button
                  accessibilityLabel="Dismiss Guide"
                  onClick="window.puzzleApp.dismissGuide(document.getElementById('setup-guide-section'))"
                  variant="tertiary"
                  tone="neutral"
                  icon="x"
                ></s-button>
                <s-button
                  id="toggle-guide-button"
                  accessibilityLabel="Toggle setup guide"
                  onClick="window.puzzleApp.toggleGuide(this, document.getElementById('steps-container'))"
                  variant="tertiary"
                  tone="neutral"
                  icon="chevron-up"
                ></s-button>
              </s-grid>
              <s-paragraph>
                Use this personalized guide to get your store ready for sales.
              </s-paragraph>
                <s-paragraph id="progress-text" color="subdued">0 out of 3 steps completed</s-paragraph>
            </s-grid>
            <!-- Steps Container -->
            <s-box id="steps-container" borderRadius="base" border="base" background="base">
              <!-- Step 1 -->
              <s-box>
                  <s-grid gridTemplateColumns="1fr auto" gap="base" padding="small">
                    <s-checkbox
                      label="Upload an image for your puzzle"
                      onInput="window.puzzleApp.updateProgress(this, document.getElementById('progress-text'))"
                    ></s-checkbox>
                    <s-button
                      id="toggle-step1-button"
                      onClick="window.puzzleApp.toggleStep(this, document.getElementById('step1-details'))"
                      accessibilityLabel="Toggle step 1 details"
                      variant="tertiary"
                      icon="chevron-down"
                    ></s-button>
                  </s-grid>
                <s-box id="step1-details" padding="small" paddingBlockStart="none" style="display: none;">
                  <s-box padding="base" background="subdued" borderRadius="base">
                    <s-grid gridTemplateColumns="1fr auto" gap="base">
                      <s-grid gap="small-200">
                        <s-paragraph>
                          Start by uploading a high-quality image that will be used to create your
                          puzzle. For best results, use images that are at least 1200x1200 pixels.
                        </s-paragraph>
                        <s-stack direction="inline" gap="small-200">
                          <s-button variant="primary">
                            Upload image
                          </s-button>
                          <s-button variant="tertiary" tone="neutral"> Image requirements </s-button>
                        </s-stack>
                      </s-grid>
                      <s-box maxBlockSize="80px" maxInlineSize="80px">
                        <s-image
                          src="https://cdn.shopify.com/s/assets/admin/checkout/settings-customizecart-705f57c725ac05be5a34ec20c05b94298cb8afd10aac7bd9c7ad02030f48cfa0.svg"
                          alt="Customize checkout illustration"
                        ></s-image>
                      </s-box>
                    </s-grid>
                  </s-box>
                </s-box>
              </s-box>
              <!-- Step 2 -->
              <s-divider></s-divider>
              <s-box>
                  <s-grid gridTemplateColumns="1fr auto" gap="base" padding="small">
                    <s-checkbox
                      label="Choose a puzzle template"
                      onInput="window.puzzleApp.updateProgress(this, document.getElementById('progress-text'))"
                    ></s-checkbox>
                    <s-button
                      id="toggle-step2-button"
                      onClick="window.puzzleApp.toggleStep(this, document.getElementById('step2-details'))"
                      accessibilityLabel="Toggle step 2 details"
                      variant="tertiary"
                      icon="chevron-down"
                    ></s-button>
                  </s-grid>
                <s-box id="step2-details" padding="small" paddingBlockStart="none" style="display: none;">
                  <s-box padding="base" background="subdued" borderRadius="base">
                    <s-grid gridTemplateColumns="1fr auto" gap="base">
                      <s-grid gap="small-200">
                        <s-paragraph>
                          Select a template for your puzzle - choose between 9-piece (beginner),
                          16-piece (intermediate), or 25-piece (advanced) layouts.
                        </s-paragraph>
                        <s-stack direction="inline" gap="small-200">
                          <s-button variant="primary">Choose template</s-button>
                          <s-button variant="tertiary" tone="neutral"> See all templates </s-button>
                        </s-stack>
                      </s-grid>
                      <s-box maxBlockSize="80px" maxInlineSize="80px">
                        <s-image
                          src="https://cdn.shopify.com/s/assets/admin/checkout/settings-customizecart-705f57c725ac05be5a34ec20c05b94298cb8afd10aac7bd9c7ad02030f48cfa0.svg"
                          alt="Customize checkout illustration"
                        ></s-image>
                      </s-box>
                    </s-grid>
                  </s-box>
                </s-box>
              </s-box>
              <!-- Step 3 -->
              <s-divider></s-divider>
              <s-box>
                  <s-grid gridTemplateColumns="1fr auto" gap="base" padding="small">
                    <s-checkbox
                      label="Customize puzzle piece shapes"
                      onInput="window.puzzleApp.updateProgress(this, document.getElementById('progress-text'))"
                    ></s-checkbox>
                    <s-button
                      id="toggle-step3-button"
                      onClick="window.puzzleApp.toggleStep(this, document.getElementById('step3-details'))"
                      accessibilityLabel="Toggle step 3 details"
                      variant="tertiary"
                      icon="chevron-down"
                    ></s-button>
                  </s-grid>
                </s-box>
                <s-box id="step3-details" padding="small" paddingBlockStart="none" style="display: none;">
                  <s-box padding="base" background="subdued" borderRadius="base">
                    <s-grid gridTemplateColumns="1fr auto" gap="base">
                      <s-grid gap="small-200">
                        <s-paragraph>
                          Make your puzzle unique by customizing the shapes of individual pieces.
                          Choose from classic, curved, or themed piece styles.
                        </s-paragraph>
                        <s-stack direction="inline" gap="small-200">
                          <s-button variant="primary"> Customize pieces </s-button>
                          <s-button variant="tertiary" tone="neutral">
                            Learn about piece styles
                          </s-button>
                        </s-stack>
                      </s-grid>
                      <s-box maxBlockSize="80px" maxInlineSize="80px">
                        <s-image
                          src="https://cdn.shopify.com/s/assets/admin/checkout/settings-customizecart-705f57c725ac05be5a34ec20c05b94298cb8afd10aac7bd9c7ad02030f48cfa0.svg"
                          alt="Customize checkout illustration"
                        ></s-image>
                      </s-box>
                    </s-grid>
                </s-box>
              </s-box>
              <!-- Add additional steps here... -->
            </s-box>
          </s-grid>
        </s-section>
        <!-- === -->
        <!-- Metrics cards -->
        <!-- Your app homepage should provide merchants with quick statistics or status updates that help them understand how the app is performing for them. -->
        <!-- === -->
        <s-section padding="small">
            <s-grid
              gridTemplateColumns="@container (inline-size <= 400px) 1fr, 1fr auto 1fr auto 1fr"
              gap="small"
            >
              <s-clickable
                href="#"
                paddingBlock="small-400"
                paddingInline="small-100"
                borderRadius="base"
              >
                <s-grid gap="small-300">
                  <s-heading>Total Designs</s-heading>
                  <s-stack direction="inline" gap="small-200">
                    <s-text>156</s-text>
                    <s-badge tone="success" icon="arrow-up"> 12% </s-badge>
                  </s-stack>
                </s-grid>
              </s-clickable>
              <s-divider direction="block"></s-divider>
              <s-clickable
                href="#"
                paddingBlock="small-400"
                paddingInline="small-100"
                borderRadius="base"
              >
                <s-grid gap="small-300">
                  <s-heading>Units Sold</s-heading>
                  <s-stack direction="inline" gap="small-200">
                    <s-text>2,847</s-text>
                    <s-badge tone="warning">0%</s-badge>
                  </s-stack>
                </s-grid>
              </s-clickable>
              <s-divider direction="block"></s-divider>
              <s-clickable
                href="#"
                paddingBlock="small-400"
                paddingInline="small-100"
                borderRadius="base"
              >
                <s-grid gap="small-300">
                  <s-heading>Return Rate</s-heading>
                  <s-stack direction="inline" gap="small-200">
                    <s-text>3.2%</s-text>
                    <s-badge tone="critical" icon="arrow-down"> 0.8% </s-badge>
                  </s-stack>
                </s-grid>
              </s-clickable>
            </s-grid>
        </s-section>
        <!-- === -->
        <!-- Callout Card -->
        <!-- If dismissed, use local storage or a database entry to avoid showing this section again to the same user. -->
        <!-- === -->
        <s-section id="callout-section">
          <s-grid gridTemplateColumns="1fr auto" gap="small-400" alignItems="start">
            <s-grid
              gridTemplateColumns="@container (inline-size <= 480px) 1fr, auto auto"
              gap="base"
              alignItems="center"
            >
              <s-grid gap="small-200">
                <s-heading>Ready to create your custom puzzle?</s-heading>
                <s-paragraph>
                  Start by uploading an image to your gallery or choose from one of our templates.
                </s-paragraph>
                <s-stack direction="inline" gap="small-200">
                  <s-button> Upload image </s-button>
                  <s-button tone="neutral" variant="tertiary"> Browse templates </s-button>
                </s-stack>
              </s-grid>
                <s-box maxInlineSize="200px" borderRadius="base" overflow="hidden">
                  <s-image
                    src="https://cdn.shopify.com/static/images/polaris/patterns/callout.png"
                    alt="Customize checkout illustration"
                    aspectRatio="1/0.5"
                  ></s-image>
                </s-box>
            </s-grid>
            <s-button
              onClick="window.puzzleApp.dismissSection(document.getElementById('callout-section'))"
              icon="x"
              tone="neutral"
              variant="tertiary"
              accessibilityLabel="Dismiss card"
            ></s-button>
          </s-grid>
        </s-section>
        <!-- === -->
        <!-- Puzzle templates -->
        <!-- === -->
        <s-section>
          <s-heading>Puzzle Templates</s-heading>
          <s-grid gridTemplateColumns="repeat(auto-fit, minmax(155px, 1fr))" gap="base">
            <!-- Featured template 1 -->
            <s-box border="base" borderRadius="base" overflow="hidden">
              <s-clickable href="/puzzles/4-piece" accessibilityLabel="4-pieces puzzle template">
                <s-image
                  aspectRatio="1/1"
                  objectFit="cover"
                  alt="4-pieces puzzle template"
                  src="https://cdn.shopify.com/static/images/polaris/patterns/4-pieces.png"
                ></s-image>
              </s-clickable>
              <s-divider></s-divider>
              <s-grid
                gridTemplateColumns="1fr auto"
                background="base"
                padding="small"
                gap="small"
                alignItems="center"
              >
                <s-heading>4-Pieces</s-heading>
                <s-button href="/puzzles/4-piece" accessibilityLabel="View 4-pieces puzzle template">
                  View
                </s-button>
              </s-grid>
            </s-box>
            <!-- Featured template 2 -->
            <s-box border="base" borderRadius="base" background="transparent" overflow="hidden">
              <s-clickable href="/puzzles/9-piece" accessibilityLabel="9-pieces puzzle template">
                <s-image
                  aspectRatio="1/1"
                  objectFit="cover"
                  alt="9-pieces puzzle template"
                  src="https://cdn.shopify.com/static/images/polaris/patterns/9-pieces.png"
                ></s-image>
              </s-clickable>
              <s-divider></s-divider>
              <s-grid
                gridTemplateColumns="1fr auto"
                background="base"
                padding="small"
                gap="small"
                alignItems="center"
              >
                <s-heading>9-Pieces</s-heading>
                <s-button href="/puzzles/9-piece" accessibilityLabel="View 9-pieces puzzle template">
                  View
                </s-button>
              </s-grid>
            </s-box>
            <!-- Featured template 3 -->
            <s-box border="base" borderRadius="base" background="transparent" overflow="hidden">
              <s-clickable href="/puzzles/16-piece" accessibilityLabel="16-pieces puzzle template">
                <s-image
                  aspectRatio="1/1"
                  objectFit="cover"
                  alt="16-pieces puzzle template"
                  src="https://cdn.shopify.com/static/images/polaris/patterns/16-pieces.png"
                ></s-image>
              </s-clickable>
              <s-divider></s-divider>
              <s-grid
                gridTemplateColumns="1fr auto"
                background="base"
                padding="small"
                gap="small"
                alignItems="center"
              >
                <s-heading>16-Pieces</s-heading>
                <s-button
                  href="/puzzles/16-piece"
                  accessibilityLabel="View 16-pieces puzzle template"
                >
                  View
                </s-button>
              </s-grid>
            </s-box>
          </s-grid>
          <s-stack
            direction="inline"
            alignItems="center"
            justifyContent="center"
            paddingBlockStart="base"
          >
            <s-link href="/puzzles">See all puzzle templates</s-link>
          </s-stack>
        </s-section>
        <!-- === -->
        <!-- News -->
        <!-- === -->
        <s-section>
          <s-heading>News</s-heading>
          <s-grid gridTemplateColumns="repeat(auto-fit, minmax(240px, 1fr))" gap="base">
            <!-- News item 1 -->
            <s-grid
              background="base"
              border="base"
              borderRadius="base"
              padding="base"
              gap="small-400"
            >
              <s-text>Jan 21, 2025</s-text>
              <s-link href="/news/new-shapes-and-themes">
                <s-heading>New puzzle shapes and themes added</s-heading>
              </s-link>
              <s-paragraph>
                We've added 5 new puzzle piece shapes and 3 seasonal themes to help you create more
                engaging and unique puzzles for your customers.
              </s-paragraph>
            </s-grid>
            <!-- News item 2 -->
            <s-grid
              background="base"
              border="base"
              borderRadius="base"
              padding="base"
              gap="small-400"
            >
              <s-text>Nov 6, 2024</s-text>
              <s-link href="/news/puzzle-difficulty-customization">
                <s-heading>Puzzle difficulty customization features</s-heading>
              </s-link>
              <s-paragraph>
                Now you can fine-tune the difficulty of your puzzles with new rotation controls, edge
                highlighting options, and piece recognition settings.
              </s-paragraph>
            </s-grid>
          </s-grid>
          <s-stack
            direction="inline"
            alignItems="center"
            justifyContent="center"
            paddingBlockStart="base"
          >
            <s-link href="/news">See all news items</s-link>
          </s-stack>
        </s-section>
        <!-- === -->
        <!-- Featured apps -->
        <!-- If dismissed, use local storage or a database entry to avoid showing this section again to the same user. -->
        <!-- === -->
        <s-section id="featured-apps-section">
          <s-grid gridTemplateColumns="1fr auto" alignItems="center" paddingBlockEnd="small-400">
            <s-heading>Featured apps</s-heading>
            <s-button
              onClick="window.puzzleApp.dismissSection(document.getElementById('featured-apps-section'))"
              icon="x"
              tone="neutral"
              variant="tertiary"
              accessibilityLabel="Dismiss featured apps section"
            ></s-button>
          </s-grid>
          <s-grid gridTemplateColumns="repeat(auto-fit, minmax(240px, 1fr))" gap="base">
            <!-- Featured app 1 -->
            <s-clickable
              href="https://apps.shopify.com/flow"
              border="base"
              borderRadius="base"
              padding="base"
              inlineSize="100%"
              accessibilityLabel="Download Shopify Flow"
            >
              <s-grid gridTemplateColumns="auto 1fr auto" alignItems="stretch" gap="base">
                  <s-thumbnail 
                    size="small"
                    src="https://cdn.shopify.com/app-store/listing_images/15100ebca4d221b650a7671125cd1444/icon/CO25r7-jh4ADEAE=.png"
                    alt="Shopify Flow icon"
                  ></s-thumbnail>
                <s-box>
                  <s-heading>Shopify Flow</s-heading>
                  <s-paragraph>Free</s-paragraph>
                  <s-paragraph> Automate everything and get back to business. </s-paragraph>
                </s-box>
                <s-stack justifyContent="start">
                  <s-button
                    href="https://apps.shopify.com/flow"
                    icon="download"
                    accessibilityLabel="Download Shopify Flow"
                  ></s-button>
                </s-stack>
              </s-grid>
            </s-clickable>
            <!-- Featured app 2 -->
            <s-clickable
              href="https://apps.shopify.com/planet"
              border="base"
              borderRadius="base"
              padding="base"
              inlineSize="100%"
              accessibilityLabel="Download Shopify Planet"
            >
              <s-grid gridTemplateColumns="auto 1fr auto" alignItems="stretch" gap="base">
                  <s-thumbnail
                    size="small"
                    src="https://cdn.shopify.com/app-store/listing_images/87176a11f3714753fdc2e1fc8bbf0415/icon/CIqiqqXsiIADEAE=.png"
                    alt="Shopify Planet icon"
                  ></s-thumbnail>
                <s-box>
                  <s-heading>Shopify Planet</s-heading>
                  <s-paragraph>Free</s-paragraph>
                  <s-paragraph>
                    Offer carbon-neutral shipping and showcase your commitment.
                  </s-paragraph>
                </s-box>
                <s-stack justifyContent="start">
                  <s-button
                    href="https://apps.shopify.com/planet"
                    icon="download"
                    accessibilityLabel="Download Shopify Planet"
                  ></s-button>
                </s-stack>
              </s-grid>
            </s-clickable>
          </s-grid>
        </s-section>
      </s-page>
    </body>
  </html>
  ```

## Related

[Requirements - Built for Shopify](https://shopify.dev/docs/apps/launch/built-for-shopify/requirements)

</page>

<page>
---
title: Index
description: >-
  The index layout lets merchants view and manage all their objects at once in a
  table format. They can filter, sort and do quick actions on their objects. To
  prevent tables from becoming visually cluttered, reveal actions only when the
  row is hovered over or selected

    | Used to | Examples |
    | --- | --- |
    | View all objects at once | Products, orders, customers, discounts |
    | Perform bulk actions | Delete products, pause/activate campaigns |

    ![Preview of the index pattern](https://cdn.shopify.com/shopifycloud/shopify-dev/development/assets/assets/images/templated-apis-screenshots/admin/patterns/index-example-BnCMe8_K.png)

    This pattern uses `Badge`, `Box`, `Button`, `Clickable`, `Grid`, `Heading`, `Image`, `Link`, `Paragraph`, `Section`, `Stack`, and `Table` components.

    ---

    ## Design guidelines
    Design your index page so users can organize and take action on resource objects.

    ### Navigation

    * Users must be able to return to the previous page without using the browser button. To achieve this, your app can provide breadcrumbs or a Back button on the page.
    * Offer users clear and predictable action labels.

    ---

    ### Layout

    * Design your app to be responsive and adapt to different screen sizes and devices. This ensures a seamless user experience across various platforms.
    * For resource index pages, use a full-width page. This is helpful when users are dealing with lists of data that have many columns.

    ---

    <style>
            div[class*="CodeBlock-module-CodeBlock_"] {
          max-height: calc(100vh - 400px) !important;
      }
      div[class*="Tabs-module-TabsContent_"] {
        overflow: auto !important;
      }
      div[class*="Screenshot-module-Screenshot_"] {
        display: none !important;
      }
    </style>

api_name: app-home
source_url:
  html: '<https://shopify.dev/docs/api/app-home/patterns/templates/index>'
  md: '<https://shopify.dev/docs/api/app-home/patterns/templates/index.md>'
---

# Index

The index layout lets merchants view and manage all their objects at once in a table format. They can filter, sort and do quick actions on their objects. To prevent tables from becoming visually cluttered, reveal actions only when the row is hovered over or selected

| Used to | Examples |
| - | - |
| View all objects at once | Products, orders, customers, discounts |
| Perform bulk actions | Delete products, pause/activate campaigns |

![Preview of the index pattern](https://cdn.shopify.com/shopifycloud/shopify-dev/development/assets/assets/images/templated-apis-screenshots/admin/patterns/index-example-BnCMe8_K.png)

This pattern uses `Badge`, `Box`, `Button`, `Clickable`, `Grid`, `Heading`, `Image`, `Link`, `Paragraph`, `Section`, `Stack`, and `Table` components.

***

## Design guidelines

Design your index page so users can organize and take action on resource objects.

### Navigation

- Users must be able to return to the previous page without using the browser button. To achieve this, your app can provide breadcrumbs or a Back button on the page.
- Offer users clear and predictable action labels.

***

### Layout

- Design your app to be responsive and adapt to different screen sizes and devices. This ensures a seamless user experience across various platforms.
- For resource index pages, use a full-width page. This is helpful when users are dealing with lists of data that have many columns.

***

Examples

### Examples

- #### Index

  ##### jsx

  ```jsx
  <s-page heading="Puzzles">
        <s-button slot="primary-action" variant="primary">
          Create puzzle
        </s-button>
        <s-button slot="secondary-actions" variant="secondary">
          Export puzzles
        </s-button>
        <s-button slot="secondary-actions" variant="secondary">
          Import puzzles
        </s-button>
        {/* === */}
        {/* Empty state */}
        {/* This should only be visible if the merchant has not created any puzzles yet. */}
        {/* === */}
        <s-section accessibilityLabel="Empty state section">
          <s-grid gap="base" justifyItems="center" paddingBlock="large-400">
            <s-box maxInlineSize="200px" maxBlockSize="200px">
              {/* aspectRatio should match the actual image dimensions (width/height) */}
              <s-image
                aspectRatio="1/0.5"
                src="https://cdn.shopify.com/static/images/polaris/patterns/callout.png"
                alt="A stylized graphic of four characters, each holding a puzzle piece"
              />
            </s-box>
            <s-grid justifyItems="center" maxInlineSize="450px" gap="base">
              <s-stack alignItems="center">
                <s-heading>Start creating puzzles</s-heading>
                <s-paragraph>
                  Create and manage your collection of puzzles for players to
                  enjoy.
                </s-paragraph>
              </s-stack>
              <s-button-group>
                <s-button
                  slot="secondary-actions"
                  accessibilityLabel="Learn more about creating puzzles"
                >
                  {" "}
                  Learn more{" "}
                </s-button>
                <s-button slot="primary-action" accessibilityLabel="Add a new puzzle">
                  {" "}
                  Create puzzle{" "}
                </s-button>
              </s-button-group>
            </s-grid>
          </s-grid>
        </s-section>

        {/* === */}
        {/* Table */}
        {/* This should only be visible if the merchant has created one or more puzzles. */}
        {/* === */}
        <s-section padding="none" accessibilityLabel="Puzzles table section">
          <s-table>
            <s-table-header-row>
              <s-table-header listSlot="primary">Puzzle</s-table-header>
              <s-table-header format="numeric">Pieces</s-table-header>
              <s-table-header>Created</s-table-header>
              <s-table-header>Status</s-table-header>
            </s-table-header-row>
            <s-table-body>
              <s-table-row>
                <s-table-cell>
                  <s-stack direction="inline" gap="small" alignItems="center">
                    <s-clickable
                      href="/app/details"
                      accessibilityLabel="Mountain View puzzle thumbnail"
                      border="base"
                      borderRadius="base"
                      overflow="hidden"
                      inlineSize="40px"
                      blockSize="40px"
                    >
                      <s-image
                        objectFit="cover"
                        alt="Mountain View puzzle thumbnail"
                        src="https://picsum.photos/id/29/80/80"
                       />
                    </s-clickable>
                    <s-link href="/app/details">Mountain View</s-link>
                  </s-stack>
                </s-table-cell>
                <s-table-cell>16</s-table-cell>
                <s-table-cell>Today</s-table-cell>
                <s-table-cell>
                  <s-badge color="base" tone="success">
                    Active
                  </s-badge>
                </s-table-cell>
              </s-table-row>
              <s-table-row>
                <s-table-cell>
                  <s-stack direction="inline" gap="small" alignItems="center">
                    <s-clickable
                      href="/app/details"
                      accessibilityLabel="Ocean Sunset puzzle thumbnail"
                      border="base"
                      borderRadius="base"
                      overflow="hidden"
                      inlineSize="40px"
                      blockSize="40px"
                    >
                      <s-image
                        objectFit="cover"
                        alt="Ocean Sunset puzzle thumbnail"
                        src="https://picsum.photos/id/12/80/80"
                       />
                    </s-clickable>
                    <s-link href="/app/details">Ocean Sunset</s-link>
                  </s-stack>
                </s-table-cell>
                <s-table-cell>9</s-table-cell>
                <s-table-cell>Yesterday</s-table-cell>
                <s-table-cell>
                  <s-badge color="base" tone="success">
                    Active
                  </s-badge>
                </s-table-cell>
              </s-table-row>
              <s-table-row>
                <s-table-cell>
                  <s-stack direction="inline" gap="small" alignItems="center">
                    <s-clickable
                      href="/app/details"
                      accessibilityLabel="Forest Animals puzzle thumbnail"
                      border="base"
                      borderRadius="base"
                      overflow="hidden"
                      inlineSize="40px"
                      blockSize="40px"
                    >
                      <s-image
                        objectFit="cover"
                        alt="Forest Animals puzzle thumbnail"
                        src="https://picsum.photos/id/324/80/80"
                       />
                    </s-clickable>
                    <s-link href="/app/details">Forest Animals</s-link>
                  </s-stack>
                </s-table-cell>
                <s-table-cell>25</s-table-cell>
                <s-table-cell>Last week</s-table-cell>
                <s-table-cell>
                  <s-badge color="base" tone="neutral">
                    Draft
                  </s-badge>
                </s-table-cell>
              </s-table-row>
              {/* Add more rows as needed here */}
              {/* If more than 100 rows are needed, index page tables should use the paginate, hasPreviousPage, hasNextPage, onPreviousPage, and onNextPage attributes to display and handle pagination) */}
            </s-table-body>
        </s-table>
      </s-section>
  </s-page>
  ```

  ##### html

  ```html
  <!DOCTYPE html>
  <html lang="en">
    <head>
      <meta charset="UTF-8" />
      <meta name="viewport" content="width=device-width, initial-scale=1.0" />
      <script src="https://cdn.shopify.com/shopifycloud/polaris.js"></script>
      <title>Pattern</title>
    </head>
    <body>
      <!-- === -->
      <!-- Index page pattern -->
      <!-- === -->
      <s-page heading="Puzzles">
        <s-button slot="primary-action" variant="primary">Create puzzle</s-button>
        <s-button slot="secondary-actions" variant="secondary">Export puzzles</s-button>
        <s-button slot="secondary-actions" variant="secondary">Import puzzles</s-button>
        <!-- === -->
        <!-- Empty state -->
        <!-- This should only be visible if the merchant has not created any puzzles yet. -->
        <!-- === -->
        <s-section accessibilityLabel="Empty state section">
          <s-grid gap="base" justifyItems="center" paddingBlock="large-400">
            <s-box maxInlineSize="200px" maxBlockSize="200px">
              <!-- aspectRatio should match the actual image dimensions (width/height) -->
              <s-image
                maxInlineSize="200px"
                maxBlockSize="200px"
                aspectRatio="1/0.5"
                src="https://cdn.shopify.com/static/images/polaris/patterns/callout.png"
                alt="A stylized graphic of four characters, each holding a puzzle piece"
              />
            </s-box>
            <s-grid justifyItems="center" maxInlineSize="450px" gap="base">
              <s-stack alignItems="center">
                <s-heading>Start creating puzzles</s-heading>
                <s-paragraph>
                  Create and manage your collection of puzzles for players to enjoy.
                </s-paragraph>
              </s-stack>
              <s-button-group>
                <s-button slot="secondary-actions" aria-label="Learn more about creating puzzles"> Learn more </s-button>
                <s-button slot="primary-action" aria-label="Add a new puzzle"> Create puzzle </s-button>
              </s-button-group>
            </s-grid>
          </s-grid>
        </s-section>
        <!-- === -->
        <!-- Table -->
        <!-- This should only be visible if the merchant has created one or more puzzles. -->
        <!-- === -->
        <s-section padding="none" accessibilityLabel="Puzzles table section">
          <s-table>
            <s-table-header-row>
              <s-table-header listSlot="primary">Puzzle</s-table-header>
              <s-table-header format="numeric">Pieces</s-table-header>
              <s-table-header>Created</s-table-header>
              <s-table-header>Status</s-table-header>
            </s-table-header-row>
            <s-table-body>
              <s-table-row>
                <s-table-cell>
                  <s-stack direction="inline" gap="small" alignItems="center">
                    <s-clickable
                      href="/app/details"
                      accessibilityLabel="Mountain View puzzle thumbnail"
                      border="base"
                      borderRadius="base"
                      overflow="hidden"
                      inlineSize="40px"
                      blockSize="40px"
                    >
                      <s-image objectFit="cover" alt="Mountain View puzzle thumbnail" src="https://picsum.photos/id/29/80/80"></s-image>
                    </s-clickable>
                    <s-link href="/app/details">Mountain View</s-link>
                  </s-stack>
                </s-table-cell>
                <s-table-cell>16</s-table-cell>
                <s-table-cell>Today</s-table-cell>
                <s-table-cell>
                  <s-badge color="base" tone="success"> Active </s-badge>
                </s-table-cell>
              </s-table-row>
              <s-table-row>
                <s-table-cell>
                  <s-stack direction="inline" gap="small" alignItems="center">
                    <s-clickable
                      href="/app/details"
                      accessibilityLabel="Ocean Sunset puzzle thumbnail"
                      border="base"
                      borderRadius="base"
                      overflow="hidden"
                      inlineSize="40px"
                      blockSize="40px"
                    >
                      <s-image objectFit="cover" alt="Ocean Sunset puzzle thumbnail" src="https://picsum.photos/id/12/80/80"></s-image>
                    </s-clickable>
                    <s-link href="/app/details">Ocean Sunset</s-link>
                  </s-stack>
                </s-table-cell>
                <s-table-cell>9</s-table-cell>
                <s-table-cell>Yesterday</s-table-cell>
                <s-table-cell>
                  <s-badge color="base" tone="success"> Active </s-badge>
                </s-table-cell>
              </s-table-row>
              <s-table-row>
                <s-table-cell>
                  <s-stack direction="inline" gap="small" alignItems="center">
                    <s-clickable
                      href="/app/details"
                      accessibilityLabel="Forest Animals puzzle thumbnail"
                      border="base"
                      borderRadius="base"
                      overflow="hidden"
                      inlineSize="40px"
                      blockSize="40px"
                    >
                      <s-image objectFit="cover" alt="Forest Animals puzzle thumbnail" src="https://picsum.photos/id/324/80/80"></s-image>
                    </s-clickable>
                    <s-link href="/app/details">Forest Animals</s-link>
                  </s-stack>
                </s-table-cell>
                <s-table-cell>25</s-table-cell>
                <s-table-cell>Last week</s-table-cell>
                <s-table-cell>
                  <s-badge color="base" tone="neutral"> Draft </s-badge>
                </s-table-cell>
              </s-table-row>
              <!-- Add more rows as needed here -->
              <!-- If more than 100 rows are needed, index page tables should use the paginate, hasPreviousPage, hasNextPage, onPreviousPage, and onNextPage attributes to display and handle pagination) -->
            </s-table-body>
          </s-table>
        </s-section>
      </s-page>
    </body>
  </html>
  ```

## Related

[Requirements - Built for Shopify](https://shopify.dev/docs/apps/launch/built-for-shopify/requirements)

</page>

<page>
---
title: Settings
description: >-
  Make settings pages easy to scan by grouping related information in a logical
  order. For complex or lengthy settings, organize content into distinct, themed
  sections that link to their own pages.
    | Used to | Examples |
    | --- | --- |
    | Find and change app settings | Membership settings, app appearance, set up theme blocks |

    ![Preview of the settings pattern](https://cdn.shopify.com/shopifycloud/shopify-dev/development/assets/assets/images/templated-apis-screenshots/admin/patterns/settings-example-B3J4JHfL.png)

    This pattern uses `Box`, `Button`, `ChoiceList`, `Clickable`, `Divider`, `Grid`, `Heading`, `Icon`, `Paragraph`, `Section`, `Select`, `Stack`, and `TextField` components.

    ---

    ## Design guidelines
    Design scannable settings pages with groups of related information placed in logical order.

    ### Navigation

    * Users must be able to return to the previous page without using the browser button. To achieve this, your app can provide breadcrumbs or a Back button on the page.
    * Offer users clear and predictable action labels.

    ---

    ### Visual design

    Design your app to be responsive and adapt to different screen sizes and devices. This ensures a seamless user experience across various platforms.

    * Use looser spacing for low-density layouts. Use tighter spacing for high-density layouts.
    * Use high-resolution photos and images to ensure a professional, high-quality experience.

    ---

    <style>
      div[class*="CodeBlock-module-CodeBlock_"] {
        max-height: calc(100vh - 80px) !important;
      }
      div[class*="Tabs-module-TabsContent_"] {
        overflow: auto !important;
      }
      div[class*="Screenshot-module-Screenshot_"] {
        display: none !important;
      }
    </style>

api_name: app-home
source_url:
  html: '<https://shopify.dev/docs/api/app-home/patterns/templates/settings>'
  md: '<https://shopify.dev/docs/api/app-home/patterns/templates/settings.md>'
---

# Settings

Make settings pages easy to scan by grouping related information in a logical order. For complex or lengthy settings, organize content into distinct, themed sections that link to their own pages.

| Used to | Examples |
| - | - |
| Find and change app settings | Membership settings, app appearance, set up theme blocks |

![Preview of the settings pattern](https://cdn.shopify.com/shopifycloud/shopify-dev/development/assets/assets/images/templated-apis-screenshots/admin/patterns/settings-example-B3J4JHfL.png)

This pattern uses `Box`, `Button`, `ChoiceList`, `Clickable`, `Divider`, `Grid`, `Heading`, `Icon`, `Paragraph`, `Section`, `Select`, `Stack`, and `TextField` components.

***

## Design guidelines

Design scannable settings pages with groups of related information placed in logical order.

### Navigation

- Users must be able to return to the previous page without using the browser button. To achieve this, your app can provide breadcrumbs or a Back button on the page.
- Offer users clear and predictable action labels.

***

### Visual design

Design your app to be responsive and adapt to different screen sizes and devices. This ensures a seamless user experience across various platforms.

- Use looser spacing for low-density layouts. Use tighter spacing for high-density layouts.
- Use high-resolution photos and images to ensure a professional, high-quality experience.

***

Examples

### Examples

- #### Settings

  ##### jsx

  ```jsx
  <form
    data-save-bar
    onSubmit={(event) => {
      event.preventDefault();
      const formData = new FormData(event.target);
      const formEntries = Object.fromEntries(formData);
      console.log("Form data", formEntries);
    }}
    onReset={(event) => {
      console.log("Handle discarded changes if necessary");
    }}
  >
        <s-page heading="Settings" inlineSize="small">
          {/* === */}
          {/* Store Information */}
          {/* === */}
          <s-section heading="Store Information">
            <s-text-field
              label="Store name"
              name="store-name"
              value="Puzzlify Store"
              placeholder="Enter store name"
            />
            <s-text-field
              label="Business address"
              name="business-address"
              value="123 Main St, Anytown, USA"
              placeholder="Enter business address"
            />
            <s-text-field
              label="Store phone"
              name="store-phone"
              value="+1 (555) 123-4567"
              placeholder="Enter phone number"
            />
            <s-choice-list label="Primary currency" name="currency">
              <s-choice value="usd" selected>
                US Dollar ($)
              </s-choice>
              <s-choice value="cad">Canadian Dollar (CAD)</s-choice>
              <s-choice value="eur">Euro (€)</s-choice>
            </s-choice-list>
          </s-section>

          {/* === */}
          {/* Notifications */}
          {/* === */}
          <s-section heading="Notifications">
            <s-select
              label="Notification frequency"
              name="notification-frequency"
            >
              <s-option value="immediately" selected>
                Immediately
              </s-option>
              <s-option value="hourly">Hourly digest</s-option>
              <s-option value="daily">Daily digest</s-option>
            </s-select>
            <s-choice-list
              label="Notification types"
              name="notifications-type"
              multiple
            >
              <s-choice value="new-order" selected>
                New order notifications
              </s-choice>
              <s-choice value="low-stock">Low stock alerts</s-choice>
              <s-choice value="customer-review">
                Customer review notifications
              </s-choice>
              <s-choice value="shipping-updates">Shipping updates</s-choice>
            </s-choice-list>
          </s-section>

          {/* === */}
          {/* Preferences */}
          {/* === */}
          <s-section heading="Preferences">
            <s-box border="base" borderRadius="base">
              <s-clickable
                padding="small-100"
                href="/app/settings/shipping"
                accessibilityLabel="Configure shipping methods, rates, and fulfillment options"
              >
                <s-grid
                  gridTemplateColumns="1fr auto"
                  alignItems="center"
                  gap="base"
                >
                  <s-box>
                    <s-heading>Shipping & fulfillment</s-heading>
                    <s-paragraph color="subdued">
                      Shipping methods, rates, zones, and fulfillment preferences.
                    </s-paragraph>
                  </s-box>
                  <s-icon type="chevron-right" />
                </s-grid>
              </s-clickable>
              <s-box paddingInline="small-100">
                <s-divider />
              </s-box>

              <s-clickable
                padding="small-100"
                href="/app/settings/products_catalog"
                accessibilityLabel="Configure product defaults, customer experience, and catalog settings"
              >
                <s-grid
                  gridTemplateColumns="1fr auto"
                  alignItems="center"
                  gap="base"
                >
                  <s-box>
                    <s-heading>Products & catalog</s-heading>
                    <s-paragraph color="subdued">
                      Product defaults, customer experience, and catalog display
                      options.
                    </s-paragraph>
                  </s-box>
                  <s-icon type="chevron-right" />
                </s-grid>
              </s-clickable>
              <s-box paddingInline="small-100">
                <s-divider />
              </s-box>

              <s-clickable
                padding="small-100"
                href="/app/settings/customer_support"
                accessibilityLabel="Manage customer support settings and help resources"
              >
                <s-grid
                  gridTemplateColumns="1fr auto"
                  alignItems="center"
                  gap="base"
                >
                  <s-box>
                    <s-heading>Customer support</s-heading>
                    <s-paragraph color="subdued">
                      Support settings, help resources, and customer service
                      tools.
                    </s-paragraph>
                  </s-box>
                  <s-icon type="chevron-right" />
                </s-grid>
              </s-clickable>
            </s-box>
          </s-section>

          {/* === */}
          {/* Tools */}
          {/* === */}
          <s-section heading="Tools">
            <s-stack
              gap="none"
              border="base"
              borderRadius="base"
              overflow="hidden"
            >
              <s-box padding="small-100">
                <s-grid
                  gridTemplateColumns="1fr auto"
                  alignItems="center"
                  gap="base"
                >
                  <s-box>
                    <s-heading>Reset app settings</s-heading>
                    <s-paragraph color="subdued">
                      Reset all settings to their default values. This action
                      cannot be undone.
                    </s-paragraph>
                  </s-box>
                  <s-button tone="critical">Reset</s-button>
                </s-grid>
              </s-box>
              <s-box paddingInline="small-100">
                <s-divider />
              </s-box>

              <s-box padding="small-100">
                <s-grid
                  gridTemplateColumns="1fr auto"
                  alignItems="center"
                  gap="base"
                >
                  <s-box>
                    <s-heading>Export settings</s-heading>
                    <s-paragraph color="subdued">
                      Download a backup of all your current settings.
                    </s-paragraph>
                  </s-box>
                  <s-button>Export</s-button>
                </s-grid>
              </s-box>
            </s-stack>
        </s-section>
    </s-page>
  </form>
  ```

  ##### html

  ```html
  <!doctype html>
  <html lang="en">
    <head>
      <meta charset="UTF-8" />
      <meta name="viewport" content="width=device-width, initial-scale=1.0" />
      <script src="https://cdn.shopify.com/shopifycloud/polaris.js"></script>
      <title>Pattern</title>
    </head>
    <body>
      <!-- === -->
      <!-- Settings page pattern -->
      <!-- === -->
      <form data-save-bar onSubmit="" onReset="">
        <s-page heading="Settings" inlineSize="small">
          <!-- === -->
          <!-- Store Information -->
          <!-- === -->
          <s-section heading="Store Information">
            <s-text-field
              label="Store name"
              name="store-name"
              value="Puzzlify Store"
              placeholder="Enter store name"
            ></s-text-field>
            <s-text-field
              label="Business address"
              name="business-address"
              value="123 Main St, Anytown, USA"
              placeholder="Enter business address"
            ></s-text-field>
            <s-text-field
              label="Store phone"
              name="store-phone"
              value="+1 (555) 123-4567"
              placeholder="Enter phone number"
            ></s-text-field>
            <s-choice-list label="Primary currency" name="currency">
              <s-choice value="usd" selected>US Dollar ($)</s-choice>
              <s-choice value="cad">Canadian Dollar (CAD)</s-choice>
              <s-choice value="eur">Euro (€)</s-choice>
            </s-choice-list>
          </s-section>
          <!-- === -->
          <!-- Notifications -->
          <!-- === -->
          <s-section heading="Notifications">
            <s-select
              label="Notification frequency"
              name="notification-frequency"
            >
              <s-option value="immediately" selected>Immediately</s-option>
              <s-option value="hourly">Hourly digest</s-option>
              <s-option value="daily">Daily digest</s-option>
            </s-select>
            <s-choice-list
              label="Notification types"
              name="notifications-type"
              multiple
            >
              <s-choice value="new-order" selected
                >New order notifications</s-choice
              >
              <s-choice value="low-stock">Low stock alerts</s-choice>
              <s-choice value="customer-review"
                >Customer review notifications</s-choice
              >
              <s-choice value="shipping-updates">Shipping updates</s-choice>
            </s-choice-list>
          </s-section>
          <!-- === -->
          <!-- Preferences -->
          <!-- === -->
          <s-section heading="Preferences">
            <s-box
              border="base"
              borderRadius="base"
            >
              <s-clickable
                padding="small-100"
                href="/app/settings/shipping"
                accessibilityLabel="Configure shipping methods, rates, and fulfillment options"
              >
                <s-grid
                  gridTemplateColumns="1fr auto"
                  alignItems="center"
                  gap="base"
                >
                  <s-box>
                    <s-heading>Shipping & fulfillment</s-heading>
                    <s-paragraph color="subdued">
                      Shipping methods, rates, zones, and fulfillment preferences.
                    </s-paragraph>
                  </s-box>
                  <s-icon type="chevron-right"></s-icon>
                </s-grid>
              </s-clickable>
              <s-box paddingInline="small-100">
                <s-divider></s-divider>
              </s-box>
              <s-clickable
                padding="small-100"
                href="/app/settings/products_catalog"
                accessibilityLabel="Configure product defaults, customer experience, and catalog settings"
              >
                <s-grid
                  gridTemplateColumns="1fr auto"
                  alignItems="center"
                  gap="base"
                >
                  <s-box>
                    <s-heading>Products & catalog</s-heading>
                    <s-paragraph color="subdued">
                      Product defaults, customer experience, and catalog display
                      options.
                    </s-paragraph>
                  </s-box>
                  <s-icon type="chevron-right"></s-icon>
                </s-grid>
              </s-clickable>
              <s-box paddingInline="small-100">
                <s-divider></s-divider>
              </s-box>
              <s-clickable
                padding="small-100"
                href="/app/settings/customer_support"
                accessibilityLabel="Manage customer support settings and help resources"
              >
                <s-grid
                  gridTemplateColumns="1fr auto"
                  alignItems="center"
                  gap="base"
                >
                  <s-box>
                    <s-heading>Customer support</s-heading>
                    <s-paragraph color="subdued">
                      Support settings, help resources, and customer service
                      tools.
                    </s-paragraph>
                  </s-box>
                  <s-icon type="chevron-right"></s-icon>
                </s-grid>
              </s-clickable>
            </s-box>
          </s-section>
          <!-- === -->
          <!-- Tools -->
          <!-- === -->
          <s-section heading="Tools">
            <s-stack
              gap="none"
              border="base"
              borderRadius="base"
              overflow="hidden"
            >
              <s-box padding="small-100">
                <s-grid
                  gridTemplateColumns="1fr auto"
                  alignItems="center"
                  gap="base"
                >
                  <s-box>
                    <s-heading>Reset app settings</s-heading>
                    <s-paragraph color="subdued">
                      Reset all settings to their default values. This action
                      cannot be undone.
                    </s-paragraph>
                  </s-box>
                  <s-button tone="critical">Reset</s-button>
                </s-grid>
              </s-box>
              <s-box paddingInline="small-100">
                <s-divider></s-divider>
              </s-box>
              <s-box padding="small-100">
                <s-grid
                  gridTemplateColumns="1fr auto"
                  alignItems="center"
                  gap="base"
                >
                  <s-box>
                    <s-heading>Export settings</s-heading>
                    <s-paragraph color="subdued">
                      Download a backup of all your current settings.
                    </s-paragraph>
                  </s-box>
                  <s-button>Export</s-button>
                </s-grid>
              </s-box>
            </s-stack>
          </s-section>
        </s-page>
      </form>
    </body>
  </html>
  ```

## Related

[Requirements - Built for Shopify](https://shopify.dev/docs/apps/launch/built-for-shopify/requirements)

</page>

<page>
---
title: Button
description: >-
  Triggers actions or events, such as submitting forms, opening dialogs, or
  navigating to other pages. Use Button to let users perform specific tasks or
  initiate interactions throughout the interface. Buttons can also function as
  links, guiding users to internal or external destinations.
api_name: app-home
source_url:
  html: 'https://shopify.dev/docs/api/app-home/polaris-web-components/actions/button'
  md: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/actions/button.md
---

# Button

Triggers actions or events, such as submitting forms, opening dialogs, or navigating to other pages. Use Button to let users perform specific tasks or initiate interactions throughout the interface. Buttons can also function as links, guiding users to internal or external destinations.

## Properties

- **accessibilityLabel**

  **string**

  A label that describes the purpose or contents of the Button. It will be read to users using assistive technologies such as screen readers.

  Use this when using only an icon or the Button text is not enough context for users using assistive technologies.

- **command**

  **'--auto' | '--show' | '--hide' | '--toggle'**

  **Default: '--auto'**

  Sets the action the [command](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/button#command) should take when this clickable is activated.

  See the documentation of particular components for the actions they support.

  - `--auto`: a default action for the target component.
  - `--show`: shows the target component.
  - `--hide`: hides the target component.
  - `--toggle`: toggles the target component.

- **commandFor**

  **string**

  Sets the element the [commandFor](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/button#commandfor) should act on when this clickable is activated.

- **disabled**

  **boolean**

  **Default: false**

  Disables the Button meaning it cannot be clicked or receive focus.

- **download**

  **string**

  Causes the browser to treat the linked URL as a download with the string being the file name. Download only works for same-origin URLs or the `blob:` and `data:` schemes.

- **href**

  **string**

  The URL to link to.

  - If set, it will navigate to the location specified by `href` after executing the `click` event.
  - If a `commandFor` is set, the `command` will be executed instead of the navigation.

- **icon**

  **"" | "replace" | "search" | "split" | "link" | "edit" | "product" | "variant" | "collection" | "select" | "info" | "incomplete" | "complete" | "color" | "money" | "order" | "code" | "adjust" | "affiliate" | "airplane" | "alert-bubble" | "alert-circle" | "alert-diamond" | "alert-location" | "alert-octagon" | "alert-octagon-filled" | "alert-triangle" | "alert-triangle-filled" | "align-horizontal-centers" | "app-extension" | "apps" | "archive" | "arrow-down" | "arrow-down-circle" | "arrow-down-right" | "arrow-left" | "arrow-left-circle" | "arrow-right" | "arrow-right-circle" | "arrow-up" | "arrow-up-circle" | "arrow-up-right" | "arrows-in-horizontal" | "arrows-out-horizontal" | "asterisk" | "attachment" | "automation" | "backspace" | "bag" | "bank" | "barcode" | "battery-low" | "bill" | "blank" | "blog" | "bolt" | "bolt-filled" | "book" | "book-open" | "bug" | "bullet" | "business-entity" | "button" | "button-press" | "calculator" | "calendar" | "calendar-check" | "calendar-compare" | "calendar-list" | "calendar-time" | "camera" | "camera-flip" | "caret-down" | "caret-left" | "caret-right" | "caret-up" | "cart" | "cart-abandoned" | "cart-discount" | "cart-down" | "cart-filled" | "cart-sale" | "cart-send" | "cart-up" | "cash-dollar" | "cash-euro" | "cash-pound" | "cash-rupee" | "cash-yen" | "catalog-product" | "categories" | "channels" | "channels-filled" | "chart-cohort" | "chart-donut" | "chart-funnel" | "chart-histogram-first" | "chart-histogram-first-last" | "chart-histogram-flat" | "chart-histogram-full" | "chart-histogram-growth" | "chart-histogram-last" | "chart-histogram-second-last" | "chart-horizontal" | "chart-line" | "chart-popular" | "chart-stacked" | "chart-vertical" | "chat" | "chat-new" | "chat-referral" | "check" | "check-circle" | "check-circle-filled" | "checkbox" | "chevron-down" | "chevron-down-circle" | "chevron-left" | "chevron-left-circle" | "chevron-right" | "chevron-right-circle" | "chevron-up" | "chevron-up-circle" | "circle" | "circle-dashed" | "clipboard" | "clipboard-check" | "clipboard-checklist" | "clock" | "clock-list" | "clock-revert" | "code-add" | "collection-featured" | "collection-list" | "collection-reference" | "color-none" | "compass" | "compose" | "confetti" | "connect" | "content" | "contract" | "corner-pill" | "corner-round" | "corner-square" | "credit-card" | "credit-card-cancel" | "credit-card-percent" | "credit-card-reader" | "credit-card-reader-chip" | "credit-card-reader-tap" | "credit-card-secure" | "credit-card-tap-chip" | "crop" | "currency-convert" | "cursor" | "cursor-banner" | "cursor-option" | "data-presentation" | "data-table" | "database" | "database-add" | "database-connect" | "delete" | "delivered" | "delivery" | "desktop" | "disabled" | "disabled-filled" | "discount" | "discount-add" | "discount-automatic" | "discount-code" | "discount-remove" | "dns-settings" | "dock-floating" | "dock-side" | "domain" | "domain-landing-page" | "domain-new" | "domain-redirect" | "download" | "drag-drop" | "drag-handle" | "drawer" | "duplicate" | "email" | "email-follow-up" | "email-newsletter" | "empty" | "enabled" | "enter" | "envelope" | "envelope-soft-pack" | "eraser" | "exchange" | "exit" | "export" | "external" | "eye-check-mark" | "eye-dropper" | "eye-dropper-list" | "eye-first" | "eyeglasses" | "fav" | "favicon" | "file" | "file-list" | "filter" | "filter-active" | "flag" | "flip-horizontal" | "flip-vertical" | "flower" | "folder" | "folder-add" | "folder-down" | "folder-remove" | "folder-up" | "food" | "foreground" | "forklift" | "forms" | "games" | "gauge" | "geolocation" | "gift" | "gift-card" | "git-branch" | "git-commit" | "git-repository" | "globe" | "globe-asia" | "globe-europe" | "globe-lines" | "globe-list" | "graduation-hat" | "grid" | "hashtag" | "hashtag-decimal" | "hashtag-list" | "heart" | "hide" | "hide-filled" | "home" | "home-filled" | "icons" | "identity-card" | "image" | "image-add" | "image-alt" | "image-explore" | "image-magic" | "image-none" | "image-with-text-overlay" | "images" | "import" | "in-progress" | "incentive" | "incoming" | "info-filled" | "inheritance" | "inventory" | "inventory-edit" | "inventory-list" | "inventory-transfer" | "inventory-updated" | "iq" | "key" | "keyboard" | "keyboard-filled" | "keyboard-hide" | "keypad" | "label-printer" | "language" | "language-translate" | "layout-block" | "layout-buy-button" | "layout-buy-button-horizontal" | "layout-buy-button-vertical" | "layout-column-1" | "layout-columns-2" | "layout-columns-3" | "layout-footer" | "layout-header" | "layout-logo-block" | "layout-popup" | "layout-rows-2" | "layout-section" | "layout-sidebar-left" | "layout-sidebar-right" | "lightbulb" | "link-list" | "list-bulleted" | "list-bulleted-filled" | "list-numbered" | "live" | "live-critical" | "live-none" | "location" | "location-none" | "lock" | "map" | "markets" | "markets-euro" | "markets-rupee" | "markets-yen" | "maximize" | "measurement-size" | "measurement-size-list" | "measurement-volume" | "measurement-volume-list" | "measurement-weight" | "measurement-weight-list" | "media-receiver" | "megaphone" | "mention" | "menu" | "menu-filled" | "menu-horizontal" | "menu-vertical" | "merge" | "metafields" | "metaobject" | "metaobject-list" | "metaobject-reference" | "microphone" | "microphone-muted" | "minimize" | "minus" | "minus-circle" | "mobile" | "money-none" | "money-split" | "moon" | "nature" | "note" | "note-add" | "notification" | "number-one" | "order-batches" | "order-draft" | "order-filled" | "order-first" | "order-fulfilled" | "order-repeat" | "order-unfulfilled" | "orders-status" | "organization" | "outdent" | "outgoing" | "package" | "package-cancel" | "package-fulfilled" | "package-on-hold" | "package-reassign" | "package-returned" | "page" | "page-add" | "page-attachment" | "page-clock" | "page-down" | "page-heart" | "page-list" | "page-reference" | "page-remove" | "page-report" | "page-up" | "pagination-end" | "pagination-start" | "paint-brush-flat" | "paint-brush-round" | "paper-check" | "partially-complete" | "passkey" | "paste" | "pause-circle" | "payment" | "payment-capture" | "payout" | "payout-dollar" | "payout-euro" | "payout-pound" | "payout-rupee" | "payout-yen" | "person" | "person-add" | "person-exit" | "person-filled" | "person-list" | "person-lock" | "person-remove" | "person-segment" | "personalized-text" | "phablet" | "phone" | "phone-down" | "phone-down-filled" | "phone-in" | "phone-out" | "pin" | "pin-remove" | "plan" | "play" | "play-circle" | "plus" | "plus-circle" | "plus-circle-down" | "plus-circle-filled" | "plus-circle-up" | "point-of-sale" | "point-of-sale-register" | "price-list" | "print" | "product-add" | "product-cost" | "product-filled" | "product-list" | "product-reference" | "product-remove" | "product-return" | "product-unavailable" | "profile" | "profile-filled" | "question-circle" | "question-circle-filled" | "radio-control" | "receipt" | "receipt-dollar" | "receipt-euro" | "receipt-folded" | "receipt-paid" | "receipt-pound" | "receipt-refund" | "receipt-rupee" | "receipt-yen" | "receivables" | "redo" | "referral-code" | "refresh" | "remove-background" | "reorder" | "replay" | "reset" | "return" | "reward" | "rocket" | "rotate-left" | "rotate-right" | "sandbox" | "save" | "savings" | "scan-qr-code" | "search-add" | "search-list" | "search-recent" | "search-resource" | "send" | "settings" | "share" | "shield-check-mark" | "shield-none" | "shield-pending" | "shield-person" | "shipping-label" | "shipping-label-cancel" | "shopcodes" | "slideshow" | "smiley-happy" | "smiley-joy" | "smiley-neutral" | "smiley-sad" | "social-ad" | "social-post" | "sort" | "sort-ascending" | "sort-descending" | "sound" | "sports" | "star" | "star-circle" | "star-filled" | "star-half" | "star-list" | "status" | "status-active" | "stop-circle" | "store" | "store-import" | "store-managed" | "store-online" | "sun" | "table" | "table-masonry" | "tablet" | "target" | "tax" | "team" | "text" | "text-align-center" | "text-align-left" | "text-align-right" | "text-block" | "text-bold" | "text-color" | "text-font" | "text-font-list" | "text-grammar" | "text-in-columns" | "text-in-rows" | "text-indent" | "text-indent-remove" | "text-italic" | "text-quote" | "text-title" | "text-underline" | "text-with-image" | "theme" | "theme-edit" | "theme-store" | "theme-template" | "three-d-environment" | "thumbs-down" | "thumbs-up" | "tip-jar" | "toggle-off" | "toggle-on" | "transaction" | "transaction-fee-add" | "transaction-fee-dollar" | "transaction-fee-euro" | "transaction-fee-pound" | "transaction-fee-rupee" | "transaction-fee-yen" | "transfer" | "transfer-in" | "transfer-internal" | "transfer-out" | "truck" | "undo" | "unknown-device" | "unlock" | "upload" | "variant-list" | "video" | "video-list" | "view" | "viewport-narrow" | "viewport-short" | "viewport-tall" | "viewport-wide" | "wallet" | "wand" | "watch" | "wifi" | "work" | "work-list" | "wrench" | "x" | "x-circle" | "x-circle-filled"**

  The type of icon to be displayed in the Button.

- **inlineSize**

  **"auto" | "fill" | "fit-content"**

  **Default: 'auto'**

  The displayed inline width of the Button.

  - `auto`: the size of the button depends on the surface and context.
  - `fill`: the button will takes up 100% of the available inline size.
  - `fit-content`: the button will take up the minimum inline-size required to fit its content.

- **interestFor**

  **string**

  Sets the element the [interestFor](https://open-ui.org/components/interest-invokers.explainer/#the-pitch-in-code) should act on when this clickable is activated.

- **loading**

  **boolean**

  **Default: false**

  Replaces content with a loading indicator while a background action is being performed.

  This also disables the Button.

- **target**

  **"auto" | AnyString | "\_blank" | "\_self" | "\_parent" | "\_top"**

  **Default: 'auto'**

  Specifies where to display the linked URL.

- **tone**

  **"critical" | "auto" | "neutral"**

  **Default: 'auto'**

  Sets the tone of the Button based on the intention of the information being conveyed.

- **type**

  **"button" | "reset" | "submit"**

  **Default: 'button'**

  The behavior of the Button.

  - `submit`: Used to indicate the component acts as a submit button, meaning it submits the closest form.
  - `button`: Used to indicate the component acts as a button, meaning it has no default action.
  - `reset`: Used to indicate the component acts as a reset button, meaning it resets the closest form (returning fields to their default values).

  This property is ignored if the component supports `href` or `commandFor`/`command` and one of them is set.

- **variant**

  **"auto" | "primary" | "secondary" | "tertiary"**

  **Default: 'auto' - the variant is automatically determined by the Button's context**

  Changes the visual appearance of the Button.

### AnyString

Prevents widening string literal types in a union to \`string\`.

```ts
string & {}
```

## Events

Learn more about [registering events](https://shopify.dev/docs/api/app-home/using-polaris-components#event-handling).

- **blur**

  **CallbackEventListener\<TTagName> | null**

- **click**

  **CallbackEventListener\<TTagName> | null**

- **focus**

  **CallbackEventListener\<TTagName> | null**

### CallbackEventListener

```ts
(EventListener & {
      (event: CallbackEvent<T>): void;
    }) | null
```

### CallbackEvent

```ts
Event & {
  currentTarget: HTMLElementTagNameMap[T];
}
```

## Slots

- **children**

  **HTMLElement**

  The content of the Button.

Examples

### Examples

- #### Code

  ##### jsx

  ```jsx
  <>
    <s-button variant="primary">Add Product</s-button>
    <s-button variant="secondary">Save Theme</s-button>
  </>
  ```

  ##### html

  ```html
  <s-button variant="primary">Add Product</s-button>
  <s-button variant="secondary">Save Theme</s-button>
  ```

- #### Basic usage

  ##### Description

  Demonstrates a simple button with default styling, automatically determining its visual variant and using a clear, action-oriented label.

  ##### jsx

  ```jsx
  <s-button>Save</s-button>
  ```

  ##### html

  ```html
  <s-button>Save</s-button>
  ```

- #### Variants

  ##### Description

  Showcases different button variants with varying visual emphasis, helping merchants understand action priorities through distinct styling.

  ##### jsx

  ```jsx
  <s-stack direction="inline" gap="base">
    <s-button variant="primary">Primary</s-button>
    <s-button variant="secondary">Secondary</s-button>
    <s-button variant="tertiary">Tertiary</s-button>
    <s-button variant="auto">Auto</s-button>
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack direction="inline" gap="base">
    <s-button variant="primary">Primary</s-button>
    <s-button variant="secondary">Secondary</s-button>
    <s-button variant="tertiary">Tertiary</s-button>
    <s-button variant="auto">Auto</s-button>
  </s-stack>
  ```

- #### Tones

  ##### Description

  Illustrates button tones that signal the semantic importance and potential impact of different actions through color and styling.

  ##### jsx

  ```jsx
  <s-stack direction="inline" gap="base">
    <s-button tone="critical">Delete</s-button>
    <s-button tone="neutral">Save draft</s-button>
    <s-button>Continue</s-button>
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack direction="inline" gap="base">
    <s-button tone="critical">Delete</s-button>
    <s-button tone="neutral">Save draft</s-button>
    <s-button>Continue</s-button>
  </s-stack>
  ```

- #### With icon

  ##### Description

  Showcases a button that combines a descriptive text label with an intuitive icon, enhancing visual communication of the action.

  ##### jsx

  ```jsx
  <s-button icon="plus">Add product</s-button>
  ```

  ##### html

  ```html
  <s-button icon="plus">Add product</s-button>
  ```

- #### Icon-only button

  ##### Description

  Demonstrates an icon-only button with an accessibility label, providing a compact interface that remains screen reader friendly.

  ##### jsx

  ```jsx
  <s-button icon="plus" accessibilityLabel="Add product" />
  ```

  ##### html

  ```html
  <s-button icon="plus" accessibilityLabel="Add product"></s-button>
  ```

- #### Loading state

  ##### Description

  Illustrates buttons in various loading states, providing visual feedback during asynchronous operations.

  ##### jsx

  ```jsx
  <s-stack direction="inline" gap="base">
    <s-button loading variant="primary">
      Saving product...
    </s-button>
    <s-button loading variant="secondary">
      Updating 247 variants...
    </s-button>
    <s-button loading tone="neutral">
      Processing shipment...
    </s-button>
  </s-stack>
  ```

  ##### html

  ```html
  <!-- Product save operation -->
  <s-button loading variant="primary">Saving product...</s-button>

  <!-- Bulk inventory update -->
  <s-button loading variant="secondary">Updating 247 variants...</s-button>

  <!-- Order fulfillment -->
  <s-button loading tone="neutral">Processing shipment...</s-button>
  ```

- #### Form states

  ##### Description

  Demonstrates buttons in different interaction states, showing how to represent disabled controls and submit actions within forms.

  ##### jsx

  ```jsx
  <s-stack direction="inline" gap="base">
    <s-button disabled>Save draft</s-button>
    <s-button type="submit" variant="primary">
      Save product
    </s-button>
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack direction="inline" gap="base">
    <s-button disabled>Save draft</s-button>
    <s-button type="submit" variant="primary">Save product</s-button>
  </s-stack>
  ```

- #### Link buttons

  ##### Description

  Showcases buttons that act as hyperlinks, supporting navigation to different pages, external resources, and file downloads.

  ##### jsx

  ```jsx
  <s-stack direction="inline" gap="base">
    <s-button href="javascript:void(0)">View products</s-button>
    <s-button href="javascript:void(0)" target="_blank">
      Help docs
    </s-button>
    <s-button href="javascript:void(0)" download="sales-report.csv">
      Export data
    </s-button>
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack direction="inline" gap="base">
    <s-button href="javascript:void(0)">View products</s-button>
    <s-button href="javascript:void(0)" target="_blank">Help docs</s-button>
    <s-button href="javascript:void(0)" download="sales-report.csv">
      Export data
    </s-button>
  </s-stack>
  ```

- #### Form submission buttons

  ##### Description

  Demonstrates a button group with carefully aligned actions, showing how to create a clear visual hierarchy for form submission and cancellation.

  ##### jsx

  ```jsx
  <s-stack direction="inline" gap="base" justifyContent="end">
    <s-button variant="secondary">Cancel</s-button>
    <s-button variant="primary" type="submit">
      Save product
    </s-button>
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack direction="inline" gap="base" justifyContent="end">
    <s-button variant="secondary">Cancel</s-button>
    <s-button variant="primary" type="submit">Save product</s-button>
  </s-stack>
  ```

- #### Delete confirmation

  ##### Description

  Illustrates a button pair for destructive actions, using a critical tone to emphasize the potentially irreversible nature of the operation.

  ##### jsx

  ```jsx
  <s-stack direction="inline" gap="base">
    <s-button variant="secondary">Cancel</s-button>
    <s-button variant="primary" tone="critical">
      Delete variant
    </s-button>
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack direction="inline" gap="base">
    <s-button variant="secondary">Cancel</s-button>
    <s-button variant="primary" tone="critical">Delete variant</s-button>
  </s-stack>
  ```

- #### Navigation button with icon

  ##### Description

  Showcases a navigation button with an icon, enabling quick access to different sections of the interface.

  ##### jsx

  ```jsx
  <s-button href="javascript:void(0)" icon="order">
    View orders
  </s-button>
  ```

  ##### html

  ```html
  <!-- Button that navigates using Shopify's navigation system. Use shopify:navigate for navigation. Refer to [handling navigation events](/docs/api/app-home?accordionItem=getting-started-existing-remix-application) for implementation details and framework-specific examples. -->
  <s-button href="javascript:void(0)" icon="order">View orders</s-button>
  ```

- #### Button group for bulk operations

  ##### Description

  Demonstrates a button group for executing operations on multiple selected items.

  ##### jsx

  ```jsx
  <s-stack direction="inline" gap="base">
    <s-button variant="secondary">Export selected</s-button>
    <s-button variant="primary" tone="critical">
      Delete selected
    </s-button>
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack direction="inline" gap="base">
    <s-button variant="secondary">Export selected</s-button>
    <s-button variant="primary" tone="critical">Delete selected</s-button>
  </s-stack>
  ```

- #### Icon-only buttons with labels

  ##### Description

  Showcases a set of compact, icon-only buttons with accessibility labels, perfect for creating dense interfaces.

  ##### jsx

  ```jsx
  <s-stack direction="inline" gap="base">
    <s-button
      icon="duplicate"
      variant="tertiary"
      accessibilityLabel="Duplicate product"
     />
    <s-button
      icon="view"
      variant="tertiary"
      accessibilityLabel="Preview product"
     />
    <s-button
      icon="menu-horizontal"
      variant="tertiary"
      accessibilityLabel="More actions"
     />
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack direction="inline" gap="base">
    <s-button
      icon="duplicate"
      variant="tertiary"
      accessibilityLabel="Duplicate product"
    ></s-button>
    <s-button
      icon="view"
      variant="tertiary"
      accessibilityLabel="Preview product"
    ></s-button>
    <s-button
      icon="menu-horizontal"
      variant="tertiary"
      accessibilityLabel="More actions"
    ></s-button>
  </s-stack>
  ```

- #### Component interactions

  ##### Description

  Demonstrates advanced button capabilities using \`command\`, \`commandFor\`, and \`interestFor\` properties to enable dynamic component communication.

  ##### jsx

  ```jsx
  <s-stack direction="inline" gap="base">
    {/* Button that navigates using Shopify's navigation system. Use shopify:navigate for navigation. Refer to [handling navigation events](/docs/api/app-home?accordionItem=getting-started-existing-remix-application) for implementation details and framework-specific examples. */}
    <s-button href="javascript:void(0)">Edit details</s-button>

    {/* Button that expresses interest in specific data */}
    <s-button interestFor="selected-products">Bulk edit</s-button>
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack direction="inline" gap="base">
    <!-- Button that navigates using Shopify's navigation system. Use shopify:navigate for navigation. Refer to [handling navigation events](/docs/api/app-home?accordionItem=getting-started-existing-remix-application) for implementation details and framework-specific examples. -->
    <s-button href="javascript:void(0)">Edit details</s-button>

    <!-- Button that expresses interest in specific data -->
    <s-button interestFor="selected-products">Bulk edit</s-button>
  </s-stack>
  ```

## Useful for

- Taking primary actions like saving or creating resources
- Taking secondary actions like canceling forms or filtering results
- Triggering destructive operations like deletion or disconnection
- Accessing supplementary actions via tertiary buttons

## Best practices

- Be clearly and accurately labeled with strong, actionable verbs
- Use established button tones appropriately (e.g., critical tone only for actions that are difficult to undo)
- Prioritize the most important actions to avoid confusion
- Be positioned in consistent locations in the interface
- Use buttons for actions and links for navigation

## Content guidelines

- Use simple, concise verbs (e.g., "Save", "Edit", "Add tags")
- Write in sentence case
- Avoid unnecessary words and articles (e.g., "a", "an", "the")
- Don't use punctuation
- For icon-only buttons, use `accessibilityLabel` to describe the action

</page>

<page>
---
title: ButtonGroup
description: Displays multiple buttons in a layout.
api_name: app-home
source_url:
  html: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/actions/buttongroup
  md: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/actions/buttongroup.md
---

# Button​Group

Displays multiple buttons in a layout.

## Properties

- **accessibilityLabel**

  **string**

  Label for the button group that describes the content of the group for screen reader users to understand what's included.

- **gap**

  **"base" | "none"**

  **Default: 'base'**

  The gap between elements.

## Slots

- **children**

  **HTMLElement**

  The content of the ButtonGroup.

- **primary-action**

  **HTMLElement**

  The primary action button for the group. Accepts a single Button element with a `variant` of `primary`. Cannot be used when gap="none".

- **secondary-actions**

  **HTMLElement**

  Secondary action buttons for the group. Accepts Button or PressButton elements with a `variant` of `secondary` or `auto`.

Examples

### Examples

- #### Code

  ##### jsx

  ```jsx
  <s-button-group>
    <s-button slot="primary-action">Save</s-button>
    <s-button slot="secondary-actions">Cancel</s-button>
  </s-button-group>
  ```

  ##### html

  ```html
  <s-button-group>
    <s-button slot="primary-action">Save</s-button>
    <s-button slot="secondary-actions">Cancel</s-button>
  </s-button-group>
  ```

- #### Basic usage

  ##### Description

  Standard button group with cancel and primary action for form workflows.

  ##### jsx

  ```jsx
  <s-button-group>
    <s-button slot="secondary-actions">Cancel</s-button>
    <s-button slot="primary-action" variant="primary">
      Save
    </s-button>
  </s-button-group>
  ```

  ##### html

  ```html
  <s-button-group>
    <s-button slot="secondary-actions">Cancel</s-button>
    <s-button slot="primary-action" variant="primary">Save</s-button>
  </s-button-group>
  ```

- #### Bulk action buttons

  ##### Description

  Action buttons for selected items with destructive option.

  ##### jsx

  ```jsx
  <s-button-group>
    <s-button slot="secondary-actions">Archive</s-button>
    <s-button slot="secondary-actions">Export</s-button>
    <s-button slot="secondary-actions" tone="critical">
      Delete
    </s-button>
  </s-button-group>
  ```

  ##### html

  ```html
  <s-button-group>
    <s-button slot="secondary-actions">Archive</s-button>
    <s-button slot="secondary-actions">Export</s-button>
    <s-button slot="secondary-actions" tone="critical">Delete</s-button>
  </s-button-group>
  ```

- #### Form action buttons

  ##### Description

  Typical form completion actions with clear visual hierarchy.

  ##### jsx

  ```jsx
  <s-button-group>
    <s-button slot="secondary-actions">Cancel</s-button>
    <s-button slot="primary-action" variant="primary">
      Save product
    </s-button>
  </s-button-group>
  ```

  ##### html

  ```html
  <s-button-group>
    <s-button slot="secondary-actions">Cancel</s-button>
    <s-button slot="primary-action" variant="primary">Save product</s-button>
  </s-button-group>
  ```

- #### Buttons with icons

  ##### Description

  Icon-labeled buttons for common actions.

  ##### jsx

  ```jsx
  <s-button-group>
    <s-button slot="secondary-actions" icon="duplicate">
      Duplicate
    </s-button>
    <s-button slot="secondary-actions" icon="archive">
      Archive
    </s-button>
    <s-button slot="secondary-actions" icon="delete" tone="critical">
      Delete
    </s-button>
  </s-button-group>
  ```

  ##### html

  ```html
  <s-button-group>
    <s-button slot="secondary-actions" icon="duplicate">Duplicate</s-button>
    <s-button slot="secondary-actions" icon="archive">Archive</s-button>
    <s-button slot="secondary-actions" icon="delete" tone="critical">
      Delete
    </s-button>
  </s-button-group>
  ```

- #### Segmented appearance

  ##### Description

  Tightly grouped buttons for view switching and filter options.

  ##### jsx

  ```jsx
  <s-button-group gap="none">
    <s-button slot="secondary-actions">Day</s-button>
    <s-button slot="secondary-actions">Week</s-button>
    <s-button slot="secondary-actions">Month</s-button>
  </s-button-group>
  ```

  ##### html

  ```html
  <s-button-group gap="none">
    <s-button slot="secondary-actions">Day</s-button>
    <s-button slot="secondary-actions">Week</s-button>
    <s-button slot="secondary-actions">Month</s-button>
  </s-button-group>
  ```

- #### Destructive actions pattern

  ##### Description

  Confirmation dialog style with cancel option and destructive action.

  ##### jsx

  ```jsx
  <s-button-group>
    <s-button slot="secondary-actions">Cancel</s-button>
    <s-button slot="secondary-actions" tone="critical">
      Delete product
    </s-button>
  </s-button-group>
  ```

  ##### html

  ```html
  <s-button-group>
    <s-button slot="secondary-actions">Cancel</s-button>
    <s-button slot="secondary-actions" tone="critical">Delete product</s-button>
  </s-button-group>
  ```

## Useful for

- Accessing related actions in a consistent layout
- Making secondary actions visible alongside primary actions

## Best practices

- Group together related calls to action
- Avoid too many actions that may cause uncertainty
- Consider how buttons will work on small screens

</page>

<page>
---
title: Clickable
description: >-
  A generic interactive container component that provides a flexible alternative
  for custom interactive elements not achievable with existing components like
  Button or Link. Use it to apply specific styling such as backgrounds, padding,
  or borders.
api_name: app-home
source_url:
  html: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/actions/clickable
  md: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/actions/clickable.md
---

# Clickable

A generic interactive container component that provides a flexible alternative for custom interactive elements not achievable with existing components like Button or Link. Use it to apply specific styling such as backgrounds, padding, or borders.

## Properties

- **accessibilityLabel**

  **string**

  A label that describes the purpose or contents of the element. When set, it will be announced to users using assistive technologies and will provide them with more context.

  Only use this when the element's content is not enough context for users using assistive technologies.

- **accessibilityRole**

  **AccessibilityRole**

  **Default: 'generic'**

  Sets the semantic meaning of the component’s content. When set, the role will be used by assistive technologies to help users navigate the page.

- **accessibilityVisibility**

  **"visible" | "hidden" | "exclusive"**

  **Default: 'visible'**

  Changes the visibility of the element.

  - `visible`: the element is visible to all users.
  - `hidden`: the element is removed from the accessibility tree but remains visible.
  - `exclusive`: the element is visually hidden but remains in the accessibility tree.

- **background**

  **BackgroundColorKeyword**

  **Default: 'transparent'**

  Adjust the background of the component.

- **blockSize**

  **SizeUnitsOrAuto**

  **Default: 'auto'**

  Adjust the [block size](https://developer.mozilla.org/en-US/docs/Web/CSS/block-size).

- **border**

  **BorderShorthand**

  **Default: 'none' - equivalent to \`none base auto\`.**

  Set the border via the shorthand property.

  This can be a size, optionally followed by a color, optionally followed by a style.

  If the color is not specified, it will be `base`.

  If the style is not specified, it will be `auto`.

  Values can be overridden by `borderWidth`, `borderStyle`, and `borderColor`.

- **borderColor**

  **"" | ColorKeyword**

  **Default: '' - meaning no override**

  Adjust the color of the border.

- **borderRadius**

  **MaybeAllValuesShorthandProperty\<BoxBorderRadii>**

  **Default: 'none'**

  Adjust the radius of the border.

- **borderStyle**

  **"" | MaybeAllValuesShorthandProperty\<BoxBorderStyles>**

  **Default: '' - meaning no override**

  Adjust the style of the border.

- **borderWidth**

  **"" | MaybeAllValuesShorthandProperty<"small" | "small-100" | "base" | "large" | "large-100" | "none">**

  **Default: '' - meaning no override**

  Adjust the width of the border.

- **command**

  **'--auto' | '--show' | '--hide' | '--toggle'**

  **Default: '--auto'**

  Sets the action the [command](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/button#command) should take when this clickable is activated.

  See the documentation of particular components for the actions they support.

  - `--auto`: a default action for the target component.
  - `--show`: shows the target component.
  - `--hide`: hides the target component.
  - `--toggle`: toggles the target component.

- **commandFor**

  **string**

  Sets the element the [commandFor](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/button#commandfor) should act on when this clickable is activated.

- **disabled**

  **boolean**

  Disables the clickable, meaning it cannot be clicked or receive focus.

  In this state, onClick will not fire. If the click event originates from a child element, the event will immediately stop propagating from this element.

  However, items within the clickable can still receive focus and be interacted with.

  This has no impact on the visual state by default, but developers are encouraged to style the clickable accordingly.

- **display**

  **MaybeResponsive<"auto" | "none">**

  **Default: 'auto'**

  Sets the outer [display](https://developer.mozilla.org/en-US/docs/Web/CSS/display) type of the component. The outer type sets a component's participation in [flow layout](https://developer.mozilla.org/en-US/docs/Web/CSS/CSS_flow_layout).

  - `auto` the component's initial value. The actual value depends on the component and context.
  - `none` hides the component from display and removes it from the accessibility tree, making it invisible to screen readers.

- **download**

  **string**

  Causes the browser to treat the linked URL as a download with the string being the file name. Download only works for same-origin URLs or the `blob:` and `data:` schemes.

- **href**

  **string**

  The URL to link to.

  - If set, it will navigate to the location specified by `href` after executing the `click` event.
  - If a `commandFor` is set, the `command` will be executed instead of the navigation.

- **inlineSize**

  **SizeUnitsOrAuto**

  **Default: 'auto'**

  Adjust the [inline size](https://developer.mozilla.org/en-US/docs/Web/CSS/inline-size).

- **interestFor**

  **string**

  Sets the element the [interestFor](https://open-ui.org/components/interest-invokers.explainer/#the-pitch-in-code) should act on when this clickable is activated.

- **loading**

  **boolean**

  Disables the clickable, and indicates to assistive technology that the loading is in progress.

  This also disables the clickable.

- **maxBlockSize**

  **SizeUnitsOrNone**

  **Default: 'none'**

  Adjust the [maximum block size](https://developer.mozilla.org/en-US/docs/Web/CSS/max-block-size).

- **maxInlineSize**

  **SizeUnitsOrNone**

  **Default: 'none'**

  Adjust the [maximum inline size](https://developer.mozilla.org/en-US/docs/Web/CSS/max-inline-size).

- **minBlockSize**

  **SizeUnits**

  **Default: '0'**

  Adjust the [minimum block size](https://developer.mozilla.org/en-US/docs/Web/CSS/min-block-size).

- **minInlineSize**

  **SizeUnits**

  **Default: '0'**

  Adjust the [minimum inline size](https://developer.mozilla.org/en-US/docs/Web/CSS/min-inline-size).

- **overflow**

  **"visible" | "hidden"**

  **Default: 'visible'**

  Sets the overflow behavior of the element.

  - `hidden`: clips the content when it is larger than the element’s container. The element will not be scrollable and the users will not be able to access the clipped content by dragging or using a scroll wheel on a mouse.
  - `visible`: the content that extends beyond the element’s container is visible.

- **padding**

  **MaybeResponsive\<MaybeAllValuesShorthandProperty\<PaddingKeyword>>**

  **Default: 'none'**

  Adjust the padding of all edges.

  [1-to-4-value syntax](https://developer.mozilla.org/en-US/docs/Web/CSS/Shorthand_properties#edges_of_a_box) is supported. Note that, contrary to the CSS, it uses flow-relative values and the order is:

  - 4 values: `block-start inline-end block-end inline-start`
  - 3 values: `block-start inline block-end`
  - 2 values: `block inline`

  For example:

  - `large` means block-start, inline-end, block-end and inline-start paddings are `large`.
  - `large none` means block-start and block-end paddings are `large`, inline-start and inline-end paddings are `none`.
  - `large none large` means block-start padding is `large`, inline-end padding is `none`, block-end padding is `large` and inline-start padding is `none`.
  - `large none large small` means block-start padding is `large`, inline-end padding is `none`, block-end padding is `large` and inline-start padding is `small`.

  A padding value of `auto` will use the default padding for the closest container that has had its usual padding removed.

  `padding` also accepts a [responsive value](https://shopify.dev/docs/api/app-home/using-polaris-components#responsive-values) string with the supported PaddingKeyword as a query value.

- **paddingBlock**

  **MaybeResponsive<"" | MaybeTwoValuesShorthandProperty\<PaddingKeyword>>**

  **Default: '' - meaning no override**

  Adjust the block-padding.

  - `large none` means block-start padding is `large`, block-end padding is `none`.

  This overrides the block value of `padding`.

  `paddingBlock` also accepts a [responsive value](https://shopify.dev/docs/api/app-home/using-polaris-components#responsive-values) string with the supported PaddingKeyword as a query value.

- **paddingBlockEnd**

  **MaybeResponsive<"" | PaddingKeyword>**

  **Default: '' - meaning no override**

  Adjust the block-end padding.

  This overrides the block-end value of `paddingBlock`.

  `paddingBlockEnd` also accepts a [responsive value](https://shopify.dev/docs/api/app-home/using-polaris-components#responsive-values) string with the supported PaddingKeyword as a query value.

- **paddingBlockStart**

  **MaybeResponsive<"" | PaddingKeyword>**

  **Default: '' - meaning no override**

  Adjust the block-start padding.

  This overrides the block-start value of `paddingBlock`.

  `paddingBlockStart` also accepts a [responsive value](https://shopify.dev/docs/api/app-home/using-polaris-components#responsive-values) string with the supported PaddingKeyword as a query value.

- **paddingInline**

  **MaybeResponsive<"" | MaybeTwoValuesShorthandProperty\<PaddingKeyword>>**

  **Default: '' - meaning no override**

  Adjust the inline padding.

  - `large none` means inline-start padding is `large`, inline-end padding is `none`.

  This overrides the inline value of `padding`.

  `paddingInline` also accepts a [responsive value](https://shopify.dev/docs/api/app-home/using-polaris-components#responsive-values) string with the supported PaddingKeyword as a query value.

- **paddingInlineEnd**

  **MaybeResponsive<"" | PaddingKeyword>**

  **Default: '' - meaning no override**

  Adjust the inline-end padding.

  This overrides the inline-end value of `paddingInline`.

  `paddingInlineEnd` also accepts a [responsive value](https://shopify.dev/docs/api/app-home/using-polaris-components#responsive-values) string with the supported PaddingKeyword as a query value.

- **paddingInlineStart**

  **MaybeResponsive<"" | PaddingKeyword>**

  **Default: '' - meaning no override**

  Adjust the inline-start padding.

  This overrides the inline-start value of `paddingInline`.

  `paddingInlineStart` also accepts a [responsive value](https://shopify.dev/docs/api/app-home/using-polaris-components#responsive-values) string with the supported PaddingKeyword as a query value.

- **target**

  **"auto" | AnyString | "\_blank" | "\_self" | "\_parent" | "\_top"**

  **Default: 'auto'**

  Specifies where to display the linked URL.

- **type**

  **"button" | "reset" | "submit"**

  **Default: 'button'**

  The behavior of the Button.

  - `submit`: Used to indicate the component acts as a submit button, meaning it submits the closest form.
  - `button`: Used to indicate the component acts as a button, meaning it has no default action.
  - `reset`: Used to indicate the component acts as a reset button, meaning it resets the closest form (returning fields to their default values).

  This property is ignored if the component supports `href` or `commandFor`/`command` and one of them is set.

### AccessibilityRole

```ts
'main' | 'header' | 'footer' | 'section' | 'region' | 'aside' | 'navigation' | 'ordered-list' | 'list-item' | 'list-item-separator' | 'unordered-list' | 'separator' | 'status' | 'alert' | 'generic' | 'presentation' | 'none'
```

### BackgroundColorKeyword

```ts
'transparent' | ColorKeyword
```

### ColorKeyword

```ts
'subdued' | 'base' | 'strong'
```

### SizeUnitsOrAuto

```ts
SizeUnits | 'auto'
```

### SizeUnits

```ts
`${number}px` | `${number}%` | `0`
```

### BorderShorthand

Represents a shorthand for defining a border. It can be a combination of size, optionally followed by color, optionally followed by style.

```ts
BorderSizeKeyword | `${BorderSizeKeyword} ${ColorKeyword}` | `${BorderSizeKeyword} ${ColorKeyword} ${BorderStyleKeyword}`
```

### BorderSizeKeyword

```ts
SizeKeyword | 'none'
```

### SizeKeyword

```ts
'small-500' | 'small-400' | 'small-300' | 'small-200' | 'small-100' | 'small' | 'base' | 'large' | 'large-100' | 'large-200' | 'large-300' | 'large-400' | 'large-500'
```

### BorderStyleKeyword

```ts
'none' | 'solid' | 'dashed' | 'dotted' | 'auto'
```

### MaybeAllValuesShorthandProperty

```ts
T | `${T} ${T}` | `${T} ${T} ${T}` | `${T} ${T} ${T} ${T}`
```

### BoxBorderRadii

```ts
'small' | 'small-200' | 'small-100' | 'base' | 'large' | 'large-100' | 'large-200' | 'none'
```

### BoxBorderStyles

```ts
'auto' | 'none' | 'solid' | 'dashed'
```

### MaybeResponsive

```ts
T | `@container${string}`
```

### SizeUnitsOrNone

```ts
SizeUnits | 'none'
```

### PaddingKeyword

```ts
SizeKeyword | 'none'
```

### MaybeTwoValuesShorthandProperty

```ts
T | `${T} ${T}`
```

### AnyString

Prevents widening string literal types in a union to \`string\`.

```ts
string & {}
```

## Events

Learn more about [registering events](https://shopify.dev/docs/api/app-home/using-polaris-components#event-handling).

- **blur**

  **CallbackEventListener\<typeof tagName> | null**

- **click**

  **CallbackEventListener\<typeof tagName> | null**

- **focus**

  **CallbackEventListener\<typeof tagName> | null**

### CallbackEventListener

```ts
(EventListener & {
      (event: CallbackEvent<T>): void;
    }) | null
```

### CallbackEvent

```ts
Event & {
  currentTarget: HTMLElementTagNameMap[T];
}
```

## Slots

- **children**

  **HTMLElement**

  The content of the Clickable.

Examples

### Examples

- #### Code

  ##### jsx

  ```jsx
  <>
    <s-clickable padding="base">Create Store</s-clickable>

    <s-clickable
      border="base"
      padding="base"
      background="subdued"
      borderRadius="base"
    >
      View Shipping Settings
    </s-clickable>
  </>
  ```

  ##### html

  ```html
  <s-clickable padding="base">Create Store</s-clickable>

  <s-clickable
    border="base"
    padding="base"
    background="subdued"
    borderRadius="base"
  >
    View Shipping Settings
  </s-clickable>
  ```

- #### Basic Button Usage

  ##### Description

  A simple clickable button with a base border and padding, demonstrating the default button behavior of the Clickable component.

  ##### jsx

  ```jsx
  <s-clickable border="base" padding="base">
    Click me
  </s-clickable>
  ```

  ##### html

  ```html
  <s-clickable border="base" padding="base">Click me</s-clickable>
  ```

- #### Link Mode

  ##### Description

  Demonstrates the Clickable component's ability to function as a link, opening the specified URL in a new browser tab when clicked.

  ##### jsx

  ```jsx
  <s-clickable href="javascript:void(0)" target="_blank">
    Visit Shopify
  </s-clickable>
  ```

  ##### html

  ```html
  <s-clickable href="javascript:void(0)" target="_blank">
    Visit Shopify
  </s-clickable>
  ```

- #### Form Submit Button

  ##### Description

  A disabled submit button that can be used within a form, showing the component's form integration capabilities and disabled state.

  ##### jsx

  ```jsx
  <s-clickable type="submit" disabled border="base" padding="base">
    Save changes
  </s-clickable>
  ```

  ##### html

  ```html
  <s-clickable type="submit" disabled border="base" padding="base">
    Save changes
  </s-clickable>
  ```

- #### Section with Clickable Action

  ##### Description

  Illustrates how the Clickable component can be integrated into a section layout to provide an interactive action button.

  ##### jsx

  ```jsx
  <s-box padding="large-400" background="base" borderRadius="small-200">
    <s-stack gap="large-300">
      <s-heading>Product settings</s-heading>
      <s-text>Configure your product inventory and pricing settings.</s-text>
      <s-clickable background="base" padding="base" borderRadius="small">
        <s-text type="strong">Configure settings</s-text>
      </s-clickable>
    </s-stack>
  </s-box>
  ```

  ##### html

  ```html
  <s-box padding="large-400" background="base" borderRadius="small-200">
    <s-stack gap="large-300">
      <s-heading>Product settings</s-heading>
      <s-text>Configure your product inventory and pricing settings.</s-text>
      <s-clickable background="base" padding="base" borderRadius="small">
        <s-text type="strong">Configure settings</s-text>
      </s-clickable>
    </s-stack>
  </s-box>
  ```

- #### Accessibility with ARIA Attributes

  ##### Description

  Demonstrates the use of an accessibility label to provide context for screen readers, enhancing the component's usability for users with assistive technologies.

  ##### jsx

  ```jsx
  <s-clickable
    accessibilityLabel="Delete product winter collection jacket"
    background="base"
    padding="base"
    borderRadius="small"
  >
    <s-text>Delete</s-text>
  </s-clickable>
  ```

  ##### html

  ```html
  <s-clickable
    accessibilityLabel="Delete product winter collection jacket"
    background="base"
    padding="base"
    borderRadius="small"
  >
    <s-text>Delete</s-text>
  </s-clickable>
  ```

- #### Disabled Link with ARIA

  ##### Description

  Shows a disabled link with an accessibility label, explaining the unavailability of a feature to users of assistive technologies.

  ##### jsx

  ```jsx
  <s-clickable
    href="javascript:void(0)"
    disabled
    accessibilityLabel="This link is currently unavailable"
  >
    <s-text>Unavailable feature</s-text>
  </s-clickable>
  ```

  ##### html

  ```html
  <s-clickable
    href="javascript:void(0)"
    disabled
    accessibilityLabel="This link is currently unavailable"
  >
    <s-text>Unavailable feature</s-text>
  </s-clickable>
  ```

</page>

<page>
---
title: ClickableChip
description: >-
  Interactive button used to categorize or highlight content attributes. They
  are often displayed near the content they classify, enhancing discoverability
  by allowing users to identify items with similar properties.
api_name: app-home
source_url:
  html: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/actions/clickablechip
  md: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/actions/clickablechip.md
---

# Clickable​Chip

Interactive button used to categorize or highlight content attributes. They are often displayed near the content they classify, enhancing discoverability by allowing users to identify items with similar properties.

## Properties

- **accessibilityLabel**

  **string**

  A label that describes the purpose or contents of the Chip. It will be read to users using assistive technologies such as screen readers.

- **color**

  **ColorKeyword**

  **Default: 'base'**

  Modify the color to be more or less intense.

- **command**

  **'--auto' | '--show' | '--hide' | '--toggle'**

  **Default: '--auto'**

  Sets the action the [command](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/button#command) should take when this clickable is activated.

  See the documentation of particular components for the actions they support.

  - `--auto`: a default action for the target component.
  - `--show`: shows the target component.
  - `--hide`: hides the target component.
  - `--toggle`: toggles the target component.

- **commandFor**

  **string**

  Sets the element the [commandFor](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/button#commandfor) should act on when this clickable is activated.

- **disabled**

  **boolean**

  **Default: false**

  Disables the chip, disallowing any interaction.

- **hidden**

  **boolean**

  **Default: false**

  Determines whether the chip is hidden.

  If this property is being set on each framework render (as in 'controlled' usage), and the chip is `removable`, ensure you update app state for this property when the `remove` event fires.

  If the chip is not `removable`, it can still be hidden by setting this property.

- **href**

  **string**

  The URL to link to.

  - If set, it will navigate to the location specified by `href` after executing the `click` event.
  - If a `commandFor` is set, the `command` will be executed instead of the navigation.

- **interestFor**

  **string**

  Sets the element the [interestFor](https://open-ui.org/components/interest-invokers.explainer/#the-pitch-in-code) should act on when this clickable is activated.

- **removable**

  **boolean**

  **Default: false**

  Whether the chip is removable.

### ColorKeyword

```ts
'subdued' | 'base' | 'strong'
```

## Events

Learn more about [registering events](https://shopify.dev/docs/api/app-home/using-polaris-components#event-handling).

- **afterhide**

  **CallbackEventListener\<typeof tagName> | null**

- **click**

  **CallbackEventListener\<typeof tagName> | null**

- **remove**

  **CallbackEventListener\<typeof tagName> | null**

### CallbackEventListener

```ts
(EventListener & {
      (event: CallbackEvent<T>): void;
    }) | null
```

### CallbackEvent

```ts
Event & {
  currentTarget: HTMLElementTagNameMap[T];
}
```

## Slots

- **children**

  **HTMLElement**

  The content of the clickable chip.

- **graphic**

  **HTMLElement**

  The graphic to display in the clickable chip.

  Only accepts `Icon` components.

Examples

### Examples

- #### Code

  ##### jsx

  ```jsx
  <s-clickable-chip>Clickable chip</s-clickable-chip>
  ```

  ##### html

  ```html
  <s-clickable-chip>Clickable chip</s-clickable-chip>
  ```

- #### Basic Usage

  ##### Description

  Demonstrates a simple clickable chip with a base color and interactive functionality.

  ##### jsx

  ```jsx
  <s-stack direction="inline" gap="base">
    <s-clickable-chip color="base" accessibilityLabel="Filter by active products">
      Active
    </s-clickable-chip>
    <s-clickable-chip
      color="subdued"
      accessibilityLabel="Filter by draft products"
    >
      Draft
    </s-clickable-chip>
    <s-clickable-chip
      color="strong"
      accessibilityLabel="Filter by archived products"
    >
      Archived
    </s-clickable-chip>
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack direction="inline" gap="base">
    <s-clickable-chip color="base" accessibilityLabel="Filter by active products">
      Active
    </s-clickable-chip>
    <s-clickable-chip
      color="subdued"
      accessibilityLabel="Filter by draft products"
    >
      Draft
    </s-clickable-chip>
    <s-clickable-chip
      color="strong"
      accessibilityLabel="Filter by archived products"
    >
      Archived
    </s-clickable-chip>
  </s-stack>
  ```

- #### With Icon and Remove Button

  ##### Description

  Showcases a strong-colored clickable chip with a check circle icon and a removable state.

  ##### jsx

  ```jsx
  <s-clickable-chip
    color="strong"
    accessibilityLabel="Remove status filter"
    removable
  >
    <s-icon slot="graphic" type="check-circle" />
    In progress
  </s-clickable-chip>
  ```

  ##### html

  ```html
  <s-clickable-chip
    color="strong"
    accessibilityLabel="Remove status filter"
    removable
  >
    <s-icon slot="graphic" type="check-circle"></s-icon>
    In progress
  </s-clickable-chip>
  ```

- #### As a Link

  ##### Description

  Demonstrates a subdued clickable chip configured as a link with a product icon.

  ##### jsx

  ```jsx
  <s-clickable-chip
    color="subdued"
    href="javascript:void(0)"
    accessibilityLabel="View T-shirt product"
  >
    <s-icon slot="graphic" type="product" />
    T-shirt
  </s-clickable-chip>
  ```

  ##### html

  ```html
  <s-clickable-chip
    color="subdued"
    href="javascript:void(0)"
    accessibilityLabel="View T-shirt product"
  >
    <s-icon slot="graphic" type="product"></s-icon>
    T-shirt
  </s-clickable-chip>
  ```

- #### Disabled State

  ##### Description

  Illustrates a clickable chip in a disabled state, preventing interaction while displaying an inactive status.

  ##### jsx

  ```jsx
  <s-clickable-chip
    color="base"
    disabled
    accessibilityLabel="Status filter (disabled)"
  >
    Inactive
  </s-clickable-chip>
  ```

  ##### html

  ```html
  <s-clickable-chip
    color="base"
    disabled
    accessibilityLabel="Status filter (disabled)"
  >
    Inactive
  </s-clickable-chip>
  ```

- #### Multiple Chips with Proper Spacing

  ##### Description

  Demonstrates how multiple clickable chips with different colors, icons, and states can be arranged using an inline stack for consistent layout and spacing.

  ##### jsx

  ```jsx
  <s-stack direction="inline" gap="base">
    <s-clickable-chip accessibilityLabel="Filter by status">
      Active
    </s-clickable-chip>
    <s-clickable-chip
      color="strong"
      accessibilityLabel="Remove status filter"
      removable
    >
      <s-icon slot="graphic" type="check-circle" />
      In progress
    </s-clickable-chip>
    <s-clickable-chip color="subdued" accessibilityLabel="Filter by category">
      <s-icon slot="graphic" type="filter" />
      Category
    </s-clickable-chip>
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack direction="inline" gap="base">
    <s-clickable-chip accessibilityLabel="Filter by status">
      Active
    </s-clickable-chip>
    <s-clickable-chip
      color="strong"
      accessibilityLabel="Remove status filter"
      removable
    >
      <s-icon slot="graphic" type="check-circle"></s-icon>
      In progress
    </s-clickable-chip>
    <s-clickable-chip color="subdued" accessibilityLabel="Filter by category">
      <s-icon slot="graphic" type="filter"></s-icon>
      Category
    </s-clickable-chip>
  </s-stack>
  ```

## Useful for

- Creating interactive filters or tags that can be clicked or removed
- Navigating to related content when configured as a link
- Allowing merchants to dismiss or remove applied filters or selections

## Best practices

- Use for interactive chips that merchants can click or dismiss
- Use Chip component instead for static, non-interactive indicators
- Keep labels short to avoid truncation
- Use color variants to indicate importance (subdued, base, strong)
- Add icons to provide visual context

</page>

<page>
---
title: Link
description: >-
  Makes text interactive, allowing users to navigate to other pages or perform
  specific actions. Supports standard URLs, custom protocols, and navigation
  within Shopify or app pages.
api_name: app-home
source_url:
  html: 'https://shopify.dev/docs/api/app-home/polaris-web-components/actions/link'
  md: 'https://shopify.dev/docs/api/app-home/polaris-web-components/actions/link.md'
---

# Link

Makes text interactive, allowing users to navigate to other pages or perform specific actions. Supports standard URLs, custom protocols, and navigation within Shopify or app pages.

## Properties

- **accessibilityLabel**

  **string**

  A label that describes the purpose or contents of the Link. It will be read to users using assistive technologies such as screen readers.

  Use this when using only an icon or the content of the link is not enough context for users using assistive technologies.

- **command**

  **'--auto' | '--show' | '--hide' | '--toggle'**

  **Default: '--auto'**

  Sets the action the [command](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/button#command) should take when this clickable is activated.

  See the documentation of particular components for the actions they support.

  - `--auto`: a default action for the target component.
  - `--show`: shows the target component.
  - `--hide`: hides the target component.
  - `--toggle`: toggles the target component.

- **commandFor**

  **string**

  Sets the element the [commandFor](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/button#commandfor) should act on when this clickable is activated.

- **download**

  **string**

  Causes the browser to treat the linked URL as a download with the string being the file name. Download only works for same-origin URLs or the `blob:` and `data:` schemes.

- **href**

  **string**

  The URL to link to.

  - If set, it will navigate to the location specified by `href` after executing the `click` event.
  - If a `commandFor` is set, the `command` will be executed instead of the navigation.

- **interestFor**

  **string**

  Sets the element the [interestFor](https://open-ui.org/components/interest-invokers.explainer/#the-pitch-in-code) should act on when this clickable is activated.

- **lang**

  **string**

  Indicate the text language. Useful when the text is in a different language than the rest of the page. It will allow assistive technologies such as screen readers to invoke the correct pronunciation. [Reference of values](https://www.iana.org/assignments/language-subtag-registry/language-subtag-registry) ("subtag" label)

- **target**

  **"auto" | AnyString | "\_blank" | "\_self" | "\_parent" | "\_top"**

  **Default: 'auto'**

  Specifies where to display the linked URL.

- **tone**

  **"critical" | "auto" | "neutral"**

  **Default: 'auto'**

  Sets the tone of the Link, based on the intention of the information being conveyed.

### AnyString

Prevents widening string literal types in a union to \`string\`.

```ts
string & {}
```

## Events

Learn more about [registering events](https://shopify.dev/docs/api/app-home/using-polaris-components#event-handling).

- **click**

  **CallbackEventListener\<TTagName> | null**

### CallbackEventListener

```ts
(EventListener & {
      (event: CallbackEvent<T>): void;
    }) | null
```

### CallbackEvent

```ts
Event & {
  currentTarget: HTMLElementTagNameMap[T];
}
```

## Slots

- **children**

  **HTMLElement**

  The content of the Link.

Examples

### Examples

- #### Code

  ##### jsx

  ```jsx
  <s-link href="javascript:void(0)">fufilling orders</s-link>
  ```

  ##### html

  ```html
  <s-link href="#beep">fufilling orders</s-link>
  ```

- #### Basic Links in Paragraph

  ##### Description

  Links automatically inherit the tone from their surrounding paragraph context.

  ##### jsx

  ```jsx
  <s-paragraph>
    Your product catalog is empty. Start by <s-link href="javascript:void(0)">adding your first product</s-link> or <s-link href="javascript:void(0)">importing existing inventory</s-link>.
  </s-paragraph>
  ```

  ##### html

  ```html
  <s-paragraph>
    Your product catalog is empty. Start by <s-link href="javascript:void(0)">adding your first product</s-link> or <s-link href="javascript:void(0)">importing existing inventory</s-link>.
  </s-paragraph>
  ```

- #### Links in Subdued Paragraph

  ##### Description

  Demonstrates links within subdued paragraph, showing how links can be used in less prominent paragraph contexts for additional guidance or support.

  ##### jsx

  ```jsx
  <s-paragraph color="subdued">
    Need help setting up shipping rates? <s-link>View shipping guide</s-link> or <s-link>contact merchant support</s-link>.
  </s-paragraph>
  ```

  ##### html

  ```html
  <s-paragraph color="subdued">
    Need help setting up shipping rates? <s-link href="javascript:void(0)" target="_blank">View shipping guide</s-link> or <s-link href="javascript:void(0)">contact merchant support</s-link>.
  </s-paragraph>
  ```

- #### Critical Context Links

  ##### Description

  Illustrates how links can be used in critical or urgent text contexts, drawing attention to important actions that require immediate user intervention.

  ##### jsx

  ```jsx
  <s-paragraph tone="critical">
    Your store will be suspended in 24 hours due to unpaid balance. <s-link href="javascript:void(0)">Update payment method</s-link> to avoid service interruption.
  </s-paragraph>
  ```

  ##### html

  ```html
  <s-paragraph tone="critical">
    Your store will be suspended in 24 hours due to unpaid balance. <s-link href="javascript:void(0)">Update payment method</s-link> to avoid service interruption.
  </s-paragraph>
  ```

- #### Links with Auto Tone

  ##### Description

  Shows how links automatically adapt their tone to the surrounding text context, maintaining visual consistency while providing navigation.

  ##### jsx

  ```jsx
  <s-paragraph>
    You have 15 pending orders to fulfill. <s-link href="javascript:void(0)">Review unfulfilled orders</s-link> to keep customers happy.
  </s-paragraph>
  ```

  ##### html

  ```html
  <s-paragraph>
    You have 15 pending orders to fulfill. <s-link href="javascript:void(0)">Review unfulfilled orders</s-link> to keep customers happy.
  </s-paragraph>
  ```

- #### Links in Banner

  ##### Description

  Demonstrates how links can be integrated within banner components to highlight important information and provide direct action paths.

  ##### jsx

  ```jsx
  <s-banner tone="info">
    <s-paragraph>
      Black Friday campaigns are now available!  <s-link href="javascript:void(0)">Create your campaign</s-link>
    </s-paragraph>
  </s-banner>
  ```

  ##### html

  ```html
  <s-banner tone="info">
    <s-paragraph>
      Black Friday campaigns are now available!
      <s-link href="javascript:void(0)">Create your campaign</s-link>
    </s-paragraph>
  </s-banner>
  ```

- #### Links in Box Container

  ##### Description

  Illustrates using links within a box container to provide contextual navigation and additional information in a visually contained area.

  ##### jsx

  ```jsx
  <s-box padding="base" background="base" borderWidth="base" borderColor="base">
    <s-paragraph>
      Boost your store's conversion with professional themes. <s-link href="javascript:void(0)">Browse theme store</s-link> or <s-link href="javascript:void(0)">customize your current theme</s-link>.
    </s-paragraph>
  </s-box>
  ```

  ##### html

  ```html
  <s-box padding="base" background="base" borderWidth="base" borderColor="base">
    <s-paragraph>
      Boost your store's conversion with professional themes. <s-link href="javascript:void(0)">Browse theme store</s-link> or <s-link href="javascript:void(0)">customize your current theme</s-link>.
    </s-paragraph>
  </s-box>
  ```

- #### Links in Banner Context

  ##### Description

  Shows how links can be used within warning banners to provide immediate actions related to critical notifications.

  ##### jsx

  ```jsx
  <s-banner tone="warning">
    <s-paragraph>
      Your inventory for "Vintage t-shirt" is running low (3 remaining).  <s-link>Restock inventory</s-link>
    </s-paragraph>
  </s-banner>
  ```

  ##### html

  ```html
  <s-banner tone="warning">
    <s-paragraph>
      Your inventory for "Vintage t-shirt" is running low (3 remaining). <s-link>Restock inventory</s-link>
    </s-paragraph>
  </s-banner>
  ```

- #### Download Links

  ##### Description

  Demonstrates how to create links that trigger file downloads, useful for exporting data or providing downloadable resources.

  ##### jsx

  ```jsx
  <s-paragraph>
    Export your customer data for marketing analysis. <s-link href="javascript:void(0)" download="customer-export.csv">Download customer list</s-link>
  </s-paragraph>
  ```

  ##### html

  ```html
  <s-paragraph>
    Export your customer data for marketing analysis. <s-link href="javascript:void(0)" download="customer-export.csv">Download customer list</s-link>
  </s-paragraph>
  ```

- #### External Links

  ##### Description

  Illustrates linking to external resources with different targets, showing how to open links in new tabs and provide navigation to external documentation.

  ##### jsx

  ```jsx
  <s-box padding="base">
    <s-paragraph>
      Need help with policies? Read our <s-link href="javascript:void(0)" target="_blank">billing docs</s-link> or review the <s-link href="javascript:void(0)" target="_blank">terms of service</s-link>.
    </s-paragraph>
  </s-box>
  ```

  ##### html

  ```html
  <s-box padding="base">
    <s-paragraph>
      Need help with policies? Read our <s-link href="javascript:void(0)" target="_blank">billing docs</s-link> or review the <s-link href="javascript:void(0)" target="_blank">terms of service</s-link>.
    </s-paragraph>
  </s-box>
  ```

- #### Links with Language Attribute

  ##### Description

  Shows how to use the \`lang\` attribute to specify the language of a link, supporting internationalization and proper screen reader pronunciation.

  ##### jsx

  ```jsx
  <s-paragraph>
    Voir en français: <s-link lang="fr">Gérer les produits</s-link>
  </s-paragraph>
  ```

  ##### html

  ```html
  <s-paragraph>
    Voir en français: <s-link lang="fr">Gérer les produits</s-link>
  </s-paragraph>
  ```

- #### Links with Different Tones

  ##### Description

  Demonstrates how links can have different visual tones, including default, neutral, and critical, allowing for varied contextual styling.

  ##### jsx

  ```jsx
  <s-stack gap="base">
    <s-paragraph>
      Default tone: <s-link>View orders</s-link>
    </s-paragraph>

    <s-paragraph tone="success">
      Success tone: <s-link>Inventory help</s-link>
    </s-paragraph>

    <s-paragraph tone="critical">
      Critical tone: <s-link>Close store</s-link>
    </s-paragraph>

    <s-paragraph tone="warning">
      Warning tone: <s-link>Update billing info</s-link>
    </s-paragraph>

    <s-paragraph tone="info">
      Info tone: <s-link>Learn more about reports</s-link>
    </s-paragraph>

    <s-paragraph tone="caution">
      Caution tone: <s-link>View archived products</s-link>
    </s-paragraph>
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack gap="base">
    <s-paragraph>
      Default tone: <s-link>View orders</s-link>
    </s-paragraph>

    <s-paragraph tone="success">
      Neutral tone: <s-link>Inventory help</s-link>
    </s-paragraph>

    <s-paragraph tone="critical">
      Critical tone: <s-link>Close store</s-link>
    </s-paragraph>

    <s-paragraph tone="warning">
      Warning tone: <s-link>Update billing info</s-link>
    </s-paragraph>

    <s-paragraph tone="info">
      Info tone: <s-link>Learn more about reports</s-link>
    </s-paragraph>

    <s-paragraph tone="caution">
      Subdued tone: <s-link>View archived products</s-link>
    </s-paragraph>
  </s-stack>
  ```

## Best practices

- Use links for navigation and buttons for actions
- Use default links whenever possible to avoid disorienting merchants
- Open external links in a new tab only when merchants are performing a task or navigating outside the Shopify admin

</page>

<page>
---
title: Menu
description: Use Menu to display a list of actions that can be performed on a resource.
api_name: app-home
source_url:
  html: 'https://shopify.dev/docs/api/app-home/polaris-web-components/actions/menu'
  md: 'https://shopify.dev/docs/api/app-home/polaris-web-components/actions/menu.md'
---

# Menu

Use Menu to display a list of actions that can be performed on a resource.

## Properties

- **accessibilityLabel**

  **string**

  A label that describes the purpose or contents of the element. When set, it will be announced using assistive technologies and provide additional context.

## Slots

- **children**

  **HTMLElement**

  The Menu items.

  Only accepts `Button` and `Section` components.

Examples

### Examples

- #### Code

  ##### jsx

  ```jsx
  <>
    <s-button commandFor="customer-menu">Edit customer</s-button>

    <s-menu id="customer-menu" accessibilityLabel="Customer actions">
      <s-button icon="merge">Merge customer</s-button>
      <s-button icon="incoming">Request customer data</s-button>
      <s-button icon="delete" tone="critical">
        Delete customer
      </s-button>
    </s-menu>
  </>
  ```

  ##### html

  ```html
  <s-button commandFor="customer-menu">Edit customer</s-button>

  <s-menu id="customer-menu" accessibilityLabel="Customer actions">
    <s-button icon="merge">Merge customer</s-button>
    <s-button icon="incoming">Request customer data</s-button>
    <s-button icon="delete" tone="critical">Delete customer</s-button>
  </s-menu>
  ```

- #### Basic Menu

  ##### Description

  Demonstrates a simple menu with basic action buttons and shows how to link it to a trigger button.

  ##### jsx

  ```jsx
  <>
    <s-button commandFor="product-menu">Product actions</s-button>

    <s-menu id="product-menu" accessibilityLabel="Product actions">
      <s-button icon="edit">Edit product</s-button>
      <s-button icon="duplicate">Duplicate product</s-button>
      <s-button icon="archive">Archive product</s-button>
    </s-menu>
  </>
  ```

  ##### html

  ```html
  <s-button commandFor="product-menu">Product actions</s-button>

  <s-menu id="product-menu" accessibilityLabel="Product actions">
    <s-button icon="edit">Edit product</s-button>
    <s-button icon="duplicate">Duplicate product</s-button>
    <s-button icon="archive">Archive product</s-button>
  </s-menu>
  ```

- #### Menu with Icons

  ##### Description

  Illustrates a menu with icons for each action, providing visual context for different menu items and showing how to use the caret-down icon on the trigger button.

  ##### jsx

  ```jsx
  <>
    <s-button icon="caret-down" commandFor="actions-menu">
      More actions
    </s-button>

    <s-menu id="actions-menu" accessibilityLabel="Product actions menu">
      <s-button icon="edit">Edit product</s-button>
      <s-button icon="duplicate">Duplicate product</s-button>
      <s-button icon="archive">Archive product</s-button>
      <s-button icon="delete" tone="critical">
        Delete product
      </s-button>
    </s-menu>
  </>
  ```

  ##### html

  ```html
  <s-button icon="caret-down" commandFor="actions-menu">More actions</s-button>

  <s-menu id="actions-menu" accessibilityLabel="Product actions menu">
    <s-button icon="edit">Edit product</s-button>
    <s-button icon="duplicate">Duplicate product</s-button>
    <s-button icon="archive">Archive product</s-button>
    <s-button icon="delete" tone="critical">Delete product</s-button>
  </s-menu>
  ```

- #### Menu with Sections

  ##### Description

  Shows how to organize menu items into logical sections with headings, helping to group related actions and improve menu readability.

  ##### jsx

  ```jsx
  <>
    <s-button commandFor="organized-menu">Bulk actions</s-button>

    <s-menu id="organized-menu" accessibilityLabel="Organized menu">
      <s-section heading="Product actions">
        <s-button icon="edit">Edit selected</s-button>
        <s-button icon="duplicate">Duplicate selected</s-button>
        <s-button icon="archive">Archive selected</s-button>
      </s-section>
      <s-section heading="Export options">
        <s-button icon="export">Export as CSV</s-button>
        <s-button icon="print">Print barcodes</s-button>
      </s-section>
    </s-menu>
  </>
  ```

  ##### html

  ```html
  <s-button commandFor="organized-menu">Bulk actions</s-button>

  <s-menu id="organized-menu" accessibilityLabel="Organized menu">
    <s-section heading="Product actions">
      <s-button icon="edit">Edit selected</s-button>
      <s-button icon="duplicate">Duplicate selected</s-button>
      <s-button icon="archive">Archive selected</s-button>
    </s-section>
    <s-section heading="Export options">
      <s-button icon="export">Export as CSV</s-button>
      <s-button icon="print">Print barcodes</s-button>
    </s-section>
  </s-menu>
  ```

- #### Menu with Links and Disabled Items

  ##### Description

  Demonstrates a menu with a mix of link-based buttons, standard buttons, and a disabled button, showcasing the menu's flexibility in handling different interaction states.

  ##### jsx

  ```jsx
  <>
    <s-button commandFor="mixed-menu">Options</s-button>

    <s-menu id="mixed-menu" accessibilityLabel="Mixed menu options">
      <s-button href="javascript:void(0)" target="_blank">
        View product page
      </s-button>
      <s-button disabled>Unavailable action</s-button>
      <s-button download="sales-report.csv" href="/reports/sales-report.csv">
        Download report
      </s-button>
    </s-menu>
  </>
  ```

  ##### html

  ```html
  <s-button commandFor="mixed-menu">Options</s-button>

  <s-menu id="mixed-menu" accessibilityLabel="Mixed menu options">
    <s-button href="javascript:void(0)" target="_blank">
      View product page
    </s-button>
    <s-button disabled>Unavailable action</s-button>
    <s-button download href="javascript:void(0)">Download report</s-button>
  </s-menu>
  ```

- #### Actions menu with sections

  ##### Description

  Presents a comprehensive menu showing how to create sections with different action groups and include a critical action at the menu's root level.

  ##### jsx

  ```jsx
  <>
    <s-button commandFor="customer-menu">Edit customer</s-button>

    <s-menu id="customer-menu" accessibilityLabel="Customer actions">
      <s-section heading="Customer management">
        <s-button icon="edit">Edit customer</s-button>
        <s-button icon="email">Send email</s-button>
        <s-button icon="order">View orders</s-button>
      </s-section>
      <s-section heading="Account actions">
        <s-button icon="reset">Reset password</s-button>
        <s-button icon="lock">Disable account</s-button>
      </s-section>
      <s-button tone="critical" icon="delete">
        Delete customer
      </s-button>
    </s-menu>
  </>
  ```

  ##### html

  ```html
  <s-button commandFor="customer-menu">Edit customer</s-button>

  <s-menu id="customer-menu" accessibilityLabel="Customer actions">
    <s-section heading="Customer management">
      <s-button icon="edit">Edit customer</s-button>
      <s-button icon="email">Send email</s-button>
      <s-button icon="order">View orders</s-button>
    </s-section>
    <s-section heading="Account actions">
      <s-button icon="reset">Reset password</s-button>
      <s-button icon="lock">Disable account</s-button>
    </s-section>
    <s-button tone="critical" icon="delete">Delete customer</s-button>
  </s-menu>
  ```

- #### Menu with nested sections

  ##### Description

  Illustrates a complex menu with nested sections, demonstrating how to organize multiple related actions with icons.

  ##### jsx

  ```jsx
  <>
    <s-button icon="settings" commandFor="admin-settings">
      Settings
    </s-button>

    <s-menu id="admin-settings" accessibilityLabel="Settings menu">
      <s-section heading="Account">
        <s-button icon="profile">Profile settings</s-button>
        <s-button icon="lock">Security</s-button>
        <s-button>Billing information</s-button>
      </s-section>
      <s-section heading="Store">
        <s-button icon="store">Store settings</s-button>
        <s-button>Payment providers</s-button>
        <s-button icon="delivery">Shipping rates</s-button>
      </s-section>
      <s-button href="javascript:void(0)" icon="person-exit">Sign out</s-button>
    </s-menu>
  </>
  ```

  ##### html

  ```html
  <s-button icon="settings" commandFor="admin-settings">Settings</s-button>

  <s-menu id="admin-settings" accessibilityLabel="Settings menu">
    <s-section heading="Account">
      <s-button icon="profile">Profile settings</s-button>
      <s-button icon="lock">Security</s-button>
      <s-button>Billing information</s-button>
    </s-section>
    <s-section heading="Store">
      <s-button icon="store">Store settings</s-button>
      <s-button>Payment providers</s-button>
      <s-button icon="delivery">Shipping rates</s-button>
    </s-section>
    <s-button href="javascript:void(0)" icon="person-exit">Sign out</s-button>
  </s-menu>
  ```

## Useful for

- Presenting a set of actions or selectable options to merchants
- Creating dropdown menus with related actions
- Organizing actions into logical groupings using sections

## Best practices

- Use for secondary or less important actions since they're hidden until merchants open them
- Contain actions that are related to each other

## Content guidelines

- Each item should be clear and predictable
- Lead with a strong verb using the {verb}+{noun} format (e.g., "Buy shipping label", "Edit HTML")
- Avoid unnecessary words and articles like "the", "an", or "a"

</page>

<page>
---
title: Badge
description: >-
  Inform users about the status of an object or indicate that an action has been
  completed.
api_name: app-home
source_url:
  html: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/feedback-and-status-indicators/badge
  md: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/feedback-and-status-indicators/badge.md
---

# Badge

Inform users about the status of an object or indicate that an action has been completed.

## Properties

- **color**

  **"base" | "strong"**

  **Default: 'base'**

  Modify the color to be more or less intense.

- **icon**

  **"" | "replace" | "search" | "split" | "link" | "edit" | "product" | "variant" | "collection" | "select" | "info" | "incomplete" | "complete" | "color" | "money" | "order" | "code" | "adjust" | "affiliate" | "airplane" | "alert-bubble" | "alert-circle" | "alert-diamond" | "alert-location" | "alert-octagon" | "alert-octagon-filled" | "alert-triangle" | "alert-triangle-filled" | "align-horizontal-centers" | "app-extension" | "apps" | "archive" | "arrow-down" | "arrow-down-circle" | "arrow-down-right" | "arrow-left" | "arrow-left-circle" | "arrow-right" | "arrow-right-circle" | "arrow-up" | "arrow-up-circle" | "arrow-up-right" | "arrows-in-horizontal" | "arrows-out-horizontal" | "asterisk" | "attachment" | "automation" | "backspace" | "bag" | "bank" | "barcode" | "battery-low" | "bill" | "blank" | "blog" | "bolt" | "bolt-filled" | "book" | "book-open" | "bug" | "bullet" | "business-entity" | "button" | "button-press" | "calculator" | "calendar" | "calendar-check" | "calendar-compare" | "calendar-list" | "calendar-time" | "camera" | "camera-flip" | "caret-down" | "caret-left" | "caret-right" | "caret-up" | "cart" | "cart-abandoned" | "cart-discount" | "cart-down" | "cart-filled" | "cart-sale" | "cart-send" | "cart-up" | "cash-dollar" | "cash-euro" | "cash-pound" | "cash-rupee" | "cash-yen" | "catalog-product" | "categories" | "channels" | "channels-filled" | "chart-cohort" | "chart-donut" | "chart-funnel" | "chart-histogram-first" | "chart-histogram-first-last" | "chart-histogram-flat" | "chart-histogram-full" | "chart-histogram-growth" | "chart-histogram-last" | "chart-histogram-second-last" | "chart-horizontal" | "chart-line" | "chart-popular" | "chart-stacked" | "chart-vertical" | "chat" | "chat-new" | "chat-referral" | "check" | "check-circle" | "check-circle-filled" | "checkbox" | "chevron-down" | "chevron-down-circle" | "chevron-left" | "chevron-left-circle" | "chevron-right" | "chevron-right-circle" | "chevron-up" | "chevron-up-circle" | "circle" | "circle-dashed" | "clipboard" | "clipboard-check" | "clipboard-checklist" | "clock" | "clock-list" | "clock-revert" | "code-add" | "collection-featured" | "collection-list" | "collection-reference" | "color-none" | "compass" | "compose" | "confetti" | "connect" | "content" | "contract" | "corner-pill" | "corner-round" | "corner-square" | "credit-card" | "credit-card-cancel" | "credit-card-percent" | "credit-card-reader" | "credit-card-reader-chip" | "credit-card-reader-tap" | "credit-card-secure" | "credit-card-tap-chip" | "crop" | "currency-convert" | "cursor" | "cursor-banner" | "cursor-option" | "data-presentation" | "data-table" | "database" | "database-add" | "database-connect" | "delete" | "delivered" | "delivery" | "desktop" | "disabled" | "disabled-filled" | "discount" | "discount-add" | "discount-automatic" | "discount-code" | "discount-remove" | "dns-settings" | "dock-floating" | "dock-side" | "domain" | "domain-landing-page" | "domain-new" | "domain-redirect" | "download" | "drag-drop" | "drag-handle" | "drawer" | "duplicate" | "email" | "email-follow-up" | "email-newsletter" | "empty" | "enabled" | "enter" | "envelope" | "envelope-soft-pack" | "eraser" | "exchange" | "exit" | "export" | "external" | "eye-check-mark" | "eye-dropper" | "eye-dropper-list" | "eye-first" | "eyeglasses" | "fav" | "favicon" | "file" | "file-list" | "filter" | "filter-active" | "flag" | "flip-horizontal" | "flip-vertical" | "flower" | "folder" | "folder-add" | "folder-down" | "folder-remove" | "folder-up" | "food" | "foreground" | "forklift" | "forms" | "games" | "gauge" | "geolocation" | "gift" | "gift-card" | "git-branch" | "git-commit" | "git-repository" | "globe" | "globe-asia" | "globe-europe" | "globe-lines" | "globe-list" | "graduation-hat" | "grid" | "hashtag" | "hashtag-decimal" | "hashtag-list" | "heart" | "hide" | "hide-filled" | "home" | "home-filled" | "icons" | "identity-card" | "image" | "image-add" | "image-alt" | "image-explore" | "image-magic" | "image-none" | "image-with-text-overlay" | "images" | "import" | "in-progress" | "incentive" | "incoming" | "info-filled" | "inheritance" | "inventory" | "inventory-edit" | "inventory-list" | "inventory-transfer" | "inventory-updated" | "iq" | "key" | "keyboard" | "keyboard-filled" | "keyboard-hide" | "keypad" | "label-printer" | "language" | "language-translate" | "layout-block" | "layout-buy-button" | "layout-buy-button-horizontal" | "layout-buy-button-vertical" | "layout-column-1" | "layout-columns-2" | "layout-columns-3" | "layout-footer" | "layout-header" | "layout-logo-block" | "layout-popup" | "layout-rows-2" | "layout-section" | "layout-sidebar-left" | "layout-sidebar-right" | "lightbulb" | "link-list" | "list-bulleted" | "list-bulleted-filled" | "list-numbered" | "live" | "live-critical" | "live-none" | "location" | "location-none" | "lock" | "map" | "markets" | "markets-euro" | "markets-rupee" | "markets-yen" | "maximize" | "measurement-size" | "measurement-size-list" | "measurement-volume" | "measurement-volume-list" | "measurement-weight" | "measurement-weight-list" | "media-receiver" | "megaphone" | "mention" | "menu" | "menu-filled" | "menu-horizontal" | "menu-vertical" | "merge" | "metafields" | "metaobject" | "metaobject-list" | "metaobject-reference" | "microphone" | "microphone-muted" | "minimize" | "minus" | "minus-circle" | "mobile" | "money-none" | "money-split" | "moon" | "nature" | "note" | "note-add" | "notification" | "number-one" | "order-batches" | "order-draft" | "order-filled" | "order-first" | "order-fulfilled" | "order-repeat" | "order-unfulfilled" | "orders-status" | "organization" | "outdent" | "outgoing" | "package" | "package-cancel" | "package-fulfilled" | "package-on-hold" | "package-reassign" | "package-returned" | "page" | "page-add" | "page-attachment" | "page-clock" | "page-down" | "page-heart" | "page-list" | "page-reference" | "page-remove" | "page-report" | "page-up" | "pagination-end" | "pagination-start" | "paint-brush-flat" | "paint-brush-round" | "paper-check" | "partially-complete" | "passkey" | "paste" | "pause-circle" | "payment" | "payment-capture" | "payout" | "payout-dollar" | "payout-euro" | "payout-pound" | "payout-rupee" | "payout-yen" | "person" | "person-add" | "person-exit" | "person-filled" | "person-list" | "person-lock" | "person-remove" | "person-segment" | "personalized-text" | "phablet" | "phone" | "phone-down" | "phone-down-filled" | "phone-in" | "phone-out" | "pin" | "pin-remove" | "plan" | "play" | "play-circle" | "plus" | "plus-circle" | "plus-circle-down" | "plus-circle-filled" | "plus-circle-up" | "point-of-sale" | "point-of-sale-register" | "price-list" | "print" | "product-add" | "product-cost" | "product-filled" | "product-list" | "product-reference" | "product-remove" | "product-return" | "product-unavailable" | "profile" | "profile-filled" | "question-circle" | "question-circle-filled" | "radio-control" | "receipt" | "receipt-dollar" | "receipt-euro" | "receipt-folded" | "receipt-paid" | "receipt-pound" | "receipt-refund" | "receipt-rupee" | "receipt-yen" | "receivables" | "redo" | "referral-code" | "refresh" | "remove-background" | "reorder" | "replay" | "reset" | "return" | "reward" | "rocket" | "rotate-left" | "rotate-right" | "sandbox" | "save" | "savings" | "scan-qr-code" | "search-add" | "search-list" | "search-recent" | "search-resource" | "send" | "settings" | "share" | "shield-check-mark" | "shield-none" | "shield-pending" | "shield-person" | "shipping-label" | "shipping-label-cancel" | "shopcodes" | "slideshow" | "smiley-happy" | "smiley-joy" | "smiley-neutral" | "smiley-sad" | "social-ad" | "social-post" | "sort" | "sort-ascending" | "sort-descending" | "sound" | "sports" | "star" | "star-circle" | "star-filled" | "star-half" | "star-list" | "status" | "status-active" | "stop-circle" | "store" | "store-import" | "store-managed" | "store-online" | "sun" | "table" | "table-masonry" | "tablet" | "target" | "tax" | "team" | "text" | "text-align-center" | "text-align-left" | "text-align-right" | "text-block" | "text-bold" | "text-color" | "text-font" | "text-font-list" | "text-grammar" | "text-in-columns" | "text-in-rows" | "text-indent" | "text-indent-remove" | "text-italic" | "text-quote" | "text-title" | "text-underline" | "text-with-image" | "theme" | "theme-edit" | "theme-store" | "theme-template" | "three-d-environment" | "thumbs-down" | "thumbs-up" | "tip-jar" | "toggle-off" | "toggle-on" | "transaction" | "transaction-fee-add" | "transaction-fee-dollar" | "transaction-fee-euro" | "transaction-fee-pound" | "transaction-fee-rupee" | "transaction-fee-yen" | "transfer" | "transfer-in" | "transfer-internal" | "transfer-out" | "truck" | "undo" | "unknown-device" | "unlock" | "upload" | "variant-list" | "video" | "video-list" | "view" | "viewport-narrow" | "viewport-short" | "viewport-tall" | "viewport-wide" | "wallet" | "wand" | "watch" | "wifi" | "work" | "work-list" | "wrench" | "x" | "x-circle" | "x-circle-filled"**

  The type of icon to be displayed in the badge.

- **size**

  **"base" | "large" | "large-100"**

  **Default: 'base'**

  Adjusts the size.

- **tone**

  **"info" | "success" | "warning" | "critical" | "auto" | "neutral" | "caution"**

  **Default: 'auto'**

  Sets the tone of the Badge, based on the intention of the information being conveyed.

## Slots

- **children**

  **HTMLElement**

  The content of the Badge.

Examples

### Examples

- #### Code

  ##### jsx

  ```jsx
  <>
    <s-badge>Fulfilled</s-badge>
    <s-badge tone="info">Draft</s-badge>
    <s-badge tone="success">Active</s-badge>
    <s-badge tone="caution">Open</s-badge>
    <s-badge tone="warning">On hold</s-badge>
    <s-badge tone="critical">Action required</s-badge>
  </>
  ```

  ##### html

  ```html
  <s-badge>Fulfilled</s-badge>
  <s-badge tone="info">Draft</s-badge>
  <s-badge tone="success">Active</s-badge>
  <s-badge tone="caution">Open</s-badge>
  <s-badge tone="warning">On hold</s-badge>
  <s-badge tone="critical">Action required</s-badge>
  ```

- #### Order status badges

  ##### Description

  Demonstrates how different badge tones can visually represent various order fulfillment states, enabling merchants to quickly understand order progress at a glance.

  ##### jsx

  ```jsx
  <s-stack direction="inline" gap="base">
    <s-badge tone="success">Fulfilled</s-badge>
    <s-badge tone="warning">Partially fulfilled</s-badge>
    <s-badge tone="neutral">Unfulfilled</s-badge>
    <s-badge tone="critical">Cancelled</s-badge>
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack direction="inline" gap="base">
    <s-badge tone="success">Fulfilled</s-badge>
    <s-badge tone="warning">Partially fulfilled</s-badge>
    <s-badge tone="neutral">Unfulfilled</s-badge>
    <s-badge tone="critical">Cancelled</s-badge>
  </s-stack>
  ```

- #### Status indicators with icons

  ##### Description

  Showcases how badges can incorporate both tones and icons to provide contextual information across different merchant scenarios, such as product management, inventory tracking, and payment status.

  ##### jsx

  ```jsx
  <s-stack gap="base">
    {/* Product status */}
    <s-stack direction="inline" gap="base">
      <s-badge tone="success" icon="view">
        Active
      </s-badge>
      <s-badge tone="warning" icon="clock">
        Scheduled
      </s-badge>
      <s-badge tone="critical">Archived</s-badge>
    </s-stack>

    {/* Inventory status */}
    <s-stack direction="inline" gap="base">
      <s-badge tone="success">In stock</s-badge>
      <s-badge tone="warning" icon="alert-triangle">
        Low stock
      </s-badge>
      <s-badge tone="critical">Out of stock</s-badge>
    </s-stack>
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack gap="base">
    <!-- Product status -->
    <s-stack direction="inline" gap="base">
      <s-badge tone="success" icon="view">Active</s-badge>
      <s-badge tone="warning" icon="clock">Scheduled</s-badge>
      <s-badge tone="critical">Archived</s-badge>
    </s-stack>

    <!-- Inventory status -->
    <s-stack direction="inline" gap="base">
      <s-badge tone="success">In stock</s-badge>
      <s-badge tone="warning" icon="alert-triangle">Low stock</s-badge>
      <s-badge tone="critical">Out of stock</s-badge>
    </s-stack>
  </s-stack>
  ```

- #### Within table context

  ##### Description

  Illustrates how badges can be seamlessly integrated into table layouts to provide quick, visually distinct status indicators for individual table rows.

  ##### jsx

  ```jsx
  <s-table>
    <s-table-header-row>
      <s-table-header>Order</s-table-header>
      <s-table-header>Fulfillment</s-table-header>
      <s-table-header>Payment</s-table-header>
    </s-table-header-row>
    <s-table-body>
      <s-table-row>
        <s-table-cell>#1001</s-table-cell>
        <s-table-cell>
          <s-badge tone="success">Fulfilled</s-badge>
        </s-table-cell>
        <s-table-cell>
          <s-badge tone="success">Paid</s-badge>
        </s-table-cell>
      </s-table-row>
    </s-table-body>
  </s-table>
  ```

  ##### html

  ```html
  <s-table>
    <s-table-header-row>
      <s-table-header>Order</s-table-header>
      <s-table-header>Fulfillment</s-table-header>
      <s-table-header>Payment</s-table-header>
    </s-table-header-row>
    <s-table-body>
      <s-table-row>
        <s-table-cell>#1001</s-table-cell>
        <s-table-cell>
          <s-badge tone="success">Fulfilled</s-badge>
        </s-table-cell>
        <s-table-cell>
          <s-badge tone="success">Paid</s-badge>
        </s-table-cell>
      </s-table-row>
    </s-table-body>
  </s-table>
  ```

- #### Different sizes for emphasis

  ##### Description

  Demonstrates the two available badge sizes for creating visual hierarchy.

  ##### jsx

  ```jsx
  <s-stack direction="inline" gap="base">
    <s-badge size="base">New</s-badge>
    <s-badge size="large">Attention needed</s-badge>
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack direction="inline" gap="base">
    <s-badge size="base">New</s-badge>
    <s-badge size="large">Attention needed</s-badge>
  </s-stack>
  ```

## Useful for

- Communicating the state of an object
- Identifying objects that need attention or action
- Quickly scanning complex lists to find specific object states

## Best practices

- `base`: use in tables where many badges are displayed
- `large`: use when badge needs to stand out prominently
- Text truncates automatically, keep labels short to avoid truncation
- Badges are static indicators, not interactive or dismissible
- Use `critical` or `warning` tones for errors needing immediate attention
- Use consistent styles and icons for common statuses
- When using badges in line items, integrate them with the full content group rather than attaching only to the header
- Don't use badges for merchant-created information. Instead, use a Chip or ClickableChip

## Content guidelines

Badge labels should:

- Use 1-2 words maximum: `Fulfilled`, `Partially refunded`
- Always use past tense: `Refunded` not `Refund`

</page>

<page>
---
title: Banner
description: >-
  Highlights important information or required actions prominently within the
  interface. Use to communicate statuses, provide feedback, or draw attention to
  critical updates.
api_name: app-home
source_url:
  html: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/feedback-and-status-indicators/banner
  md: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/feedback-and-status-indicators/banner.md
---

# Banner

Highlights important information or required actions prominently within the interface. Use to communicate statuses, provide feedback, or draw attention to critical updates.

## Properties

- **dismissible**

  **boolean**

  **Default: false**

  Determines whether the close button of the banner is present.

  When the close button is pressed, the `dismiss` event will fire, then `hidden` will be true, any animation will complete, and the `afterhide` event will fire.

- **heading**

  **string**

  **Default: ''**

  The title of the banner.

- **hidden**

  **boolean**

  **Default: false**

  Determines whether the banner is hidden.

  If this property is being set on each framework render (as in 'controlled' usage), and the banner is `dismissible`, ensure you update app state for this property when the `dismiss` event fires.

  If the banner is not `dismissible`, it can still be hidden by setting this property.

- **tone**

  **"info" | "success" | "warning" | "critical" | "auto"**

  **Default: 'auto'**

  Sets the tone of the Banner, based on the intention of the information being conveyed.

  The banner is a live region and the type of status will be dictated by the Tone selected.

  - `critical` creates an [assertive live region](https://developer.mozilla.org/en-US/docs/Web/Accessibility/ARIA/Roles/alert_role) that is announced by screen readers immediately.
  - `neutral`, `info`, `success`, `warning` and `caution` creates an [informative live region](https://developer.mozilla.org/en-US/docs/Web/Accessibility/ARIA/Roles/status_role) that is announced by screen readers after the current message.

## Events

Learn more about [registering events](https://shopify.dev/docs/api/app-home/using-polaris-components#event-handling).

- **afterhide**

  **CallbackEventListener\<typeof tagName> | null**

- **dismiss**

  **CallbackEventListener\<typeof tagName> | null**

### CallbackEventListener

```ts
(EventListener & {
      (event: CallbackEvent<T>): void;
    }) | null
```

### CallbackEvent

```ts
Event & {
  currentTarget: HTMLElementTagNameMap[T];
}
```

## Slots

- **children**

  **HTMLElement**

  The content of the Banner.

- **secondary-actions**

  **HTMLElement**

  The secondary actions to display at the bottom of the Banner.

  Only Buttons with the `variant` of "secondary" or "auto" are permitted. A maximum of two `s-button` components are allowed.

Examples

### Examples

- #### Code

  ##### jsx

  ```jsx
  <s-banner heading="Order archived" tone="info" dismissible>
    This order was archived on March 7, 2017 at 3:12pm EDT.
  </s-banner>
  ```

  ##### html

  ```html
  <s-banner heading="Order archived" tone="info" dismissible>
    This order was archived on March 7, 2017 at 3:12pm EDT.
  </s-banner>
  ```

- #### Basic information banner

  ##### Description

  Demonstrates a simple informational banner used to communicate status updates or completed actions, providing clear and concise feedback to the user.

  ##### jsx

  ```jsx
  <s-banner heading="Order archived">
    This order was archived on March 7, 2017 at 3:12pm EDT.
  </s-banner>
  ```

  ##### html

  ```html
  <s-banner heading="Order archived">
    This order was archived on March 7, 2017 at 3:12pm EDT.
  </s-banner>
  ```

- #### Warning banner with specific actions

  ##### Description

  Illustrates a warning banner that highlights a potential issue and provides actionable buttons to help users resolve the problem quickly and effectively.

  ##### jsx

  ```jsx
  <s-banner heading="127 products missing shipping weights" tone="warning">
    Products without weights may show inaccurate shipping rates, leading to
    checkout abandonment.
    <s-button
      slot="secondary-actions"
      variant="secondary"
      href="/admin/products?filter=missing-weights"
    >
      Review products
    </s-button>
    <s-button
      slot="secondary-actions"
      variant="secondary"
      href="javascript:void(0)"
    >
      Setup guide
    </s-button>
  </s-banner>
  ```

  ##### html

  ```html
  <s-banner heading="127 products missing shipping weights" tone="warning">
    Products without weights may show inaccurate shipping rates, leading to
    checkout abandonment.
    <s-button
      slot="secondary-actions"
      variant="secondary"
      href="/admin/products?filter=missing-weights"
    >
      Review products
    </s-button>
    <s-button
      slot="secondary-actions"
      variant="secondary"
      href="javascript:void(0)"
    >
      Setup guide
    </s-button>
  </s-banner>
  ```

- #### Critical banner with clear next steps

  ##### Description

  Demonstrates an urgent banner design that signals a critical issue requiring immediate action, with clear and prominent secondary action buttons to guide users.

  ##### jsx

  ```jsx
  <s-banner heading="Order #1024 flagged for fraud review" tone="critical">
    This order shows multiple risk indicators and cannot be auto-fulfilled. Review
    required within 24 hours to prevent automatic cancellation.
    <s-button
      slot="secondary-actions"
      variant="secondary"
      href="/admin/orders/1024/risk"
    >
      Review order details
    </s-button>
    <s-button
      slot="secondary-actions"
      variant="secondary"
      href="/admin/settings/payments/fraud"
    >
      Adjust fraud settings
    </s-button>
  </s-banner>
  ```

  ##### html

  ```html
  <s-banner heading="Order #1024 flagged for fraud review" tone="critical">
    This order shows multiple risk indicators and cannot be auto-fulfilled. Review
    required within 24 hours to prevent automatic cancellation.
    <s-button
      slot="secondary-actions"
      variant="secondary"
      href="/admin/orders/1024/risk"
    >
      Review order details
    </s-button>
    <s-button
      slot="secondary-actions"
      variant="secondary"
      href="/admin/settings/payments/fraud"
    >
      Adjust fraud settings
    </s-button>
  </s-banner>
  ```

- #### Dismissible success banner

  ##### Description

  Success confirmation with dismiss option for completed operations.

  ##### jsx

  ```jsx
  <s-banner heading="Products imported" tone="success" dismissible={true}>
    Successfully imported 50 products to your store.
  </s-banner>
  ```

  ##### html

  ```html
  <s-banner heading="Products imported" tone="success" dismissible="true">
    Successfully imported 50 products to your store.
  </s-banner>
  ```

- #### Info banner with clear value proposition

  ##### Description

  Informational banner highlighting app updates with clear benefits and actions.

  ##### jsx

  ```jsx
  <s-banner heading="Point of Sale v2.4 available" tone="info" dismissible>
    New version includes faster checkout processing and inventory sync
    improvements.
    <s-button
      slot="secondary-actions"
      variant="secondary"
      href="/admin/apps/pos/changelog"
    >
      View changes
    </s-button>
    <s-button slot="secondary-actions" variant="secondary">
      Install update
    </s-button>
  </s-banner>
  ```

  ##### html

  ```html
  <s-banner heading="Point of Sale v2.4 available" tone="info" dismissible>
    New version includes faster checkout processing and inventory sync
    improvements.
    <s-button
      slot="secondary-actions"
      variant="secondary"
      href="/admin/apps/pos/changelog"
    >
      View changes
    </s-button>
    <s-button slot="secondary-actions" variant="secondary">
      Install update
    </s-button>
  </s-banner>
  ```

## Useful for

- Showing important information or changes
- Prompting merchants to take a specific action
- Displaying warnings, errors, or critical information
- Communicating persistent conditions that need attention

## Outside of a section

Banners placed outside of a section will display in their own card and should be located at the top of the page. They're useful for conveying messages that apply to the entire page or areas not visible within the viewport, such as validation errors further down the page.

## In a section

Banners placed inside a section will have styles applied contextually. They're useful for conveying messages relevant to a specific section or piece of content.

## Best practices

- Seeing these banners can be stressful, so use them sparingly to avoid overwhelming users.
- Focus on a single piece of information or required action to avoid overwhelming users.
- Make the message concise and scannable. Users shouldn’t need to spend a lot of time figuring out what they need to know and do.
- Info, Warning and Critical banners should contain a call to action and clear next steps. Users should know what to do after seeing the banner.
- Avoid banners that can't be dismissed unless the user is required to take action.

## Content guidelines

- Keep titles concise and clear
- Limit body content to 1-2 sentences where possible
- Use action-led buttons with strong verbs (e.g., "Activate Apple Pay" not "Try Apple Pay")
- Avoid unnecessary words and articles in button text
- For warning and critical banners, explain how to resolve the issue

</page>

<page>
---
title: Spinner
description: >-
  Displays an animated indicator showing users that content or actions are
  loading. Use to communicate ongoing processes, such as fetching data from a
  server. For loading states on buttons, use the “loading” property on the
  Button component instead.
api_name: app-home
source_url:
  html: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/feedback-and-status-indicators/spinner
  md: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/feedback-and-status-indicators/spinner.md
---

# Spinner

Displays an animated indicator showing users that content or actions are loading. Use to communicate ongoing processes, such as fetching data from a server. For loading states on buttons, use the “loading” property on the Button component instead.

## Properties

- **accessibilityLabel**

  **string**

  A label that describes the purpose of the progress. When set, it will be announced to users using assistive technologies and will provide them with more context. Providing an `accessibilityLabel` is recommended if there is no accompanying text describing that something is loading.

- **size**

  **"base" | "large" | "large-100"**

  **Default: 'base'**

Examples

### Examples

- #### Code

  ##### jsx

  ```jsx
  <s-spinner accessibilityLabel="Loading" size="large-100" />
  ```

  ##### html

  ```html
  <s-spinner accessibilityLabel="Loading" size="large-100"></s-spinner>
  ```

- #### Basic usage

  ##### Description

  Standard loading spinner with accessibility label for screen readers.

  ##### jsx

  ```jsx
  <s-spinner accessibilityLabel="Loading content" />
  ```

  ##### html

  ```html
  <s-spinner accessibilityLabel="Loading content"></s-spinner>
  ```

- #### Loading state in section

  ##### Description

  Centered loading indicator with text in a section layout for content loading states.

  ##### jsx

  ```jsx
  <s-stack alignItems="center" gap="base" padding="large">
    <s-spinner accessibilityLabel="Loading products" size="large" />
    <s-text>Loading products...</s-text>
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack alignItems="center" gap="base" padding="large">
    <s-spinner accessibilityLabel="Loading products" size="large"></s-spinner>
    <s-text>Loading products...</s-text>
  </s-stack>
  ```

- #### Inline loading with text

  ##### Description

  Compact inline loading indicator for form submissions and quick actions.

  ##### jsx

  ```jsx
  <s-stack direction="inline" alignItems="center" gap="small">
    <s-spinner accessibilityLabel="Saving" />
    <s-text>Saving...</s-text>
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack direction="inline" alignItems="center" gap="small">
    <s-spinner accessibilityLabel="Saving"></s-spinner>
    <s-text>Saving...</s-text>
  </s-stack>
  ```

## Best practices

- Use to notify merchants that their action is being processed
- Don't use for entire page loads

</page>

<page>
---
title: Checkbox
description: >-
  Give users a clear way to make selections, such as agreeing to terms or
  choosing multiple items from a list.
api_name: app-home
source_url:
  html: 'https://shopify.dev/docs/api/app-home/polaris-web-components/forms/checkbox'
  md: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/forms/checkbox.md
---

# Checkbox

Give users a clear way to make selections, such as agreeing to terms or choosing multiple items from a list.

## Properties

- **accessibilityLabel**

  **string**

  A label used for users using assistive technologies like screen readers. When set, any children or `label` supplied will not be announced. This can also be used to display a control without a visual label, while still providing context to users using screen readers.

- **checked**

  **boolean**

  **Default: false**

  Whether the control is active.

- **defaultChecked**

  **boolean**

  **Default: false**

  Whether the control is active by default.

- **defaultIndeterminate**

  **boolean**

  **Default: false**

- **details**

  **string**

  Additional text to provide context or guidance for the field. This text is displayed along with the field and its label to offer more information or instructions to the user.

  This will also be exposed to screen reader users.

- **disabled**

  **boolean**

  **Default: false**

  Disables the field, disallowing any interaction.

- **error**

  **string**

  Indicate an error to the user. The field will be given a specific stylistic treatment to communicate problems that have to be resolved immediately.

- **id**

  **string**

  A unique identifier for the element.

- **indeterminate**

  **boolean**

- **label**

  **string**

  Visual content to use as the control label.

- **name**

  **string**

  An identifier for the field that is unique within the nearest containing form.

- **required**

  **boolean**

  **Default: false**

  Whether the field needs a value. This requirement adds semantic value to the field, but it will not cause an error to appear automatically. If you want to present an error when this field is empty, you can do so with the `error` property.

- **value**

  **string**

  The value used in form data when the checkbox is checked.

## Events

Learn more about [registering events](https://shopify.dev/docs/api/app-home/using-polaris-components#event-handling).

- **change**

  **CallbackEventListener<'input'>**

- **input**

  **CallbackEventListener<'input'>**

### CallbackEventListener

```ts
(EventListener & {
      (event: CallbackEvent<T>): void;
    }) | null
```

### CallbackEvent

```ts
Event & {
  currentTarget: HTMLElementTagNameMap[T];
}
```

Examples

### Examples

- #### Code

  ##### jsx

  ```jsx
  <s-checkbox
    label="Require a confirmation step"
    details="Ensure all criteria are met before proceeding"
   />
  ```

  ##### html

  ```html
  <s-checkbox
    label="Require a confirmation step"
    details="Ensure all criteria are met before proceeding"
  ></s-checkbox>
  ```

- #### Indeterminate state

  ##### Description

  Checkbox in indeterminate state, commonly used for "select all" functionality when some items are selected.

  ##### jsx

  ```jsx
  const [selectedItems, setSelectedItems] = useState([]);
  const items = ['Item 1', 'Item 2', 'Item 3'];

  const toggleItem = (value, checked) => setSelectedItems(checked ? [...selectedItems, value] : selectedItems.filter(item => item !== value));
  const toggleAll = (checked) => setSelectedItems(checked ? items : []);
  const isSelected = (item) => selectedItems.includes(item);

  return (
    <s-stack gap="small">
      <s-checkbox
        label="Select all items"
        indeterminate={selectedItems.length !== 0 && selectedItems.length !== items.length}
        checked={selectedItems.length === items.length}
        onChange={e => toggleAll(e.currentTarget.checked)}
      />
      <s-divider />
      {items.map(i => (
        <s-checkbox key={i} label={i} checked={isSelected(i)} onChange={e => toggleItem(i, e.currentTarget.checked)} />
      ))}
    </s-stack>
  );
  ```

- #### Error state

  ##### Description

  Checkbox with validation error message for required form fields.

  ##### jsx

  ```jsx
  <s-checkbox
    label="I agree to the terms"
    error="You must accept the terms to continue"
  />
  ```

  ##### html

  ```html
  <s-checkbox
    label="I agree to the terms"
    error="You must accept the terms to continue"
  ></s-checkbox>
  ```

- #### Help text

  ##### Description

  Checkbox with descriptive details text to provide additional context about the option.

  ##### jsx

  ```jsx
  <s-checkbox
    label="Send order notifications"
    details="You'll receive emails when orders are placed, fulfilled, or cancelled"
   />
  ```

  ##### html

  ```html
  <s-checkbox
    label="Send order notifications"
    details="You'll receive emails when orders are placed, fulfilled, or cancelled"
  ></s-checkbox>
  ```

- #### Disabled state

  ##### Description

  Checkbox in disabled state with explanatory details about why it's unavailable.

  ##### jsx

  ```jsx
  <s-checkbox
    label="Advanced settings"
    disabled
    details="Contact your administrator to enable advanced settings"
   />
  ```

  ##### html

  ```html
  <s-checkbox
    label="Advanced settings"
    disabled
    details="Contact your administrator to enable advanced settings"
  ></s-checkbox>
  ```

- #### Settings group

  ##### Description

  Multiple checkboxes for different configuration options with helpful details.

  ##### jsx

  ```jsx
  <s-stack gap="base">
    <s-checkbox label="Show only published products" checked />
    <s-checkbox
      label="Enable inventory tracking"
      details="Track inventory levels and receive low stock alerts"
      checked
     />
    <s-checkbox
      label="View customer data"
      details="Allow staff to access customer contact information and order history"
     />
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack gap="base">
    <s-checkbox label="Show only published products" checked></s-checkbox>
    <s-checkbox
      label="Enable inventory tracking"
      details="Track inventory levels and receive low stock alerts"
      checked
    ></s-checkbox>
    <s-checkbox
      label="View customer data"
      details="Allow staff to access customer contact information and order history"
    ></s-checkbox>
  </s-stack>
  ```

- #### Checkbox validation

  ##### Description

  Interactive example showing required checkbox validation with dynamic error messages.

  ##### jsx

  ```jsx
  const [checked, setChecked] = useState(false);
  const errorMessage = 'You must accept the terms to continue';
  const [error, setError] = useState(errorMessage);

  return (
    <s-section>
      <s-stack gap="base" justifyContent="start">
        <s-text-field label="Name" />
        <s-checkbox
          label="I agree to the terms"
          checked={checked}
          error={error}
          onChange={(e) => {
            setChecked(e.currentTarget.checked);
            setError(e.currentTarget.checked ? '' : errorMessage);
          }}
        />
      </s-stack>
    </s-section>
  )
  ```

## Best practices

- Use ChoiceList when rendering multiple checkboxes to provide a consistent and accessible selection interface
- Work independently from each other
- Be framed positively (e.g., "Publish store" not "Hide store")
- Always have a label when used to activate or deactivate a setting
- Be listed in a logical order (alphabetical, numerical, time-based, etc.)

## Content guidelines

- Start each option with a capital letter
- Don't use commas or semicolons at the end of each line
- Use first person when asking merchants to agree to terms (e.g., "I agree to the Terms of Service")

</page>

<page>
---
title: ChoiceList
description: >-
  Present multiple options to users, allowing either single selections with
  radio buttons or multiple selections with checkboxes.
api_name: app-home
source_url:
  html: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/forms/choicelist
  md: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/forms/choicelist.md
---

# Choice​List

Present multiple options to users, allowing either single selections with radio buttons or multiple selections with checkboxes.

## Properties

- **details**

  **string**

  Additional text to provide context or guidance for the field. This text is displayed along with the field and its label to offer more information or instructions to the user.

  This will also be exposed to screen reader users.

- **disabled**

  **boolean**

  **Default: false**

  Disables the field, disallowing any interaction.

  `disabled` on any child choices is ignored when this is true.

- **error**

  **string**

  Indicate an error to the user. The field will be given a specific stylistic treatment to communicate problems that have to be resolved immediately.

- **label**

  **string**

  Content to use as the field label.

- **labelAccessibilityVisibility**

  **"visible" | "exclusive"**

  **Default: 'visible'**

  Changes the visibility of the component's label.

  - `visible`: the label is visible to all users.
  - `exclusive`: the label is visually hidden but remains in the accessibility tree.

- **multiple**

  **boolean**

  **Default: false**

  Whether multiple choices can be selected.

- **name**

  **string**

  An identifier for the field that is unique within the nearest containing form.

- **values**

  **string\[]**

  An array of the `value`s of the selected options.

  This is a convenience prop for setting the `selected` prop on child options.

## Events

Learn more about [registering events](https://shopify.dev/docs/api/app-home/using-polaris-components#event-handling).

- **change**

  **CallbackEventListener\<typeof tagName> | null**

- **input**

  **CallbackEventListener\<typeof tagName> | null**

### CallbackEventListener

```ts
(EventListener & {
      (event: CallbackEvent<T>): void;
    }) | null
```

### CallbackEvent

```ts
Event & {
  currentTarget: HTMLElementTagNameMap[T];
}
```

## Choice

Create options that let users select one or multiple items from a list of choices.

- **accessibilityLabel**

  **string**

  A label used for users using assistive technologies like screen readers. When set, any children or `label` supplied will not be announced. This can also be used to display a control without a visual label, while still providing context to users using screen readers.

- **defaultSelected**

  **boolean**

  **Default: false**

  Whether the control is active by default.

- **disabled**

  **boolean**

  **Default: false**

  Disables the control, disallowing any interaction.

- **selected**

  **boolean**

  **Default: false**

  Whether the control is active.

- **value**

  **string**

  The value used in form data when the control is checked.

## Slots

- **children**

  **HTMLElement**

  Content to use as the choice label.

  The label is produced by extracting and concatenating the text nodes from the provided content; any markup or element structure is ignored.

- **details**

  **HTMLElement**

  Additional text to provide context or guidance for the input.

  This text is displayed along with the input and its label to offer more information or instructions to the user.

- **secondary-content**

  **HTMLElement**

  Additional content to display below the choice label. Can include rich content like TextFields, Buttons, or other interactive components. Event handlers on React components are preserved.

Examples

### Examples

- #### Code

  ##### jsx

  ```jsx
  const handleChange = (event) => {
    console.log('Values: ', event.currentTarget.values)
  }

  return (
    <s-choice-list
      label="Company name"
      name="Company name"
      details="The company name will be displayed on the checkout page."
      onChange={handleChange}
    >
      <s-choice value="hidden">Hidden</s-choice>
      <s-choice value="optional">Optional</s-choice>
      <s-choice value="required">Required</s-choice>
    </s-choice-list>
  )
  ```

  ##### html

  ```html
  <script>
    const handleChange = (event) =>
      console.log('Values: ', event.currentTarget.values);
  </script>
  <s-choice-list
    label="Company name"
    name="Company name"
    details="The company name will be displayed on the checkout page."
    onChange="handleChange(event)"
  >
    <s-choice value="hidden">Hidden</s-choice>
    <s-choice value="optional">Optional</s-choice>
    <s-choice value="required">Required</s-choice>
  </s-choice-list>
  ```

- #### Basic usage

  ##### Description

  Demonstrates a basic ChoiceList with single selection, showing how to create a group of radio button choices.

  ##### jsx

  ```jsx
  <s-choice-list label="Product visibility" name="visibility">
    <s-choice value="hidden">Hidden</s-choice>
    <s-choice value="optional">Optional</s-choice>
    <s-choice value="required" selected>
      Required
    </s-choice>
  </s-choice-list>
  ```

  ##### html

  ```html
  <s-choice-list label="Product visibility" name="visibility">
    <s-choice value="hidden">Hidden</s-choice>
    <s-choice value="optional">Optional</s-choice>
    <s-choice value="required" selected>Required</s-choice>
  </s-choice-list>
  ```

- #### Multiple selections

  ##### Description

  Illustrates a ChoiceList with multiple selection enabled, allowing users to choose multiple options with additional descriptive details for each choice.

  ##### jsx

  ```jsx
  <s-choice-list label="Checkout options" name="checkout" multiple>
    <s-choice value="shipping" selected>
      Use the shipping address as the billing address by default
      <s-text slot="details">
        Reduces the number of fields required to check out. The billing address
        can still be edited.
      </s-text>
    </s-choice>
    <s-choice value="confirmation">
      Require a confirmation step
      <s-text slot="details">
        Customers must review their order details before purchasing.
      </s-text>
    </s-choice>
  </s-choice-list>
  ```

  ##### html

  ```html
  <s-choice-list label="Checkout options" name="checkout" multiple>
    <s-choice value="shipping" selected>
      Use the shipping address as the billing address by default
      <s-text slot="details">
        Reduces the number of fields required to check out. The billing address
        can still be edited.
      </s-text>
    </s-choice>
    <s-choice value="confirmation">
      Require a confirmation step
      <s-text slot="details">
        Customers must review their order details before purchasing.
      </s-text>
    </s-choice>
  </s-choice-list>
  ```

- #### With error state

  ##### Description

  Shows how to display an error message in a ChoiceList when an invalid selection is made or a validation constraint is not met.

  ##### jsx

  ```jsx
  <s-choice-list
    label="Product visibility"
    error="Please select an option"
  >
    <s-choice value="hidden">Hidden</s-choice>
    <s-choice value="optional">Optional</s-choice>
    <s-choice value="required">Required</s-choice>
  </s-choice-list>
  ```

  ##### html

  ```html
  <s-choice-list
    label="Product visibility"
    name="visibility"
    error="Product visibility cannot be hidden at this time"
  >
    <s-choice value="hidden">Hidden</s-choice>
    <s-choice value="optional">Optional</s-choice>
    <s-choice value="required" selected>Required</s-choice>
  </s-choice-list>
  ```

- #### Multiple choices with details

  ##### Description

  Showcases a multiple-selection ChoiceList with each option including detailed information.

  ##### jsx

  ```jsx
  <s-choice-list
    label="Available shipping methods"
    name="shipping-methods"
    multiple
  >
    <s-choice value="standard" selected>
      Standard shipping
      <s-text slot="details">5-7 business days delivery</s-text>
    </s-choice>
    <s-choice value="express" selected>
      Express shipping
      <s-text slot="details">2-3 business days delivery</s-text>
    </s-choice>
    <s-choice value="overnight">
      Overnight shipping
      <s-text slot="details">Next business day delivery</s-text>
    </s-choice>
  </s-choice-list>
  ```

  ##### html

  ```html
  <s-choice-list
    label="Available shipping methods"
    name="shipping-methods"
    multiple
  >
    <s-choice value="standard" selected>
      Standard shipping
      <s-text slot="details">5-7 business days delivery</s-text>
    </s-choice>
    <s-choice value="express" selected>
      Express shipping
      <s-text slot="details">2-3 business days delivery</s-text>
    </s-choice>
    <s-choice value="overnight">
      Overnight shipping
      <s-text slot="details">Next business day delivery</s-text>
    </s-choice>
  </s-choice-list>
  ```

- #### Choice list validation

  ##### Description

  Interactive example showing required choice validation with dynamic error messages.

  ##### jsx

  ```jsx
  const [error, setError] = useState('Please select at least one option');

  return (
    <s-section>
      <s-stack gap="base" justifyContent="start">
        <s-choice-list
          label="Product visibility"
          name="visibility"
          error={error}
          onChange={(e) => {
            setError(e.currentTarget.values.length > 0 ? '' : 'Please select at least one option');
          }}
        >
          <s-choice value="hidden">Hidden</s-choice>
          <s-choice value="optional">Optional</s-choice>
          <s-choice value="required">Required</s-choice>
        </s-choice-list>
      </s-stack>
    </s-section>
  )
  ```

## Best practices

- Include a title that tells merchants what to do or explains the available options
- Label options clearly based on what the option will do
- Avoid mutually exclusive options when allowing multiple selection

## Content guidelines

- Write titles and choices in sentence case
- End titles with a colon if they introduce the list
- Start each choice with a capital letter
- Don't use commas or semicolons at the end of lines

</page>

<page>
---
title: ColorField
description: Allow users to select a color with a color picker or as a text input.
api_name: app-home
source_url:
  html: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/forms/colorfield
  md: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/forms/colorfield.md
---

# Color​Field

Allow users to select a color with a color picker or as a text input.

## Properties

- **alpha**

  **boolean**

  **Default: false**

  Allow user to select an alpha value.

- **autocomplete**

  **"on" | "off"**

  **Default: 'on' for everything else**

  A hint as to the intended content of the field.

  When set to `on` (the default), this property indicates that the field should support autofill, but you do not have any more semantic information on the intended contents.

  When set to `off`, you are indicating that this field contains sensitive information, or contents that are never saved, like one-time codes.

  Alternatively, you can provide value which describes the specific data you would like to be entered into this field during autofill.

- **defaultValue**

  **string**

  The default value for the field.

- **details**

  **string**

  Additional text to provide context or guidance for the field. This text is displayed along with the field and its label to offer more information or instructions to the user.

  This will also be exposed to screen reader users.

- **disabled**

  **boolean**

  **Default: false**

  Disables the field, disallowing any interaction.

- **error**

  **string**

  Indicate an error to the user. The field will be given a specific stylistic treatment to communicate problems that have to be resolved immediately.

- **id**

  **string**

  A unique identifier for the element.

- **label**

  **string**

  Content to use as the field label.

- **labelAccessibilityVisibility**

  **"visible" | "exclusive"**

  **Default: 'visible'**

  Changes the visibility of the component's label.

  - `visible`: the label is visible to all users.
  - `exclusive`: the label is visually hidden but remains in the accessibility tree.

- **name**

  **string**

  An identifier for the field that is unique within the nearest containing form.

- **placeholder**

  **string**

  A short hint that describes the expected value of the field.

- **readOnly**

  **boolean**

  **Default: false**

  The field cannot be edited by the user. It is focusable will be announced by screen readers.

- **required**

  **boolean**

  **Default: false**

  Whether the field needs a value. This requirement adds semantic value to the field, but it will not cause an error to appear automatically. If you want to present an error when this field is empty, you can do so with the `error` property.

- **value**

  **string**

  The current value for the field. If omitted, the field will be empty.

  The current value for the field. If omitted, the field will be empty.

## Events

Learn more about [registering events](https://shopify.dev/docs/api/app-home/using-polaris-components#event-handling).

- **blur**

  **CallbackEventListener<'input'>**

- **change**

  **CallbackEventListener<'input'>**

- **focus**

  **CallbackEventListener<'input'>**

- **input**

  **CallbackEventListener<'input'>**

### CallbackEventListener

```ts
(EventListener & {
      (event: CallbackEvent<T>): void;
    }) | null
```

### CallbackEvent

```ts
Event & {
  currentTarget: HTMLElementTagNameMap[T];
}
```

Examples

### Examples

- #### Code

  ##### jsx

  ```jsx
  <s-color-field placeholder="Select a color" value="#FF0000" />
  ```

  ##### html

  ```html
  <s-color-field placeholder="Select a color" value="#FF0000"></s-color-field>
  ```

- #### Basic Usage

  ##### Description

  Standard color input field with hex value.

  ##### jsx

  ```jsx
  <s-stack gap="base">
    <s-color-field label="Brand color" name="brandColor" value="#FF0000" />
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack gap="base">
    <s-color-field label="Brand color" name="color" value="#FF0000"></s-color-field>
  </s-stack>
  ```

- #### Required

  ##### Description

  Required color field ensuring essential color values are provided.

  ##### jsx

  ```jsx
  <s-stack gap="base">
    <s-color-field label="Brand color" value="#FF0000" required />
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack gap="base">
    <s-color-field label="Brand color" value="#FF0000" required></s-color-field>
  </s-stack>
  ```

- #### With Alpha Transparency

  ##### Description

  Color field supporting alpha channel for transparency control.

  ##### jsx

  ```jsx
  <s-stack gap="base">
    <s-color-field
      label="Background color"
      value="rgba(255, 0, 0, 0.5)"
      alpha
     />
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack gap="base">
    <s-color-field
      label="Background color"
      value="rgba(255, 0, 0, 0.5)"
      alpha
    ></s-color-field>
  </s-stack>
  ```

- #### With Error State

  ##### Description

  Color field with validation error for invalid color format inputs.

  ##### jsx

  ```jsx
  <s-stack gap="base">
    <s-color-field
      label="Brand color"
      name="brandColor"
      value="#invalid"
      error="Please enter a valid color format (hex, rgb, or rgba)"
      required
    ></s-color-field>
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack gap="base">
    <s-color-field
      label="Brand color"
      name="brandColor"
      value="#invalid"
      error="Please enter a valid color format (hex, rgb, or rgba)"
      required
    ></s-color-field>
  </s-stack>
  ```

- #### With Help Text

  ##### Description

  Color field with contextual details providing additional guidance.

  ##### jsx

  ```jsx
  <s-stack gap="base">
    <s-color-field
      label="Primary color"
      value="#1a73e8"
      details="Main brand color used for buttons and links"
     />
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack gap="base">
    <s-color-field
      label="Primary color"
      value="#1a73e8"
      details="Main brand color used for buttons and links"
    ></s-color-field>
  </s-stack>
  ```

- #### With Placeholder

  ##### Description

  Color field demonstrating how to use a placeholder to guide user input for color selection.

  ##### jsx

  ```jsx
  <s-stack gap="base">
    <s-color-field
      label="Theme color"
      placeholder="Enter a hex color (e.g., #FF0000)"
      value="#000000"
     />
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack gap="base">
    <s-color-field
      label="Theme color"
      placeholder="Enter a hex color (e.g., #FF0000)"
      value="#000000"
    ></s-color-field>
  </s-stack>
  ```

- #### Read Only State

  ##### Description

  Color field in a read-only mode, preventing user modifications to the color value.

  ##### jsx

  ```jsx
  <s-stack gap="base">
    <s-color-field label="System color" name="color" value="#1a73e8" readOnly />
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack gap="base">
    <s-color-field label="System color" name="color" value="#1a73e8" readOnly></s-color-field>
  </s-stack>
  ```

- #### Form Integration

  ##### Description

  A multi-color field form section demonstrating how ColorField can be used to capture different color settings in a single form.

  ##### jsx

  ```jsx
  <s-stack gap="base">
    <s-section>
      <s-heading>Theme settings</s-heading>
      <s-stack gap="base">
        <s-color-field
          label="Primary brand color"
          name="primaryColor"
          value="#1a73e8"
          defaultValue="#1a73e8"
          details="This color will be used for buttons, links, and brand elements"
          required
         />
        <s-color-field
          label="Secondary color"
          name="secondaryColor"
          value="#34a853"
          details="Used for secondary actions and accents"
         />
        <s-color-field
          label="Background overlay"
          name="overlayColor"
          value="rgba(0, 0, 0, 0.5)"
          alpha
          details="Background color for modal overlays and dropdowns"
         />
      </s-stack>
    </s-section>
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack gap="base">
    <s-section>
      <s-heading>Theme settings</s-heading>
      <s-stack gap="base">
        <s-color-field
          label="Primary brand color"
          name="primaryColor"
          value="#1a73e8"
          defaultValue="#1a73e8"
          details="This color will be used for buttons, links, and brand elements"
          required
        ></s-color-field>
        <s-color-field
          label="Secondary color"
          name="secondaryColor"
          value="#34a853"
          details="Used for secondary actions and accents"
        ></s-color-field>
        <s-color-field
          label="Background overlay"
          name="overlayColor"
          value="rgba(0, 0, 0, 0.5)"
          alpha
          details="Background color for modal overlays and dropdowns"
        ></s-color-field>
      </s-stack>
    </s-section>
  </s-stack>
  ```

- #### Color validation

  ##### Description

  Interactive example showing real-time hex color validation with error messages.

  ##### jsx

  ```jsx
  const [color, setColor] = useState('#invalid');
  const [error, setError] = useState('Please enter a valid color format');

  return (
    <s-section>
      <s-stack gap="base" justifyContent="start">
        <s-text-field label="Theme name" />
        <s-color-field
          label="Brand color"
          name="Color"
          value={color}
          error={error}
          required
          onInput={(e) => {
            setColor(e.currentTarget.value);
            setError(/^#([0-9A-F]{3}){1,2}$/i.test(e.currentTarget.value) ? '' : 'Please enter a valid color format');
          }}
        />
      </s-stack>
    </s-section>
  )
  ```

## Best practices

- Use the alpha property to allow merchants to select transparent colors
- Provide clear labels that indicate what the color will be used for
- Use details text to provide context about the color's purpose
- Validate color format inputs and provide clear error messages

</page>

<page>
---
title: ColorPicker
description: Allow users to select a color from a color palette.
api_name: app-home
source_url:
  html: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/forms/colorpicker
  md: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/forms/colorpicker.md
---

# Color​Picker

Allow users to select a color from a color palette.

## Properties

- **alpha**

  **boolean**

  **Default: false**

  Allow user to select an alpha value.

- **defaultValue**

  **string**

  The default value for the field.

- **formResetCallback**

  **() => void**

- **name**

  **string**

  An identifier for the field that is unique within the nearest containing form.

- **value**

  **string**

  The currently selected color.

  Supported formats include:

  - HSL

## Events

Learn more about [registering events](https://shopify.dev/docs/api/app-home/using-polaris-components#event-handling).

- **change**

  **CallbackEventListener\<typeof tagName> | null**

- **input**

  **CallbackEventListener\<typeof tagName> | null**

### CallbackEventListener

```ts
(EventListener & {
      (event: CallbackEvent<T>): void;
    }) | null
```

### CallbackEvent

```ts
Event & {
  currentTarget: HTMLElementTagNameMap[T];
}
```

Examples

### Examples

- #### Code

  ##### jsx

  ```jsx
  <s-color-picker value="#FF0000" alpha />
  ```

  ##### html

  ```html
  <s-color-picker value="#FF0000" alpha></s-color-picker>
  ```

- #### Basic usage

  ##### Description

  Demonstrates a simple color picker with a pre-selected red color, showing the basic implementation without alpha transparency.

  ##### jsx

  ```jsx
  <s-box padding="large" border="base" borderRadius="base">
    <s-color-picker value="#FF0000" name="primary-color" />
  </s-box>
  ```

  ##### html

  ```html
  <s-box padding="large" border="base" borderRadius="base">
    <s-color-picker value="#FF0000" name="primary-color"></s-color-picker>
  </s-box>
  ```

- #### With alpha transparency

  ##### Description

  Illustrates a color picker with alpha transparency enabled, allowing selection of colors with varying opacity levels.

  ##### jsx

  ```jsx
  <s-box padding="large" border="base" borderRadius="base">
    <s-color-picker
      value="#FF0000FF"
      alpha
      name="color-with-alpha"
     />
  </s-box>
  ```

  ##### html

  ```html
  <s-box padding="large" border="base" borderRadius="base">
    <s-color-picker
      value="#FF0000FF"
      alpha
      name="color-with-alpha"
    ></s-color-picker>
  </s-box>
  ```

## Best practices

- Use the alpha slider if you want to allow merchants to select a transparent color

</page>

<page>
---
title: DateField
description: Allow users to select a specific date with a date picker.
api_name: app-home
source_url:
  html: 'https://shopify.dev/docs/api/app-home/polaris-web-components/forms/datefield'
  md: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/forms/datefield.md
---

# Date​Field

Allow users to select a specific date with a date picker.

## Properties

- **allow**

  **string**

  **Default: ""**

  Dates that can be selected.

  A comma-separated list of dates, date ranges. Whitespace is allowed after commas.

  The default `''` allows all dates.

  - Dates in `YYYY-MM-DD` format allow a single date.

  - Dates in `YYYY-MM` format allow a whole month.

  - Dates in `YYYY` format allow a whole year.

  - Ranges are expressed as `start--end`. - Ranges are inclusive.

    - If either `start` or `end` is omitted, the range is unbounded in that direction.
    - If parts of the date are omitted for `start`, they are assumed to be the minimum possible value. So `2024--` is equivalent to `2024-01-01--`.
    - If parts of the date are omitted for `end`, they are assumed to be the maximum possible value. So `--2024` is equivalent to `--2024-12-31`.
    - Whitespace is allowed either side of `--`.

- **allowDays**

  **string**

  **Default: ""**

  Days of the week that can be selected. These intersect with the result of `allow` and `disallow`.

  A comma-separated list of days. Whitespace is allowed after commas.

  The default `''` has no effect on the result of `allow` and `disallow`.

  Days are `sunday`, `monday`, `tuesday`, `wednesday`, `thursday`, `friday`, `saturday`.

- **autocomplete**

  **DateAutocompleteField**

  **Default: 'on' for everything else**

  A hint as to the intended content of the field.

  When set to `on` (the default), this property indicates that the field should support autofill, but you do not have any more semantic information on the intended contents.

  When set to `off`, you are indicating that this field contains sensitive information, or contents that are never saved, like one-time codes.

  Alternatively, you can provide value which describes the specific data you would like to be entered into this field during autofill.

- **defaultValue**

  **string**

  The default value for the field.

- **defaultView**

  **string**

  Default month to display in `YYYY-MM` format.

  This value is used until `view` is set, either directly or as a result of user interaction.

  Defaults to the current month in the user's locale.

- **details**

  **string**

  Additional text to provide context or guidance for the field. This text is displayed along with the field and its label to offer more information or instructions to the user.

  This will also be exposed to screen reader users.

- **disabled**

  **boolean**

  **Default: false**

  Disables the field, disallowing any interaction.

- **disallow**

  **string**

  **Default: ""**

  Dates that cannot be selected. These subtract from `allow`.

  A comma-separated list of dates, date ranges. Whitespace is allowed after commas.

  The default `''` has no effect on `allow`.

  - Dates in `YYYY-MM-DD` format disallow a single date.

  - Dates in `YYYY-MM` format disallow a whole month.

  - Dates in `YYYY` format disallow a whole year.

  - Ranges are expressed as `start--end`. - Ranges are inclusive.

    - If either `start` or `end` is omitted, the range is unbounded in that direction.
    - If parts of the date are omitted for `start`, they are assumed to be the minimum possible value. So `2024--` is equivalent to `2024-01-01--`.
    - If parts of the date are omitted for `end`, they are assumed to be the maximum possible value. So `--2024` is equivalent to `--2024-12-31`.
    - Whitespace is allowed either side of `--`.

- **disallowDays**

  **string**

  **Default: ""**

  Days of the week that cannot be selected. This subtracts from `allowDays`, and intersects with the result of `allow` and `disallow`.

  A comma-separated list of days. Whitespace is allowed after commas.

  The default `''` has no effect on `allowDays`.

  Days are `sunday`, `monday`, `tuesday`, `wednesday`, `thursday`, `friday`, `saturday`.

- **error**

  **string**

  Indicate an error to the user. The field will be given a specific stylistic treatment to communicate problems that have to be resolved immediately.

- **id**

  **string**

  A unique identifier for the element.

- **label**

  **string**

  Content to use as the field label.

- **labelAccessibilityVisibility**

  **"visible" | "exclusive"**

  **Default: 'visible'**

  Changes the visibility of the component's label.

  - `visible`: the label is visible to all users.
  - `exclusive`: the label is visually hidden but remains in the accessibility tree.

- **name**

  **string**

  An identifier for the field that is unique within the nearest containing form.

- **placeholder**

  **string**

  A short hint that describes the expected value of the field.

- **readOnly**

  **boolean**

  **Default: false**

  The field cannot be edited by the user. It is focusable will be announced by screen readers.

- **required**

  **boolean**

  **Default: false**

  Whether the field needs a value. This requirement adds semantic value to the field, but it will not cause an error to appear automatically. If you want to present an error when this field is empty, you can do so with the `error` property.

- **value**

  **string**

  The current value for the field. If omitted, the field will be empty.

- **view**

  **string**

  Displayed month in `YYYY-MM` format.

  `onViewChange` is called when this value changes.

  Defaults to `defaultView`.

### DateAutocompleteField

```ts
'bday-day' | 'bday-month' | 'bday-year' | 'bday' | 'cc-expiry-month' | 'cc-expiry-year' | 'cc-expiry'
```

## Events

Learn more about [registering events](https://shopify.dev/docs/api/app-home/using-polaris-components#event-handling).

- **blur**

  **CallbackEventListener<'input'>**

- **change**

  **CallbackEventListener<'input'>**

- **focus**

  **CallbackEventListener<'input'>**

- **input**

  **CallbackEventListener<'input'>**

- **invalid**

  **CallbackEventListener\<typeof tagName> | null**

- **viewchange**

  **CallbackEventListener\<typeof tagName> | null**

### CallbackEventListener

```ts
(EventListener & {
      (event: CallbackEvent<T>): void;
    }) | null
```

### CallbackEvent

```ts
Event & {
  currentTarget: HTMLElementTagNameMap[T];
}
```

Examples

### Examples

- #### Code

  ##### jsx

  ```jsx
  <s-date-field defaultView="2025-09" defaultValue="2025-09-01" />
  ```

  ##### html

  ```html
  <s-date-field defaultView="2025-09" defaultValue="2025-09-01"></s-date-field>
  ```

- #### Basic usage

  ##### Description

  Simple date field for collecting a single date with a descriptive label.

  ##### jsx

  ```jsx
  <s-date-field
    label="Order date"
    name="orderDate"
    placeholder="Select date"
   />
  ```

  ##### html

  ```html
  <s-date-field
    label="Order date"
    name="orderDate"
    placeholder="Select date"
  ></s-date-field>
  ```

- #### With default value

  ##### Description

  Date field pre-populated with a specific date for editing existing data.

  ##### jsx

  ```jsx
  <s-date-field
    label="Shipping date"
    name="shippingDate"
    value="2024-03-15"
   />
  ```

  ##### html

  ```html
  <s-date-field
    label="Shipping date"
    name="shippingDate"
    value="2024-03-15"
  ></s-date-field>
  ```

- #### With date restrictions

  ##### Description

  Shows how to restrict selectable dates to weekdays only.

  ##### jsx

  ```jsx
  <s-date-field
    label="Delivery date"
    name="deliveryDate"
    disallowDays="[0, 6]"
    details="Delivery available Monday through Friday only"
   />
  ```

  ##### html

  ```html
  <s-date-field
    label="Delivery date"
    name="deliveryDate"
    disallowDays="[0, 6]"
    details="Delivery available Monday through Friday only"
  ></s-date-field>
  ```

- #### With specific allowed dates

  ##### Description

  Demonstrates allowing only specific dates from a predefined list.

  ##### jsx

  ```jsx
  <s-date-field
    label="Available appointment dates"
    name="appointmentDate"
    allow='["2024-03-20", "2024-03-22", "2024-03-25", "2024-03-27"]'
    details="Select from available time slots"
   />
  ```

  ##### html

  ```html
  <s-date-field
    label="Available appointment dates"
    name="appointmentDate"
    allow='["2024-03-20", "2024-03-22", "2024-03-25", "2024-03-27"]'
    details="Select from available time slots"
  ></s-date-field>
  ```

- #### With error state

  ##### Description

  Date field showing validation error for required field or invalid date entry.

  ##### jsx

  ```jsx
  <s-date-field
    label="Event date"
    error="Event date is required"
    required
  />
  ```

  ##### html

  ```html
  <s-date-field
    label="Event date"
    name="eventDate"
    required
    error="Select a valid event date"
  ></s-date-field>
  ```

- #### Disabled and read-only states

  ##### Description

  Shows date fields in different interaction states for viewing-only or disabled forms.

  ##### jsx

  ```jsx
  <s-stack gap="base">
    <s-date-field
      label="Creation date"
      name="createdDate"
      value="2024-01-01"
      readOnly
     />

    <s-date-field
      label="Archived date"
      name="archivedDate"
      value="2024-02-15"
      disabled
     />
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack gap="base">
    <s-date-field
      label="Creation date"
      name="createdDate"
      value="2024-01-01"
      readOnly
    ></s-date-field>

    <s-date-field
      label="Archived date"
      name="archivedDate"
      value="2024-02-15"
      disabled
    ></s-date-field>
  </s-stack>
  ```

- #### Form integration

  ##### Description

  Complete form example showing date field with other form elements.

  ##### jsx

  ```jsx
  <form>
    <s-stack gap="base">
      <s-text-field
        label="Customer name"
        name="customerName"
        required
       />

      <s-date-field
        label="Order date"
        name="orderDate"
        value="2024-03-15"
        required
       />

      <s-date-field
        label="Requested delivery date"
        name="deliveryDate"
        disallowDays="[0, 6]"
        details="Weekdays only"
       />

      <s-button type="submit" variant="primary">
        Process order
      </s-button>
    </s-stack>
  </form>
  ```

  ##### html

  ```html
  <form>
    <s-stack gap="base">
      <s-text-field
        label="Customer name"
        name="customerName"
        required
      ></s-text-field>

      <s-date-field
        label="Order date"
        name="orderDate"
        value="2024-03-15"
        required
      ></s-date-field>

      <s-date-field
        label="Requested delivery date"
        name="deliveryDate"
        disallowDays="[0, 6]"
        details="Weekdays only"
      ></s-date-field>

      <s-button type="submit" variant="primary">Process order</s-button>
    </s-stack>
  </form>
  ```

- #### Date range selection

  ##### Description

  Example showing two date fields for selecting a date range.

  ##### jsx

  ```jsx
  <s-stack gap="base">
    <s-heading>Sales report period</s-heading>

    <s-stack direction="inline" gap="base">
      <s-date-field
        label="Start date"
        name="startDate"
        id="report-start"
       />

      <s-date-field
        label="End date"
        name="endDate"
        id="report-end"
       />
    </s-stack>

    <s-button variant="primary">Generate report</s-button>
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack gap="base">
    <s-heading>Sales report period</s-heading>

    <s-stack direction="inline" gap="base">
      <s-date-field
        label="Start date"
        name="startDate"
        id="report-start"
      ></s-date-field>

      <s-date-field
        label="End date"
        name="endDate"
        id="report-end"
      ></s-date-field>
    </s-stack>

    <s-button variant="primary">Generate report</s-button>
  </s-stack>
  ```

- #### Date fields with validation

  ##### Description

  Demonstrates date fields with business logic restrictions and validation.

  ##### jsx

  ```jsx
  <s-section>
    <s-heading>Promotion settings</s-heading>
    <s-stack gap="base">
      <s-text-field
        label="Promotion name"
        name="promoName"
        value="Spring sale"
       />

      <s-date-field
        label="Start date"
        name="promoStart"
        value="2024-04-01"
        details="Promotion begins at midnight"
       />

      <s-date-field
        label="End date"
        name="promoEnd"
        value="2024-04-30"
        details="Promotion ends at 11:59 PM"
       />

      <s-checkbox
        name="autoPublish"
        label="Automatically publish on start date"
       />
    </s-stack>
  </s-section>
  ```

  ##### html

  ```html
  <s-section>
    <s-heading>Promotion settings</s-heading>
    <s-stack gap="base">
      <s-text-field
        label="Promotion name"
        name="promoName"
        value="Spring sale"
      ></s-text-field>

      <s-date-field
        label="Start date"
        name="promoStart"
        value="2024-04-01"
        details="Promotion begins at midnight"
      ></s-date-field>

      <s-date-field
        label="End date"
        name="promoEnd"
        value="2024-04-30"
        details="Promotion ends at 11:59 PM"
      ></s-date-field>

      <s-checkbox
        name="autoPublish"
        label="Automatically publish on start date"
      ></s-checkbox>
    </s-stack>
  </s-section>
  ```

- #### Date field validation

  ##### Description

  Interactive example showing required date field validation with dynamic error messages.

  ##### jsx

  ```jsx
  const [date, setDate] = useState('');
  const [error, setError] = useState('Event date is required');

  return (
    <s-section>
      <s-stack gap="base" justifyContent="start">
        <s-text-field label="Event name" />
        <s-date-field
          label="Event date"
          value={date}
          error={error}
          required
          onChange={(e) => {
            setDate(e.currentTarget.value);
            setError(e.currentTarget.value ? '' : 'Event date is required');
          }}
        />
      </s-stack>
    </s-section>
  )
  ```

  ##### html

  ```html
  <s-date-field
    label="Event date"
    name="eventDate"
    required
    error="Select a valid event date"
  ></s-date-field>
  ```

## Best practices

- Use smart defaults and highlight common selections
- Use `allow` and `disallow` properties to restrict selectable dates appropriately
- Provide clear labels and use details text to explain date restrictions
- Don't use for dates that are many years in the future or the past

</page>

<page>
---
title: DatePicker
description: Allow users to select a specific date or date range.
api_name: app-home
source_url:
  html: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/forms/datepicker
  md: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/forms/datepicker.md
---

# Date​Picker

Allow users to select a specific date or date range.

## DatePicker

- **allow**

  **string**

  **Default: ""**

  Dates that can be selected.

  A comma-separated list of dates, date ranges. Whitespace is allowed after commas.

  The default `''` allows all dates.

  - Dates in `YYYY-MM-DD` format allow a single date.

  - Dates in `YYYY-MM` format allow a whole month.

  - Dates in `YYYY` format allow a whole year.

  - Ranges are expressed as `start--end`. - Ranges are inclusive.

    - If either `start` or `end` is omitted, the range is unbounded in that direction.
    - If parts of the date are omitted for `start`, they are assumed to be the minimum possible value. So `2024--` is equivalent to `2024-01-01--`.
    - If parts of the date are omitted for `end`, they are assumed to be the maximum possible value. So `--2024` is equivalent to `--2024-12-31`.
    - Whitespace is allowed either side of `--`.

- **allowDays**

  **string**

  **Default: ""**

  Days of the week that can be selected. These intersect with the result of `allow` and `disallow`.

  A comma-separated list of days. Whitespace is allowed after commas.

  The default `''` has no effect on the result of `allow` and `disallow`.

  Days are `sunday`, `monday`, `tuesday`, `wednesday`, `thursday`, `friday`, `saturday`.

- **defaultValue**

  **string**

  **Default: ""**

  Default selected value.

  The default means no date is selected.

  If the provided value is invalid, no date is selected.

  - If `type="single"`, this is a date in `YYYY-MM-DD` format.
  - If `type="multiple"`, this is a comma-separated list of dates in `YYYY-MM-DD` format.
  - If `type="range"`, this is a range in `YYYY-MM-DD--YYYY-MM-DD` format. The range is inclusive.

- **defaultView**

  **string**

  Default month to display in `YYYY-MM` format.

  This value is used until `view` is set, either directly or as a result of user interaction.

  Defaults to the current month in the user's locale.

- **disallow**

  **string**

  **Default: ""**

  Dates that cannot be selected. These subtract from `allow`.

  A comma-separated list of dates, date ranges. Whitespace is allowed after commas.

  The default `''` has no effect on `allow`.

  - Dates in `YYYY-MM-DD` format disallow a single date.

  - Dates in `YYYY-MM` format disallow a whole month.

  - Dates in `YYYY` format disallow a whole year.

  - Ranges are expressed as `start--end`. - Ranges are inclusive.

    - If either `start` or `end` is omitted, the range is unbounded in that direction.
    - If parts of the date are omitted for `start`, they are assumed to be the minimum possible value. So `2024--` is equivalent to `2024-01-01--`.
    - If parts of the date are omitted for `end`, they are assumed to be the maximum possible value. So `--2024` is equivalent to `--2024-12-31`.
    - Whitespace is allowed either side of `--`.

- **disallowDays**

  **string**

  **Default: ""**

  Days of the week that cannot be selected. This subtracts from `allowDays`, and intersects with the result of `allow` and `disallow`.

  A comma-separated list of days. Whitespace is allowed after commas.

  The default `''` has no effect on `allowDays`.

  Days are `sunday`, `monday`, `tuesday`, `wednesday`, `thursday`, `friday`, `saturday`.

- **name**

  **string**

  An identifier for the field that is unique within the nearest containing form.

- **type**

  **"single" | "range"**

  **Default: "single"**

- **value**

  **string**

  **Default: ""**

  Current selected value.

  The default means no date is selected.

  If the provided value is invalid, no date is selected.

  Otherwise:

  - If `type="single"`, this is a date in `YYYY-MM-DD` format.
  - If `type="multiple"`, this is a comma-separated list of dates in `YYYY-MM-DD` format.
  - If `type="range"`, this is a range in `YYYY-MM-DD--YYYY-MM-DD` format. The range is inclusive.

- **view**

  **string**

  Displayed month in `YYYY-MM` format.

  `onViewChange` is called when this value changes.

  Defaults to `defaultView`.

## Events

Learn more about [registering events](https://shopify.dev/docs/api/app-home/using-polaris-components#event-handling).

- **blur**

  **CallbackEventListener\<typeof tagName> | null**

- **change**

  **CallbackEventListener\<typeof tagName> | null**

- **focus**

  **CallbackEventListener\<typeof tagName> | null**

- **input**

  **CallbackEventListener\<typeof tagName> | null**

- **viewchange**

  **CallbackEventListener\<typeof tagName> | null**

### CallbackEventListener

```ts
(EventListener & {
      (event: CallbackEvent<T>): void;
    }) | null
```

### CallbackEvent

```ts
Event & {
  currentTarget: HTMLElementTagNameMap[T];
}
```

Examples

### Examples

- #### Code

  ##### jsx

  ```jsx
  <s-date-picker
    view="2025-05"
    type="range"
    value="2025-05-28--2025-05-31"
   />
  ```

  ##### html

  ```html
  <s-date-picker
    view="2025-05"
    type="range"
    value="2025-05-28--2025-05-31"
  ></s-date-picker>
  ```

- #### Single date selection

  ##### Description

  Demonstrates a date picker configured for selecting a single date with a default value and specific month view.

  ##### jsx

  ```jsx
  <s-date-picker
    type="single"
    name="delivery-date"
    value="2024-01-15"
    view="2024-01"
   />
  ```

  ##### html

  ```html
  <s-date-picker
    type="single"
    name="delivery-date"
    value="2024-01-15"
    view="2024-01"
  ></s-date-picker>
  ```

- #### With date restrictions

  ##### Description

  Illustrates how to restrict date selection to a specific date range, preventing selection of past or future dates outside the allowed period.

  ##### jsx

  ```jsx
  <s-date-picker
    type="single"
    name="appointment-date"
    disallow="past"
    allow="2024-06-01--2024-06-31"
    view="2024-06"
   />
  ```

  ##### html

  ```html
  <!-- Disable past dates and far future dates -->
  <s-date-picker
    type="single"
    name="appointment-date"
    disallow="past"
    allow="2024-06-01--2024-06-31"
    view="2024-06"
  ></s-date-picker>
  ```

- #### Handling onChange callbacks

  ##### Description

  Demonstrates how to handle onChange callbacks for both single and range date pickers, showing how to extract and process the selected values.

  ##### jsx

  ```jsx
  const [dateRange, setDateRange] = useState('2024-01-01--2024-01-31');
  const [orderNumber, setOrderNumber] = useState('');

  const handleApplyFilters = () => {
    console.log('Applying filters:', {
      orderNumber,
      dateRange
    });
  }

  return (
    <s-stack gap="base">
      <s-text-field
        label="Order number"
        placeholder="Search orders..."
        value={orderNumber}
        onChange={(event) => setOrderNumber(event.currentTarget.value)}
      />
      <s-date-picker
        type="range"
        name="order-date-range"
        value={dateRange}
        onChange={(event) => setDateRange(event.currentTarget.value)}
        view="2024-01"
      />
      <s-button onClick={handleApplyFilters}>Apply filters</s-button>
    </s-stack>
  )
  ```

  ##### html

  ```html
  <form>
    <s-text-field
      label="Order number"
      placeholder="Search orders..."
    ></s-text-field>
    <s-date-picker
      type="range"
      name="order-date-range"
      value="2024-01-01--2024-01-31"
      view="2024-01"
    ></s-date-picker>
    <s-button type="submit">Apply filters</s-button>
  </form>
  ```

- #### With quick date selection

  ##### Description

  Illustrates a date picker with quick preset buttons and onChange callback to capture user selections and update the displayed value.

  ##### jsx

  ```jsx
  const [value, setValue] = useState('2025-01-01--2025-01-31');

  const last7Days = () => {
    setValue('2025-01-07--2025-01-13');
  }

  const last30Days = () => {
    setValue('2024-12-14--2025-01-13');
  }

  const thisMonth = () => {
    setValue('2025-01-01--2025-01-31');
  }

  return (
    <s-stack gap="base">
      <s-button-group>
        <s-button slot="secondary-actions" onClick={last7Days}>Last 7 days</s-button>
        <s-button slot="secondary-actions" onClick={last30Days}>Last 30 days</s-button>
        <s-button slot="secondary-actions" onClick={thisMonth}>This month</s-button>
      </s-button-group>
      <s-date-picker
        type="range"
        name="analytics-period"
        id="analytics-picker"
        view="2025-01"
        value={value}
        onChange={(event) => {
          console.log('Date range changed:', event.currentTarget.value);
          setValue(event.currentTarget.value);
        }}
      />
      <s-text>Selected range: {value}</s-text>
    </s-stack>
  )
  ```

  ##### html

  ```html
  <!-- Quick date selection with onChange callback -->
  <s-stack gap="base">
    <s-button-group>
      <s-button slot="secondary-actions" id="last-7-days">Last 7 days</s-button>
      <s-button slot="secondary-actions" id="last-30-days">Last 30 days</s-button>
      <s-button slot="secondary-actions" id="this-month">This month</s-button>
    </s-button-group>
    <s-date-picker
      type="range"
      name="analytics-period"
      id="analytics-picker"
      value="2025-01-01--2025-01-31"
      view="2025-01"
      onchange="console.log('Date range changed:', event.currentTarget.value)"
    ></s-date-picker>
    <s-text id="selected-range">
      Selected range: 2025-01-01--2025-01-31
    </s-text>
  </s-stack>

  <script>
    const picker = document.getElementById('analytics-picker');
    const display = document.getElementById('selected-range');

    // Handle picker changes
    picker.addEventListener('change', (event) => {
      display.textContent = `Selected range: ${event.currentTarget.value}`;
    });

    // Quick selection buttons
    document.getElementById('last-7-days').addEventListener('click', () => {
      picker.value = '2025-01-07--2025-01-13';
      display.textContent = 'Selected range: 2025-01-07--2025-01-13';
    });

    document.getElementById('last-30-days').addEventListener('click', () => {
      picker.value = '2024-12-14--2025-01-13';
      display.textContent = 'Selected range: 2024-12-14--2025-01-13';
    });

    document.getElementById('this-month').addEventListener('click', () => {
      picker.value = '2025-01-01--2025-01-31';
      display.textContent = 'Selected range: 2025-01-01--2025-01-31';
    });
  </script>
  ```

## Best practices

- Use smart defaults and highlight common selections
- Don't use to enter a date that is many years in the future or the past

</page>

<page>
---
title: DropZone
description: >-
  Lets users upload files through drag-and-drop functionality into a designated
  area on a page, or by activating a button.
api_name: app-home
source_url:
  html: 'https://shopify.dev/docs/api/app-home/polaris-web-components/forms/dropzone'
  md: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/forms/dropzone.md
---

# Drop​Zone

Lets users upload files through drag-and-drop functionality into a designated area on a page, or by activating a button.

## Properties

- **accept**

  **string**

  **Default: ''**

  A string representing the types of files that are accepted by the drop zone. This string is a comma-separated list of unique file type specifiers which can be one of the following:

  - A file extension starting with a period (".") character (e.g. .jpg, .pdf, .doc)
  - A valid MIME type string with no extensions

  If omitted, all file types are accepted.

- **accessibilityLabel**

  **string**

  A label that describes the purpose or contents of the item. When set, it will be announced to buyers using assistive technologies and will provide them with more context.

- **disabled**

  **boolean**

  **Default: false**

  Disables the field, disallowing any interaction.

- **error**

  **string**

  Indicate an error to the user. The field will be given a specific stylistic treatment to communicate problems that have to be resolved immediately.

- **files**

  **File\[]**

  **Default: \[]**

  An array of File objects representing the files currently selected by the user.

  This property is read-only and cannot be directly modified. To clear the selected files, set the `value` prop to an empty string or null.

- **label**

  **string**

  Content to use as the field label.

- **labelAccessibilityVisibility**

  **"visible" | "exclusive"**

  **Default: 'visible'**

  Changes the visibility of the component's label.

  - `visible`: the label is visible to all users.
  - `exclusive`: the label is visually hidden but remains in the accessibility tree.

- **multiple**

  **boolean**

  **Default: false**

  Whether multiple files can be selected or dropped at once.

- **name**

  **string**

  An identifier for the field that is unique within the nearest containing form.

- **required**

  **boolean**

  **Default: false**

  Whether the field needs a value. This requirement adds semantic value to the field, but it will not cause an error to appear automatically. If you want to present an error when this field is empty, you can do so with the `error` property.

- **value**

  **string**

  **Default: ''**

  This sets the input value for a file type, which cannot be set programatically, so it can only be reset.

## Events

Learn more about [registering events](https://shopify.dev/docs/api/app-home/using-polaris-components#event-handling).

- **change**

  **CallbackEventListener\<typeof tagName$K>**

- **droprejected**

  **CallbackEventListener\<typeof tagName$K>**

- **input**

  **CallbackEventListener\<typeof tagName$K>**

### CallbackEventListener

```ts
(EventListener & {
      (event: CallbackEvent<T>): void;
    }) | null
```

### CallbackEvent

```ts
Event & {
  currentTarget: HTMLElementTagNameMap[T];
}
```

Examples

### Examples

- #### Code

  ##### jsx

  ```jsx
  <s-drop-zone
    label="Upload"
    accessibilityLabel="Upload image of type jpg, png, or gif"
    accept=".jpg,.png,.gif"
    multiple
    onInput={(event) => console.log('onInput', event.currentTarget?.value)}
    onChange={(event) => console.log('onChange', event.currentTarget?.value)}
    onDropRejected={(event) => console.log('onDropRejected', event.currentTarget?.value)}
   />
  ```

  ##### html

  ```html
  <s-drop-zone
    label="Upload"
    accessibilityLabel="Upload image of type jpg, png, or gif"
    accept=".jpg,.png,.gif"
    multiple
    onInput="console.log('onInput', event.currentTarget?.value)"
    onChange="console.log('onChange', event.currentTarget?.value)"
    onDropRejected="console.log('onDropRejected', event.currentTarget?.value)"
  ></s-drop-zone>
  ```

- #### Basic usage

  ##### Description

  Demonstrates a basic drop zone that allows multiple file uploads with a simple label.

  ##### jsx

  ```jsx
  <s-drop-zone label="Drop files to upload" multiple />
  ```

  ##### html

  ```html
  <s-drop-zone label="Drop files to upload" multiple></s-drop-zone>
  ```

- #### Image upload

  ##### Description

  Shows a drop zone configured specifically for uploading multiple image files.

  ##### jsx

  ```jsx
  <s-drop-zone accept="image/*" label="Upload images" multiple />
  ```

  ##### html

  ```html
  <s-drop-zone accept="image/*" label="Upload images" multiple></s-drop-zone>
  ```

- #### With required field

  ##### Description

  Illustrates a drop zone when the file upload is required.

  ##### jsx

  ```jsx
  <s-drop-zone name="file" required label="Upload file" />
  ```

  ##### html

  ```html
  <s-drop-zone name="file" required label="Upload file"></s-drop-zone>
  ```

- #### Disabled state

  ##### Description

  Displays a drop zone in a disabled state, preventing file uploads.

  ##### jsx

  ```jsx
  <s-drop-zone label="Upload not available" disabled />
  ```

  ##### html

  ```html
  <s-drop-zone label="Upload not available" disabled></s-drop-zone>
  ```

- #### File type restrictions

  ##### Description

  Demonstrates restricting file uploads to specific document types like PDF and DOC.

  ##### jsx

  ```jsx
  <s-drop-zone
    accept=".pdf,.doc,.docx"
    label="Upload documents"
    multiple
   />
  ```

  ##### html

  ```html
  <s-drop-zone
    accept=".pdf,.doc,.docx"
    label="Upload documents"
    multiple
  ></s-drop-zone>
  ```

- #### With error state

  ##### Description

  Shows a drop zone with an error message, useful for indicating file upload validation issues.

  ##### jsx

  ```jsx
  <s-drop-zone
    label="Upload file"
    error="File size must be less than 5mb"
   />
  ```

  ##### html

  ```html
  <s-drop-zone
    label="Upload file"
    error="File size must be less than 5mb"
  ></s-drop-zone>
  ```

- #### With accessibility options

  ##### Description

  Illustrates advanced accessibility configuration for the drop zone, including custom accessibility labels.

  ##### jsx

  ```jsx
  <s-drop-zone
    label="Upload files"
    accessibilityLabel="Upload files using drag and drop or file selector"
    labelAccessibilityVisibility="exclusive"
    multiple
   />
  ```

  ##### html

  ```html
  <s-drop-zone
    label="Upload files"
    accessibilityLabel="Upload files using drag and drop or file selector"
    labelAccessibilityVisibility="exclusive"
    multiple
  ></s-drop-zone>
  ```

</page>

<page>
---
title: EmailField
description: Let users enter email addresses with optimized keyboard settings.
api_name: app-home
source_url:
  html: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/forms/emailfield
  md: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/forms/emailfield.md
---

# Email​Field

Let users enter email addresses with optimized keyboard settings.

## Properties

- **autocomplete**

  **"on" | "off" | EmailAutocompleteField | \`section-${string} email\` | \`section-${string} home email\` | \`section-${string} mobile email\` | \`section-${string} fax email\` | \`section-${string} pager email\` | "shipping email" | "shipping home email" | "shipping mobile email" | "shipping fax email" | "shipping pager email" | "billing email" | "billing home email" | "billing mobile email" | "billing fax email" | "billing pager email" | \`section-${string} shipping email\` | \`section-${string} shipping home email\` | \`section-${string} shipping mobile email\` | \`section-${string} shipping fax email\` | \`section-${string} shipping pager email\` | \`section-${string} billing email\` | \`section-${string} billing home email\` | \`section-${string} billing mobile email\` | \`section-${string} billing fax email\` | \`section-${string} billing pager email\`**

  **Default: 'on' for everything else**

  A hint as to the intended content of the field.

  When set to `on` (the default), this property indicates that the field should support autofill, but you do not have any more semantic information on the intended contents.

  When set to `off`, you are indicating that this field contains sensitive information, or contents that are never saved, like one-time codes.

  Alternatively, you can provide value which describes the specific data you would like to be entered into this field during autofill.

- **defaultValue**

  **string**

  The default value for the field.

- **details**

  **string**

  Additional text to provide context or guidance for the field. This text is displayed along with the field and its label to offer more information or instructions to the user.

  This will also be exposed to screen reader users.

- **disabled**

  **boolean**

  **Default: false**

  Disables the field, disallowing any interaction.

- **error**

  **string**

  Indicate an error to the user. The field will be given a specific stylistic treatment to communicate problems that have to be resolved immediately.

- **id**

  **string**

  A unique identifier for the element.

- **label**

  **string**

  Content to use as the field label.

- **labelAccessibilityVisibility**

  **"visible" | "exclusive"**

  **Default: 'visible'**

  Changes the visibility of the component's label.

  - `visible`: the label is visible to all users.
  - `exclusive`: the label is visually hidden but remains in the accessibility tree.

- **maxLength**

  **number**

  **Default: Infinity**

  Specifies the maximum number of characters allowed.

- **minLength**

  **number**

  **Default: 0**

  Specifies the min number of characters allowed.

- **name**

  **string**

  An identifier for the field that is unique within the nearest containing form.

- **placeholder**

  **string**

  A short hint that describes the expected value of the field.

- **readOnly**

  **boolean**

  **Default: false**

  The field cannot be edited by the user. It is focusable will be announced by screen readers.

- **required**

  **boolean**

  **Default: false**

  Whether the field needs a value. This requirement adds semantic value to the field, but it will not cause an error to appear automatically. If you want to present an error when this field is empty, you can do so with the `error` property.

- **value**

  **string**

  The current value for the field. If omitted, the field will be empty.

### EmailAutocompleteField

```ts
'email' | 'home email' | 'mobile email' | 'fax email' | 'pager email'
```

## Events

Learn more about [registering events](https://shopify.dev/docs/api/app-home/using-polaris-components#event-handling).

- **blur**

  **CallbackEventListener<'input'>**

- **change**

  **CallbackEventListener<'input'>**

- **focus**

  **CallbackEventListener<'input'>**

- **input**

  **CallbackEventListener<'input'>**

### CallbackEventListener

```ts
(EventListener & {
      (event: CallbackEvent<T>): void;
    }) | null
```

### CallbackEvent

```ts
Event & {
  currentTarget: HTMLElementTagNameMap[T];
}
```

Examples

### Examples

- #### Code

  ##### jsx

  ```jsx
  <s-email-field
    label="Email"
    placeholder="bernadette.lapresse@jadedpixel.com"
    details="Used for sending notifications"
   />
  ```

  ##### html

  ```html
  <s-email-field
    label="Email"
    placeholder="bernadette.lapresse@jadedpixel.com"
    details="Used for sending notifications"
  ></s-email-field>
  ```

- #### Basic usage

  ##### Description

  Demonstrates a simple email field with a label and required attribute, showing the most fundamental way to use the EmailField component.

  ##### jsx

  ```jsx
  <s-stack gap="base">
    <s-email-field label="Email address" required />
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack gap="base">
    <s-email-field label="Email address" required></s-email-field>
  </s-stack>
  ```

- #### With error and help text

  ##### Description

  Showcases an email field with additional details and an error message, illustrating how to provide contextual information and validation feedback.

  ##### jsx

  ```jsx
  <s-stack gap="base">
    <s-email-field
      label="Contact email"
      details="We'll send your order confirmation here"
      error="Please enter a valid email address"
      required
    />
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack gap="base">
    <s-email-field
      label="Contact email"
      details="We'll send your order confirmation here"
      error="Please enter a valid email address"
      required
    ></s-email-field>
  </s-stack>
  ```

- #### Optional field with placeholder

  ##### Description

  Illustrates an optional email field with a placeholder text and help text, demonstrating a common pattern for collecting alternative contact information.

  ##### jsx

  ```jsx
  <s-stack gap="base">
    <s-email-field
      label="Alternate email"
      placeholder="secondary@example.com"
      details="Additional email for notifications"
     />
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack gap="base">
    <s-email-field
      label="Alternate email"
      placeholder="secondary@example.com"
      details="Additional email for notifications"
    ></s-email-field>
  </s-stack>
  ```

- #### Read-only display

  ##### Description

  Shows how to render an email field in a read-only state, useful for displaying existing email addresses that cannot be modified.

  ##### jsx

  ```jsx
  <s-stack gap="base">
    <s-email-field
      label="Account email"
      value="user@example.com"
      readOnly
     />
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack gap="base">
    <s-email-field
      label="Account email"
      value="user@example.com"
      readOnly
    ></s-email-field>
  </s-stack>
  ```

- #### With length constraints

  ##### Description

  Demonstrates setting minimum and maximum length constraints for the email input, providing additional validation beyond the standard email format check.

  ##### jsx

  ```jsx
  <s-stack gap="base">
    <s-email-field
      label="Business email"
      minLength={5}
      maxLength={100}
      required
     />
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack gap="base">
    <s-email-field
      label="Business email"
      minLength="5"
      maxLength="100"
      required
    ></s-email-field>
  </s-stack>
  ```

- #### Email validation

  ##### Description

  Interactive example showing real-time email validation with error messages that update as the user types.

  ##### jsx

  ```jsx
  const [email, setEmail] = useState('invalid-email');
  const [error, setError] = useState('Please enter a valid email address');

  return (
    <s-section>
      <s-stack gap="base" justifyContent="start">
        <s-text-field label="Name" />
        <s-email-field
          label="Contact email"
          details="We'll send your order confirmation here"
          value={email}
          error={error}
          required
          onInput={(e) => {
            setEmail(e.currentTarget.value);
            setError(/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(e.currentTarget.value) ? '' : 'Please enter a valid email address');
          }}
        />
      </s-stack>
    </s-section>
  )
  ```

</page>

<page>
---
title: MoneyField
description: >-
  Collect monetary values from users with built-in currency formatting and
  validation.
api_name: app-home
source_url:
  html: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/forms/moneyfield
  md: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/forms/moneyfield.md
---

# Money​Field

Collect monetary values from users with built-in currency formatting and validation.

## Properties

- **autocomplete**

  **string**

  **Default: 'on' for everything else**

  A hint as to the intended content of the field.

  When set to `on` (the default), this property indicates that the field should support autofill, but you do not have any more semantic information on the intended contents.

  When set to `off`, you are indicating that this field contains sensitive information, or contents that are never saved, like one-time codes.

  Alternatively, you can provide value which describes the specific data you would like to be entered into this field during autofill.

- **defaultValue**

  **string**

  The default value for the field.

- **details**

  **string**

  Additional text to provide context or guidance for the field. This text is displayed along with the field and its label to offer more information or instructions to the user.

  This will also be exposed to screen reader users.

- **disabled**

  **boolean**

  **Default: false**

  Disables the field, disallowing any interaction.

- **error**

  **string**

  Indicate an error to the user. The field will be given a specific stylistic treatment to communicate problems that have to be resolved immediately.

- **id**

  **string**

  A unique identifier for the element.

- **label**

  **string**

  Content to use as the field label.

- **labelAccessibilityVisibility**

  **"visible" | "exclusive"**

  **Default: 'visible'**

  Changes the visibility of the component's label.

  - `visible`: the label is visible to all users.
  - `exclusive`: the label is visually hidden but remains in the accessibility tree.

- **max**

  **number**

  **Default: Infinity**

  The highest decimal or integer to be accepted for the field. When used with `step` the value will round down to the max number.

  Note: a user will still be able to use the keyboard to input a number higher than the max. It is up to the developer to add appropriate validation.

- **min**

  **number**

  **Default: -Infinity**

  The lowest decimal or integer to be accepted for the field. When used with `step` the value will round up to the min number.

  Note: a user will still be able to use the keyboard to input a number lower than the min. It is up to the developer to add appropriate validation.

- **name**

  **string**

  An identifier for the field that is unique within the nearest containing form.

- **placeholder**

  **string**

  A short hint that describes the expected value of the field.

- **readOnly**

  **boolean**

  **Default: false**

  The field cannot be edited by the user. It is focusable will be announced by screen readers.

- **required**

  **boolean**

  **Default: false**

  Whether the field needs a value. This requirement adds semantic value to the field, but it will not cause an error to appear automatically. If you want to present an error when this field is empty, you can do so with the `error` property.

- **value**

  **string**

  The current value for the field. If omitted, the field will be empty.

## Events

Learn more about [registering events](https://shopify.dev/docs/api/app-home/using-polaris-components#event-handling).

- **blur**

  **CallbackEventListener<'input'>**

- **change**

  **CallbackEventListener<'input'>**

- **focus**

  **CallbackEventListener<'input'>**

- **input**

  **CallbackEventListener<'input'>**

### CallbackEventListener

```ts
(EventListener & {
      (event: CallbackEvent<T>): void;
    }) | null
```

### CallbackEvent

```ts
Event & {
  currentTarget: HTMLElementTagNameMap[T];
}
```

Examples

### Examples

- #### Code

  ##### jsx

  ```jsx
  <s-money-field
    label="Regional Price"
    placeholder="99.99"
    details="Recommended price for the European market"
   />
  ```

  ##### html

  ```html
  <s-money-field
    label="Regional Price"
    placeholder="99.99"
    details="Recommended price for the European market"
  ></s-money-field>
  ```

- #### Basic usage

  ##### Description

  Demonstrates a simple money field with a label, initial value, and numeric constraints.

  ##### jsx

  ```jsx
  <s-money-field
    label="Price"
    value="19.99"
    min={0}
    max={1000}
   />
  ```

  ##### html

  ```html
  <s-money-field
    label="Price"
    value="19.99"
    min="0"
    max="1000"
  ></s-money-field>
  ```

- #### With validation limits

  ##### Description

  Showcases a money field with explicit minimum and maximum value limits, and a detailed description for user guidance.

  ##### jsx

  ```jsx
  <s-money-field
    label="Discount amount"
    value="5.00"
    min={0}
    max={100}
    details="Enter discount amount between $0 and $100"
  />
  ```

  ##### html

  ```html
  <s-money-field
    label="Discount amount"
    value="5.00"
    min="0"
    max="100"
    details="Enter discount amount between $0 and $100"
  ></s-money-field>
  ```

- #### Basic field

  ##### Description

  Illustrates a money field demonstrating basic error handling and validation.

  ##### jsx

  ```jsx
  <s-money-field
    label="Product cost"
    value="29.99"
    min={0.01}
    error="Product cost is required"
   />
  ```

  ##### html

  ```html
  <s-money-field
    label="Product cost"
    value="29.99"
    min="0.01"
    error="Product cost is required"
  ></s-money-field>
  ```

- #### Currency formatting with form integration

  ##### Description

  Displays multiple money fields in a vertical stack, showing how to integrate multiple currency inputs in a form with varied details and constraints.

  ##### jsx

  ```jsx
  <s-stack direction="block" gap="base">
    <s-money-field
      label="Price"
      value="0.00"
      min={0.01}
      max={99999.99}
      details="Customers will see this price"
     />

    <s-money-field
      label="Compare at price"
      value=""
      min={0}
      max={99999.99}
      details="Show customers the original price when on sale"
     />

    <s-money-field
      label="Cost per item"
      value=""
      min={0}
      max={99999.99}
      details="Customers won't see this"
     />
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack direction="block" gap="base">
    <s-money-field
      label="Price"
      value="0.00"
      min="0.01"
      max="99999.99"
      details="Customers will see this price"
    ></s-money-field>

    <s-money-field
      label="Compare at price"
      value=""
      min="0"
      max="99999.99"
      details="Show customers the original price when on sale"
    ></s-money-field>

    <s-money-field
      label="Cost per item"
      value=""
      min="0"
      max="99999.99"
      details="Customers won't see this"
    ></s-money-field>
  </s-stack>
  ```

- #### Money field validation

  ##### Description

  Interactive example showing real-time validation with min/max limits and dynamic error messages.

  ##### jsx

  ```jsx
  const [amount, setAmount] = useState('150');
  const [error, setError] = useState('Value must be no more than $100');

  return (
    <s-section>
      <s-stack gap="base" justifyContent="start">
        <s-text-field label="Product name" />
        <s-money-field
          label="Discount amount"
          value={amount}
          min={0}
          max={100}
          details="Enter discount amount between $0 and $100"
          error={error}
          onInput={(e) => {
            setAmount(e.currentTarget.value);
            const val = parseFloat(e.currentTarget.value);
            setError(val > e.currentTarget.max ? 'Value must be no more than $100' : val < e.currentTarget.min ? 'Value must be at least $0' : '');
          }}
        />
      </s-stack>
    </s-section>
  )
  ```

</page>

<page>
---
title: NumberField
description: >-
  Collect numerical values from users with optimized keyboard settings and
  built-in validation.
api_name: app-home
source_url:
  html: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/forms/numberfield
  md: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/forms/numberfield.md
---

# Number​Field

Collect numerical values from users with optimized keyboard settings and built-in validation.

## Properties

- **autocomplete**

  **"on" | "off" | NumberAutocompleteField | \`section-${string} one-time-code\` | \`section-${string} cc-number\` | \`section-${string} cc-csc\` | "shipping one-time-code" | "shipping cc-number" | "shipping cc-csc" | "billing one-time-code" | "billing cc-number" | "billing cc-csc" | \`section-${string} shipping one-time-code\` | \`section-${string} shipping cc-number\` | \`section-${string} shipping cc-csc\` | \`section-${string} billing one-time-code\` | \`section-${string} billing cc-number\` | \`section-${string} billing cc-csc\`**

  **Default: 'on' for everything else**

  A hint as to the intended content of the field.

  When set to `on` (the default), this property indicates that the field should support autofill, but you do not have any more semantic information on the intended contents.

  When set to `off`, you are indicating that this field contains sensitive information, or contents that are never saved, like one-time codes.

  Alternatively, you can provide value which describes the specific data you would like to be entered into this field during autofill.

- **defaultValue**

  **string**

  The default value for the field.

- **details**

  **string**

  Additional text to provide context or guidance for the field. This text is displayed along with the field and its label to offer more information or instructions to the user.

  This will also be exposed to screen reader users.

- **disabled**

  **boolean**

  **Default: false**

  Disables the field, disallowing any interaction.

- **error**

  **string**

  Indicate an error to the user. The field will be given a specific stylistic treatment to communicate problems that have to be resolved immediately.

- **id**

  **string**

  A unique identifier for the element.

- **inputMode**

  **"decimal" | "numeric"**

  **Default: 'decimal'**

  Sets the virtual keyboard.

- **label**

  **string**

  Content to use as the field label.

- **labelAccessibilityVisibility**

  **"visible" | "exclusive"**

  **Default: 'visible'**

  Changes the visibility of the component's label.

  - `visible`: the label is visible to all users.
  - `exclusive`: the label is visually hidden but remains in the accessibility tree.

- **max**

  **number**

  **Default: Infinity**

  The highest decimal or integer to be accepted for the field. When used with `step` the value will round down to the max number.

  Note: a user will still be able to use the keyboard to input a number higher than the max. It is up to the developer to add appropriate validation.

- **min**

  **number**

  **Default: -Infinity**

  The lowest decimal or integer to be accepted for the field. When used with `step` the value will round up to the min number.

  Note: a user will still be able to use the keyboard to input a number lower than the min. It is up to the developer to add appropriate validation.

- **name**

  **string**

  An identifier for the field that is unique within the nearest containing form.

- **placeholder**

  **string**

  A short hint that describes the expected value of the field.

- **prefix**

  **string**

  **Default: ''**

  A value to be displayed immediately before the editable portion of the field.

  This is useful for displaying an implied part of the value, such as "https://" or "+353".

  This cannot be edited by the user, and it isn't included in the value of the field.

  It may not be displayed until the user has interacted with the input. For example, an inline label may take the place of the prefix until the user focuses the input.

- **readOnly**

  **boolean**

  **Default: false**

  The field cannot be edited by the user. It is focusable will be announced by screen readers.

- **required**

  **boolean**

  **Default: false**

  Whether the field needs a value. This requirement adds semantic value to the field, but it will not cause an error to appear automatically. If you want to present an error when this field is empty, you can do so with the `error` property.

- **step**

  **number**

  **Default: 1**

  The amount the value can increase or decrease by. This can be an integer or decimal. If a `max` or `min` is specified with `step` when increasing/decreasing the value via the buttons, the final value will always round to the `max` or `min` rather than the closest valid amount.

- **suffix**

  **string**

  **Default: ''**

  A value to be displayed immediately after the editable portion of the field.

  This is useful for displaying an implied part of the value, such as "@shopify.com", or "%".

  This cannot be edited by the user, and it isn't included in the value of the field.

  It may not be displayed until the user has interacted with the input. For example, an inline label may take the place of the suffix until the user focuses the input.

- **value**

  **string**

  The current value for the field. If omitted, the field will be empty.

### NumberAutocompleteField

```ts
'one-time-code' | 'cc-number' | 'cc-csc'
```

## Events

Learn more about [registering events](https://shopify.dev/docs/api/app-home/using-polaris-components#event-handling).

- **blur**

  **CallbackEventListener<'input'>**

- **change**

  **CallbackEventListener<'input'>**

- **focus**

  **CallbackEventListener<'input'>**

- **input**

  **CallbackEventListener<'input'>**

### CallbackEventListener

```ts
(EventListener & {
      (event: CallbackEvent<T>): void;
    }) | null
```

### CallbackEvent

```ts
Event & {
  currentTarget: HTMLElementTagNameMap[T];
}
```

Examples

### Examples

- #### Code

  ##### jsx

  ```jsx
  <s-number-field
    label="Quantity"
    details="Number of items in stock"
    placeholder="0"
    step={5}
    min={0}
    max={100}
  />
  ```

  ##### html

  ```html
  <s-number-field
    label="Quantity"
    details="Number of items in stock"
    placeholder="0"
    step="5"
    min="0"
    max="100"
  ></s-number-field>
  ```

- #### Basic usage

  ##### Description

  Demonstrates a simple number field for entering order quantity with a predefined range and step value.

  ##### jsx

  ```jsx
  <s-number-field
    label="Order quantity"
    value="5"
    min={1}
    max={999}
    step={1}
   />
  ```

  ##### html

  ```html
  <s-number-field
    label="Order quantity"
    value="5"
    min="1"
    max="999"
    step="1"
  ></s-number-field>
  ```

- #### With prefix and suffix

  ##### Description

  Illustrates a number field for entering product prices with currency prefix and suffix, using decimal input mode.

  ##### jsx

  ```jsx
  <s-number-field
    label="Product price"
    value="29.99"
    prefix="$"
    suffix="CAD"
    inputMode="decimal"
    step={0.01}
    min={0}
   />
  ```

  ##### html

  ```html
  <s-number-field
    label="Product price"
    value="29.99"
    prefix="$"
    suffix="CAD"
    inputMode="decimal"
    step="0.01"
    min="0"
  ></s-number-field>
  ```

- #### Multiple examples

  ##### Description

  Showcases multiple number fields for different use cases: inventory tracking, percentage discount, and shipping weight, demonstrating various input modes and configurations.

  ##### jsx

  ```jsx
  <s-stack gap="base">
    <s-number-field
      label="Inventory count"
      value="150"
      min={0}
      step={1}
      inputMode="numeric"
      details="Current stock available for sale"
     />

    <s-number-field
      label="Discount percentage"
      value="15"
      suffix="%"
      min={0}
      max={100}
      step={0.1}
      inputMode="decimal"
     />

    <s-number-field
      label="Shipping weight"
      value="2.5"
      suffix="kg"
      min={0.1}
      step={0.1}
      inputMode="decimal"
     />
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack gap="base">
    <s-number-field
      label="Inventory count"
      value="150"
      min="0"
      step="1"
      inputMode="numeric"
      details="Current stock available for sale"
    ></s-number-field>

    <s-number-field
      label="Discount percentage"
      value="15"
      suffix="%"
      min="0"
      max="100"
      step="0.1"
      inputMode="decimal"
    ></s-number-field>

    <s-number-field
      label="Shipping weight"
      value="2.5"
      suffix="kg"
      min="0.1"
      step="0.1"
      inputMode="decimal"
    ></s-number-field>
  </s-stack>
  ```

</page>

<page>
---
title: PasswordField
description: Securely collect sensitive information from users.
api_name: app-home
source_url:
  html: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/forms/passwordfield
  md: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/forms/passwordfield.md
---

# Password​Field

Securely collect sensitive information from users.

## Properties

- **autocomplete**

  **"on" | "off" | PasswordAutocompleteField | \`section-${string} current-password\` | \`section-${string} new-password\` | "shipping current-password" | "shipping new-password" | "billing current-password" | "billing new-password" | \`section-${string} shipping current-password\` | \`section-${string} shipping new-password\` | \`section-${string} billing current-password\` | \`section-${string} billing new-password\`**

  **Default: 'on' for everything else**

  A hint as to the intended content of the field.

  When set to `on` (the default), this property indicates that the field should support autofill, but you do not have any more semantic information on the intended contents.

  When set to `off`, you are indicating that this field contains sensitive information, or contents that are never saved, like one-time codes.

  Alternatively, you can provide value which describes the specific data you would like to be entered into this field during autofill.

- **defaultValue**

  **string**

  The default value for the field.

- **details**

  **string**

  Additional text to provide context or guidance for the field. This text is displayed along with the field and its label to offer more information or instructions to the user.

  This will also be exposed to screen reader users.

- **disabled**

  **boolean**

  **Default: false**

  Disables the field, disallowing any interaction.

- **error**

  **string**

  Indicate an error to the user. The field will be given a specific stylistic treatment to communicate problems that have to be resolved immediately.

- **id**

  **string**

  A unique identifier for the element.

- **label**

  **string**

  Content to use as the field label.

- **labelAccessibilityVisibility**

  **"visible" | "exclusive"**

  **Default: 'visible'**

  Changes the visibility of the component's label.

  - `visible`: the label is visible to all users.
  - `exclusive`: the label is visually hidden but remains in the accessibility tree.

- **maxLength**

  **number**

  **Default: Infinity**

  Specifies the maximum number of characters allowed.

- **minLength**

  **number**

  **Default: 0**

  Specifies the min number of characters allowed.

- **name**

  **string**

  An identifier for the field that is unique within the nearest containing form.

- **placeholder**

  **string**

  A short hint that describes the expected value of the field.

- **readOnly**

  **boolean**

  **Default: false**

  The field cannot be edited by the user. It is focusable will be announced by screen readers.

- **required**

  **boolean**

  **Default: false**

  Whether the field needs a value. This requirement adds semantic value to the field, but it will not cause an error to appear automatically. If you want to present an error when this field is empty, you can do so with the `error` property.

- **value**

  **string**

  The current value for the field. If omitted, the field will be empty.

### PasswordAutocompleteField

```ts
'current-password' | 'new-password'
```

## Events

Learn more about [registering events](https://shopify.dev/docs/api/app-home/using-polaris-components#event-handling).

- **blur**

  **CallbackEventListener<'input'>**

- **change**

  **CallbackEventListener<'input'>**

- **focus**

  **CallbackEventListener<'input'>**

- **input**

  **CallbackEventListener<'input'>**

### CallbackEventListener

```ts
(EventListener & {
      (event: CallbackEvent<T>): void;
    }) | null
```

### CallbackEvent

```ts
Event & {
  currentTarget: HTMLElementTagNameMap[T];
}
```

Examples

### Examples

- #### Code

  ##### jsx

  ```jsx
  <s-password-field
    label="Password"
    placeholder="Enter your password"
    details="Must be at least 8 characters long"
    minLength={8}
   />
  ```

  ##### html

  ```html
  <s-password-field
    label="Password"
    placeholder="Enter your password"
    details="Must be at least 8 characters long"
    minLength="8"
  ></s-password-field>
  ```

- #### Basic usage

  ##### Description

  Demonstrates a basic password field with a label, name, and required validation. Sets a minimum length of 8 characters and configures autocomplete for a new password.

  ##### jsx

  ```jsx
  <s-password-field
    label="Password"
    name="password"
    required
    minLength={8}
    autocomplete="new-password"
  />
  ```

  ##### html

  ```html
  <s-password-field
    label="Password"
    name="password"
    required
    minLength="8"
    autocomplete="new-password"
  ></s-password-field>
  ```

- #### With error state

  ##### Description

  Shows a password field in an error state, displaying a custom error message when the password does not meet the minimum length requirement.

  ##### jsx

  ```jsx
  <s-password-field
    label="Password"
    name="password"
    error="Password must be at least 8 characters"
    minLength={8}
    autocomplete="new-password"
   />
  ```

  ##### html

  ```html
  <s-password-field
    label="Password"
    name="password"
    error="Password must be at least 8 characters"
    minLength="8"
    autocomplete="new-password"
  ></s-password-field>
  ```

- #### With helper text

  ##### Description

  Illustrates a password field with additional details providing guidance about password creation requirements.

  ##### jsx

  ```jsx
  <s-password-field
    label="Create password"
    name="new-password"
    details="Password must be at least 8 characters and include uppercase, lowercase, and numbers"
    minLength={8}
    autocomplete="new-password"
   />
  ```

  ##### html

  ```html
  <s-password-field
    label="Create password"
    name="new-password"
    details="Password must be at least 8 characters and include uppercase, lowercase, and numbers"
    minLength="8"
    autocomplete="new-password"
  ></s-password-field>
  ```

- #### In form layout

  ##### Description

  Shows how the password field can be integrated into a form alongside other input fields, such as an email field, to create a complete login or registration form.

  ##### jsx

  ```jsx
  <s-stack gap="base">
    <s-email-field
      label="Email"
      name="email"
      autocomplete="email"
      required
     />
    <s-password-field
      label="Password"
      name="password"
      autocomplete="current-password"
      required
     />
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack gap="base">
    <s-email-field
      label="Email"
      name="email"
      autocomplete="username"
      required
    ></s-email-field>
    <s-password-field
      label="Password"
      name="password"
      autocomplete="current-password"
      required
    ></s-password-field>
  </s-stack>
  ```

- #### With password strength requirements

  ##### Description

  Demonstrates a password field with dynamic password strength validation, showing real-time feedback on password complexity requirements.

  ##### jsx

  ```jsx
  <s-stack gap="large-100">
    <s-password-field
      label="Create password"
      name="password"
      value="example-password"
      autocomplete="new-password"
      required
     />
    <s-stack gap="small-200">
      <s-text tone="success">✓ At least 8 characters</s-text>
      <s-text color="subdued">○ Contains uppercase letter</s-text>
      <s-text color="subdued">○ Contains lowercase letter</s-text>
      <s-text color="subdued">○ Contains number</s-text>
    </s-stack>
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack gap="large-100">
    <s-password-field
      label="Create password"
      name="password"
      value="example-password"
      autocomplete="new-password"
      required
    ></s-password-field>
    <s-stack gap="small-200">
      <s-text tone="success">✓ At least 8 characters</s-text>
      <s-text color="subdued">○ Contains uppercase letter</s-text>
      <s-text color="subdued">○ Contains lowercase letter</s-text>
      <s-text color="subdued">○ Contains number</s-text>
    </s-stack>
  </s-stack>
  ```

</page>

<page>
---
title: SearchField
description: >-
  Let users enter search terms or find specific items using a single-line input
  field.
api_name: app-home
source_url:
  html: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/forms/searchfield
  md: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/forms/searchfield.md
---

# Search​Field

Let users enter search terms or find specific items using a single-line input field.

## SearchField

A search input field that allows users to enter a search term.

- **autocomplete**

  **"on" | "off" | TextAutocompleteField | \`section-${string} one-time-code\` | "shipping one-time-code" | "billing one-time-code" | \`section-${string} shipping one-time-code\` | \`section-${string} billing one-time-code\` | \`section-${string} language\` | \`section-${string} organization\` | \`section-${string} additional-name\` | \`section-${string} address-level1\` | \`section-${string} address-level2\` | \`section-${string} address-level3\` | \`section-${string} address-level4\` | \`section-${string} address-line1\` | \`section-${string} address-line2\` | \`section-${string} address-line3\` | \`section-${string} country-name\` | \`section-${string} country\` | \`section-${string} family-name\` | \`section-${string} given-name\` | \`section-${string} honorific-prefix\` | \`section-${string} honorific-suffix\` | \`section-${string} name\` | \`section-${string} nickname\` | \`section-${string} organization-title\` | \`section-${string} postal-code\` | \`section-${string} sex\` | \`section-${string} street-address\` | \`section-${string} transaction-currency\` | \`section-${string} username\` | \`section-${string} cc-additional-name\` | \`section-${string} cc-family-name\` | \`section-${string} cc-given-name\` | \`section-${string} cc-name\` | \`section-${string} cc-type\` | "shipping language" | "shipping organization" | "shipping additional-name" | "shipping address-level1" | "shipping address-level2" | "shipping address-level3" | "shipping address-level4" | "shipping address-line1" | "shipping address-line2" | "shipping address-line3" | "shipping country-name" | "shipping country" | "shipping family-name" | "shipping given-name" | "shipping honorific-prefix" | "shipping honorific-suffix" | "shipping name" | "shipping nickname" | "shipping organization-title" | "shipping postal-code" | "shipping sex" | "shipping street-address" | "shipping transaction-currency" | "shipping username" | "shipping cc-additional-name" | "shipping cc-family-name" | "shipping cc-given-name" | "shipping cc-name" | "shipping cc-type" | "billing language" | "billing organization" | "billing additional-name" | "billing address-level1" | "billing address-level2" | "billing address-level3" | "billing address-level4" | "billing address-line1" | "billing address-line2" | "billing address-line3" | "billing country-name" | "billing country" | "billing family-name" | "billing given-name" | "billing honorific-prefix" | "billing honorific-suffix" | "billing name" | "billing nickname" | "billing organization-title" | "billing postal-code" | "billing sex" | "billing street-address" | "billing transaction-currency" | "billing username" | "billing cc-additional-name" | "billing cc-family-name" | "billing cc-given-name" | "billing cc-name" | "billing cc-type" | \`section-${string} shipping language\` | \`section-${string} shipping organization\` | \`section-${string} shipping additional-name\` | \`section-${string} shipping address-level1\` | \`section-${string} shipping address-level2\` | \`section-${string} shipping address-level3\` | \`section-${string} shipping address-level4\` | \`section-${string} shipping address-line1\` | \`section-${string} shipping address-line2\` | \`section-${string} shipping address-line3\` | \`section-${string} shipping country-name\` | \`section-${string} shipping country\` | \`section-${string} shipping family-name\` | \`section-${string} shipping given-name\` | \`section-${string} shipping honorific-prefix\` | \`section-${string} shipping honorific-suffix\` | \`section-${string} shipping name\` | \`section-${string} shipping nickname\` | \`section-${string} shipping organization-title\` | \`section-${string} shipping postal-code\` | \`section-${string} shipping sex\` | \`section-${string} shipping street-address\` | \`section-${string} shipping transaction-currency\` | \`section-${string} shipping username\` | \`section-${string} shipping cc-additional-name\` | \`section-${string} shipping cc-family-name\` | \`section-${string} shipping cc-given-name\` | \`section-${string} shipping cc-name\` | \`section-${string} shipping cc-type\` | \`section-${string} billing language\` | \`section-${string} billing organization\` | \`section-${string} billing additional-name\` | \`section-${string} billing address-level1\` | \`section-${string} billing address-level2\` | \`section-${string} billing address-level3\` | \`section-${string} billing address-level4\` | \`section-${string} billing address-line1\` | \`section-${string} billing address-line2\` | \`section-${string} billing address-line3\` | \`section-${string} billing country-name\` | \`section-${string} billing country\` | \`section-${string} billing family-name\` | \`section-${string} billing given-name\` | \`section-${string} billing honorific-prefix\` | \`section-${string} billing honorific-suffix\` | \`section-${string} billing name\` | \`section-${string} billing nickname\` | \`section-${string} billing organization-title\` | \`section-${string} billing postal-code\` | \`section-${string} billing sex\` | \`section-${string} billing street-address\` | \`section-${string} billing transaction-currency\` | \`section-${string} billing username\` | \`section-${string} billing cc-additional-name\` | \`section-${string} billing cc-family-name\` | \`section-${string} billing cc-given-name\` | \`section-${string} billing cc-name\` | \`section-${string} billing cc-type\`**

  **Default: 'on' for everything else**

  A hint as to the intended content of the field.

  When set to `on` (the default), this property indicates that the field should support autofill, but you do not have any more semantic information on the intended contents.

  When set to `off`, you are indicating that this field contains sensitive information, or contents that are never saved, like one-time codes.

  Alternatively, you can provide value which describes the specific data you would like to be entered into this field during autofill.

- **defaultValue**

  **string**

  The default value for the field.

- **details**

  **string**

  Additional text to provide context or guidance for the field. This text is displayed along with the field and its label to offer more information or instructions to the user.

  This will also be exposed to screen reader users.

- **disabled**

  **boolean**

  **Default: false**

  Disables the field, disallowing any interaction.

- **error**

  **string**

  Indicate an error to the user. The field will be given a specific stylistic treatment to communicate problems that have to be resolved immediately.

- **id**

  **string**

  A unique identifier for the element.

- **label**

  **string**

  Content to use as the field label.

- **labelAccessibilityVisibility**

  **"visible" | "exclusive"**

  **Default: 'visible'**

  Changes the visibility of the component's label.

  - `visible`: the label is visible to all users.
  - `exclusive`: the label is visually hidden but remains in the accessibility tree.

- **maxLength**

  **number**

  **Default: Infinity**

  Specifies the maximum number of characters allowed.

- **minLength**

  **number**

  **Default: 0**

  Specifies the min number of characters allowed.

- **name**

  **string**

  An identifier for the field that is unique within the nearest containing form.

- **placeholder**

  **string**

  A short hint that describes the expected value of the field.

- **readOnly**

  **boolean**

  **Default: false**

  The field cannot be edited by the user. It is focusable will be announced by screen readers.

- **required**

  **boolean**

  **Default: false**

  Whether the field needs a value. This requirement adds semantic value to the field, but it will not cause an error to appear automatically. If you want to present an error when this field is empty, you can do so with the `error` property.

- **value**

  **string**

  The current value for the field. If omitted, the field will be empty.

### TextAutocompleteField

```ts
'language' | 'organization' | 'additional-name' | 'address-level1' | 'address-level2' | 'address-level3' | 'address-level4' | 'address-line1' | 'address-line2' | 'address-line3' | 'country-name' | 'country' | 'family-name' | 'given-name' | 'honorific-prefix' | 'honorific-suffix' | 'name' | 'nickname' | 'one-time-code' | 'organization-title' | 'postal-code' | 'sex' | 'street-address' | 'transaction-currency' | 'username' | 'cc-additional-name' | 'cc-family-name' | 'cc-given-name' | 'cc-name' | 'cc-type'
```

## Events

Learn more about [registering events](https://shopify.dev/docs/api/app-home/using-polaris-components#event-handling).

- **blur**

  **CallbackEventListener<'input'>**

- **change**

  **CallbackEventListener<'input'>**

- **focus**

  **CallbackEventListener<'input'>**

- **input**

  **CallbackEventListener<'input'>**

### CallbackEventListener

```ts
(EventListener & {
      (event: CallbackEvent<T>): void;
    }) | null
```

### CallbackEvent

```ts
Event & {
  currentTarget: HTMLElementTagNameMap[T];
}
```

Examples

### Examples

- #### Code

  ##### jsx

  ```jsx
  <s-search-field
    label="Search"
    labelAccessibilityVisibility="exclusive"
    placeholder="Search items"
   />
  ```

  ##### html

  ```html
  <s-search-field
    label="Search"
    labelAccessibilityVisibility="exclusive"
    placeholder="Search items"
  ></s-search-field>
  ```

- #### Basic usage

  ##### Description

  Demonstrates a standard search input field for product discovery, with a clear label, name, and placeholder text to guide user interaction.

  ##### jsx

  ```jsx
  <s-search-field
    label="Search products"
    name="search"
    placeholder="Search products..."
   />
  ```

  ##### html

  ```html
  <s-search-field
    label="Search products"
    name="search"
    placeholder="Search products..."
  ></s-search-field>
  ```

- #### With error state

  ##### Description

  Illustrates how the search field handles and displays an error state when no results are found or when there's a search-related issue.

  ##### jsx

  ```jsx
  <s-search-field
    label="Search orders"
    name="orderSearch"
    error="No results found"
    value="xyz123"
   />
  ```

  ##### html

  ```html
  <s-search-field
    label="Search orders"
    name="orderSearch"
    error="No results found"
    value="xyz123"
  ></s-search-field>
  ```

- #### Disabled state

  ##### Description

  Demonstrates the appearance and behavior of a search field when it is disabled, preventing user interaction.

  ##### jsx

  ```jsx
  <s-search-field
    label="Search customers"
    name="customerSearch"
    disabled
    placeholder="Search is currently unavailable"
   />
  ```

  ##### html

  ```html
  <s-search-field
    label="Search customers"
    name="customerSearch"
    disabled
    placeholder="Search is currently unavailable"
  ></s-search-field>
  ```

- #### With character limits

  ##### Description

  Showcases a search field with minimum and maximum character length constraints, providing guidance on input requirements.

  ##### jsx

  ```jsx
  <s-search-field
    label="Search collections"
    name="collectionSearch"
    minLength={3}
    maxLength={50}
    placeholder="Enter at least 3 characters"
   />
  ```

  ##### html

  ```html
  <s-search-field
    label="Search collections"
    name="collectionSearch"
    minLength="3"
    maxLength="50"
    placeholder="Enter at least 3 characters"
  ></s-search-field>
  ```

## Best practices

- The SearchField automatically includes a clear button when text is entered, so you should not create your own clear button

</page>

<page>
---
title: Select
description: >-
  Allow users to pick one option from a menu. Ideal when presenting four or more
  choices to keep interfaces uncluttered.
api_name: app-home
source_url:
  html: 'https://shopify.dev/docs/api/app-home/polaris-web-components/forms/select'
  md: 'https://shopify.dev/docs/api/app-home/polaris-web-components/forms/select.md'
---

# Select

Allow users to pick one option from a menu. Ideal when presenting four or more choices to keep interfaces uncluttered.

## Properties

- **details**

  **string**

  Additional text to provide context or guidance for the field. This text is displayed along with the field and its label to offer more information or instructions to the user.

  This will also be exposed to screen reader users.

- **disabled**

  **boolean**

  **Default: false**

  Disables the field, disallowing any interaction.

- **error**

  **string**

  Indicate an error to the user. The field will be given a specific stylistic treatment to communicate problems that have to be resolved immediately.

- **icon**

  **"" | "replace" | "search" | "split" | "link" | "edit" | "product" | "variant" | "collection" | "select" | "info" | "incomplete" | "complete" | "color" | "money" | "order" | "code" | "adjust" | "affiliate" | "airplane" | "alert-bubble" | "alert-circle" | "alert-diamond" | "alert-location" | "alert-octagon" | "alert-octagon-filled" | "alert-triangle" | "alert-triangle-filled" | "align-horizontal-centers" | "app-extension" | "apps" | "archive" | "arrow-down" | "arrow-down-circle" | "arrow-down-right" | "arrow-left" | "arrow-left-circle" | "arrow-right" | "arrow-right-circle" | "arrow-up" | "arrow-up-circle" | "arrow-up-right" | "arrows-in-horizontal" | "arrows-out-horizontal" | "asterisk" | "attachment" | "automation" | "backspace" | "bag" | "bank" | "barcode" | "battery-low" | "bill" | "blank" | "blog" | "bolt" | "bolt-filled" | "book" | "book-open" | "bug" | "bullet" | "business-entity" | "button" | "button-press" | "calculator" | "calendar" | "calendar-check" | "calendar-compare" | "calendar-list" | "calendar-time" | "camera" | "camera-flip" | "caret-down" | "caret-left" | "caret-right" | "caret-up" | "cart" | "cart-abandoned" | "cart-discount" | "cart-down" | "cart-filled" | "cart-sale" | "cart-send" | "cart-up" | "cash-dollar" | "cash-euro" | "cash-pound" | "cash-rupee" | "cash-yen" | "catalog-product" | "categories" | "channels" | "channels-filled" | "chart-cohort" | "chart-donut" | "chart-funnel" | "chart-histogram-first" | "chart-histogram-first-last" | "chart-histogram-flat" | "chart-histogram-full" | "chart-histogram-growth" | "chart-histogram-last" | "chart-histogram-second-last" | "chart-horizontal" | "chart-line" | "chart-popular" | "chart-stacked" | "chart-vertical" | "chat" | "chat-new" | "chat-referral" | "check" | "check-circle" | "check-circle-filled" | "checkbox" | "chevron-down" | "chevron-down-circle" | "chevron-left" | "chevron-left-circle" | "chevron-right" | "chevron-right-circle" | "chevron-up" | "chevron-up-circle" | "circle" | "circle-dashed" | "clipboard" | "clipboard-check" | "clipboard-checklist" | "clock" | "clock-list" | "clock-revert" | "code-add" | "collection-featured" | "collection-list" | "collection-reference" | "color-none" | "compass" | "compose" | "confetti" | "connect" | "content" | "contract" | "corner-pill" | "corner-round" | "corner-square" | "credit-card" | "credit-card-cancel" | "credit-card-percent" | "credit-card-reader" | "credit-card-reader-chip" | "credit-card-reader-tap" | "credit-card-secure" | "credit-card-tap-chip" | "crop" | "currency-convert" | "cursor" | "cursor-banner" | "cursor-option" | "data-presentation" | "data-table" | "database" | "database-add" | "database-connect" | "delete" | "delivered" | "delivery" | "desktop" | "disabled" | "disabled-filled" | "discount" | "discount-add" | "discount-automatic" | "discount-code" | "discount-remove" | "dns-settings" | "dock-floating" | "dock-side" | "domain" | "domain-landing-page" | "domain-new" | "domain-redirect" | "download" | "drag-drop" | "drag-handle" | "drawer" | "duplicate" | "email" | "email-follow-up" | "email-newsletter" | "empty" | "enabled" | "enter" | "envelope" | "envelope-soft-pack" | "eraser" | "exchange" | "exit" | "export" | "external" | "eye-check-mark" | "eye-dropper" | "eye-dropper-list" | "eye-first" | "eyeglasses" | "fav" | "favicon" | "file" | "file-list" | "filter" | "filter-active" | "flag" | "flip-horizontal" | "flip-vertical" | "flower" | "folder" | "folder-add" | "folder-down" | "folder-remove" | "folder-up" | "food" | "foreground" | "forklift" | "forms" | "games" | "gauge" | "geolocation" | "gift" | "gift-card" | "git-branch" | "git-commit" | "git-repository" | "globe" | "globe-asia" | "globe-europe" | "globe-lines" | "globe-list" | "graduation-hat" | "grid" | "hashtag" | "hashtag-decimal" | "hashtag-list" | "heart" | "hide" | "hide-filled" | "home" | "home-filled" | "icons" | "identity-card" | "image" | "image-add" | "image-alt" | "image-explore" | "image-magic" | "image-none" | "image-with-text-overlay" | "images" | "import" | "in-progress" | "incentive" | "incoming" | "info-filled" | "inheritance" | "inventory" | "inventory-edit" | "inventory-list" | "inventory-transfer" | "inventory-updated" | "iq" | "key" | "keyboard" | "keyboard-filled" | "keyboard-hide" | "keypad" | "label-printer" | "language" | "language-translate" | "layout-block" | "layout-buy-button" | "layout-buy-button-horizontal" | "layout-buy-button-vertical" | "layout-column-1" | "layout-columns-2" | "layout-columns-3" | "layout-footer" | "layout-header" | "layout-logo-block" | "layout-popup" | "layout-rows-2" | "layout-section" | "layout-sidebar-left" | "layout-sidebar-right" | "lightbulb" | "link-list" | "list-bulleted" | "list-bulleted-filled" | "list-numbered" | "live" | "live-critical" | "live-none" | "location" | "location-none" | "lock" | "map" | "markets" | "markets-euro" | "markets-rupee" | "markets-yen" | "maximize" | "measurement-size" | "measurement-size-list" | "measurement-volume" | "measurement-volume-list" | "measurement-weight" | "measurement-weight-list" | "media-receiver" | "megaphone" | "mention" | "menu" | "menu-filled" | "menu-horizontal" | "menu-vertical" | "merge" | "metafields" | "metaobject" | "metaobject-list" | "metaobject-reference" | "microphone" | "microphone-muted" | "minimize" | "minus" | "minus-circle" | "mobile" | "money-none" | "money-split" | "moon" | "nature" | "note" | "note-add" | "notification" | "number-one" | "order-batches" | "order-draft" | "order-filled" | "order-first" | "order-fulfilled" | "order-repeat" | "order-unfulfilled" | "orders-status" | "organization" | "outdent" | "outgoing" | "package" | "package-cancel" | "package-fulfilled" | "package-on-hold" | "package-reassign" | "package-returned" | "page" | "page-add" | "page-attachment" | "page-clock" | "page-down" | "page-heart" | "page-list" | "page-reference" | "page-remove" | "page-report" | "page-up" | "pagination-end" | "pagination-start" | "paint-brush-flat" | "paint-brush-round" | "paper-check" | "partially-complete" | "passkey" | "paste" | "pause-circle" | "payment" | "payment-capture" | "payout" | "payout-dollar" | "payout-euro" | "payout-pound" | "payout-rupee" | "payout-yen" | "person" | "person-add" | "person-exit" | "person-filled" | "person-list" | "person-lock" | "person-remove" | "person-segment" | "personalized-text" | "phablet" | "phone" | "phone-down" | "phone-down-filled" | "phone-in" | "phone-out" | "pin" | "pin-remove" | "plan" | "play" | "play-circle" | "plus" | "plus-circle" | "plus-circle-down" | "plus-circle-filled" | "plus-circle-up" | "point-of-sale" | "point-of-sale-register" | "price-list" | "print" | "product-add" | "product-cost" | "product-filled" | "product-list" | "product-reference" | "product-remove" | "product-return" | "product-unavailable" | "profile" | "profile-filled" | "question-circle" | "question-circle-filled" | "radio-control" | "receipt" | "receipt-dollar" | "receipt-euro" | "receipt-folded" | "receipt-paid" | "receipt-pound" | "receipt-refund" | "receipt-rupee" | "receipt-yen" | "receivables" | "redo" | "referral-code" | "refresh" | "remove-background" | "reorder" | "replay" | "reset" | "return" | "reward" | "rocket" | "rotate-left" | "rotate-right" | "sandbox" | "save" | "savings" | "scan-qr-code" | "search-add" | "search-list" | "search-recent" | "search-resource" | "send" | "settings" | "share" | "shield-check-mark" | "shield-none" | "shield-pending" | "shield-person" | "shipping-label" | "shipping-label-cancel" | "shopcodes" | "slideshow" | "smiley-happy" | "smiley-joy" | "smiley-neutral" | "smiley-sad" | "social-ad" | "social-post" | "sort" | "sort-ascending" | "sort-descending" | "sound" | "sports" | "star" | "star-circle" | "star-filled" | "star-half" | "star-list" | "status" | "status-active" | "stop-circle" | "store" | "store-import" | "store-managed" | "store-online" | "sun" | "table" | "table-masonry" | "tablet" | "target" | "tax" | "team" | "text" | "text-align-center" | "text-align-left" | "text-align-right" | "text-block" | "text-bold" | "text-color" | "text-font" | "text-font-list" | "text-grammar" | "text-in-columns" | "text-in-rows" | "text-indent" | "text-indent-remove" | "text-italic" | "text-quote" | "text-title" | "text-underline" | "text-with-image" | "theme" | "theme-edit" | "theme-store" | "theme-template" | "three-d-environment" | "thumbs-down" | "thumbs-up" | "tip-jar" | "toggle-off" | "toggle-on" | "transaction" | "transaction-fee-add" | "transaction-fee-dollar" | "transaction-fee-euro" | "transaction-fee-pound" | "transaction-fee-rupee" | "transaction-fee-yen" | "transfer" | "transfer-in" | "transfer-internal" | "transfer-out" | "truck" | "undo" | "unknown-device" | "unlock" | "upload" | "variant-list" | "video" | "video-list" | "view" | "viewport-narrow" | "viewport-short" | "viewport-tall" | "viewport-wide" | "wallet" | "wand" | "watch" | "wifi" | "work" | "work-list" | "wrench" | "x" | "x-circle" | "x-circle-filled"**

  The type of icon to be displayed in the field.

- **id**

  **string**

  A unique identifier for the element.

- **label**

  **string**

  Content to use as the field label.

- **labelAccessibilityVisibility**

  **"visible" | "exclusive"**

  **Default: 'visible'**

  Changes the visibility of the component's label.

  - `visible`: the label is visible to all users.
  - `exclusive`: the label is visually hidden but remains in the accessibility tree.

- **name**

  **string**

  An identifier for the field that is unique within the nearest containing form.

- **placeholder**

  **string**

  A short hint that describes the expected value of the field.

- **required**

  **boolean**

  **Default: false**

  Whether the field needs a value. This requirement adds semantic value to the field, but it will not cause an error to appear automatically. If you want to present an error when this field is empty, you can do so with the `error` property.

- **value**

  **string**

  The current value for the field. If omitted, the field will be empty.

## Events

Learn more about [registering events](https://shopify.dev/docs/api/app-home/using-polaris-components#event-handling).

- **change**

  **CallbackEventListener<'input'>**

- **input**

  **CallbackEventListener<'input'>**

### CallbackEventListener

```ts
(EventListener & {
      (event: CallbackEvent<T>): void;
    }) | null
```

### CallbackEvent

```ts
Event & {
  currentTarget: HTMLElementTagNameMap[T];
}
```

## Slots

- **children**

  **HTMLElement**

  The options a user can select from.

  Accepts `Option` and `OptionGroup` components.

## Option

Represents a single option within a select component. Use only as a child of `s-select` components.

- **defaultSelected**

  **boolean**

  **Default: false**

  Whether the control is active by default.

- **disabled**

  **boolean**

  **Default: false**

  Disables the control, disallowing any interaction.

- **selected**

  **boolean**

  **Default: false**

  Whether the control is active.

- **value**

  **string**

  The value used in form data when the control is checked.

## Slots

- **children**

  **HTMLElement**

  The content to use as the label.

## OptionGroup

Represents a group of options within a select component. Use only as a child of `s-select` components.

- **disabled**

  **boolean**

  **Default: false**

  Whether the options within this group can be selected or not.

- **label**

  **string**

  The user-facing label for this group of options.

## Slots

- **children**

  **HTMLElement**

  The options a user can select from.

  Accepts `Option` components.

Examples

### Examples

- #### Code

  ##### jsx

  ```jsx
  <s-select label="Date range">
    <s-option value="1">Today</s-option>
    <s-option value="2">Yesterday</s-option>
    <s-option value="3">Last 7 days</s-option>
    <s-option-group label="Custom ranges">
      <s-option value="4">Last 30 days</s-option>
      <s-option value="5">Last 90 days</s-option>
    </s-option-group>
  </s-select>
  ```

  ##### html

  ```html
  <s-select label="Date range">
    <s-option value="1">Today</s-option>
    <s-option value="2">Yesterday</s-option>
    <s-option value="3">Last 7 days</s-option>
    <s-option-group label="Custom ranges">
      <s-option value="4">Last 30 days</s-option>
      <s-option value="5">Last 90 days</s-option>
    </s-option-group>
  </s-select>
  ```

- #### Basic usage

  ##### Description

  A simple select dropdown with pre-selected value for product sorting options.

  ##### jsx

  ```jsx
  <s-select label="Sort products by" value="newest">
    <s-option value="newest">Newest first</s-option>
    <s-option value="oldest">Oldest first</s-option>
    <s-option value="title">Title A–Z</s-option>
    <s-option value="price-low">Price: low to high</s-option>
    <s-option value="price-high">Price: high to low</s-option>
  </s-select>
  ```

  ##### html

  ```html
  <s-select label="Sort products by" value="newest">
    <s-option value="newest">Newest first</s-option>
    <s-option value="oldest">Oldest first</s-option>
    <s-option value="title">Title A–Z</s-option>
    <s-option value="price-low">Price: low to high</s-option>
    <s-option value="price-high">Price: high to low</s-option>
  </s-select>
  ```

- #### With placeholder

  ##### Description

  Select dropdown with helpful placeholder text guiding category selection.

  ##### jsx

  ```jsx
  <s-select
    label="Product category"
    placeholder="Choose category for better organization"
  >
    <s-option value="clothing">Clothing & apparel</s-option>
    <s-option value="accessories">Accessories & jewelry</s-option>
    <s-option value="home-garden">Home & garden</s-option>
    <s-option value="electronics">Electronics & tech</s-option>
    <s-option value="books">Books & media</s-option>
  </s-select>
  ```

  ##### html

  ```html
  <s-select
    label="Product category"
    placeholder="Choose category for better organization"
  >
    <s-option value="clothing">Clothing & apparel</s-option>
    <s-option value="accessories">Accessories & jewelry</s-option>
    <s-option value="home-garden">Home & garden</s-option>
    <s-option value="electronics">Electronics & tech</s-option>
    <s-option value="books">Books & media</s-option>
  </s-select>
  ```

- #### With error state

  ##### Description

  Select in error state showing specific business context and actionable error message.

  ##### jsx

  ```jsx
  <s-select
    label="Shipping origin"
    error="Select your primary shipping location to calculate accurate rates for customers"
    required
  >
    <s-option value="ca">Canada</s-option>
    <s-option value="us">United states</s-option>
    <s-option value="mx">Mexico</s-option>
    <s-option value="uk">United kingdom</s-option>
  </s-select>
  ```

  ##### html

  ```html
  <s-select
    label="Shipping origin"
    error="Select your primary shipping location to calculate accurate rates for customers"
    required
  >
    <s-option value="ca">Canada</s-option>
    <s-option value="us">United states</s-option>
    <s-option value="mx">Mexico</s-option>
    <s-option value="uk">United kingdom</s-option>
  </s-select>
  ```

- #### With option groups

  ##### Description

  Grouped select options organized by geographical regions for international shipping.

  ##### jsx

  ```jsx
  <s-select label="Shipping destination">
    <s-option-group label="North america">
      <s-option value="ca">Canada</s-option>
      <s-option value="us">United states</s-option>
      <s-option value="mx">Mexico</s-option>
    </s-option-group>
    <s-option-group label="Europe">
      <s-option value="uk">United kingdom</s-option>
      <s-option value="fr">France</s-option>
      <s-option value="de">Germany</s-option>
      <s-option value="it">Italy</s-option>
    </s-option-group>
    <s-option-group label="Asia pacific">
      <s-option value="au">Australia</s-option>
      <s-option value="jp">Japan</s-option>
      <s-option value="sg">Singapore</s-option>
    </s-option-group>
  </s-select>
  ```

  ##### html

  ```html
  <s-select label="Shipping destination">
    <s-option-group label="North america">
      <s-option value="ca">Canada</s-option>
      <s-option value="us">United states</s-option>
      <s-option value="mx">Mexico</s-option>
    </s-option-group>
    <s-option-group label="Europe">
      <s-option value="uk">United kingdom</s-option>
      <s-option value="fr">France</s-option>
      <s-option value="de">Germany</s-option>
      <s-option value="it">Italy</s-option>
    </s-option-group>
    <s-option-group label="Asia pacific">
      <s-option value="au">Australia</s-option>
      <s-option value="jp">Japan</s-option>
      <s-option value="sg">Singapore</s-option>
    </s-option-group>
  </s-select>
  ```

- #### With icon

  ##### Description

  Select dropdown with sort icon for filtering order management views.

  ##### jsx

  ```jsx
  <s-select label="Filter orders by" icon="sort">
    <s-option value="date">Order date</s-option>
    <s-option value="status">Fulfillment status</s-option>
    <s-option value="total">Order total</s-option>
    <s-option value="customer">Customer name</s-option>
  </s-select>
  ```

  ##### html

  ```html
  <s-select label="Filter orders by" icon="sort">
    <s-option value="date">Order date</s-option>
    <s-option value="status">Fulfillment status</s-option>
    <s-option value="total">Order total</s-option>
    <s-option value="customer">Customer name</s-option>
  </s-select>
  ```

- #### Disabled state

  ##### Description

  Select in disabled state preventing user interaction with pre-selected value.

  ##### jsx

  ```jsx
  <s-select label="Product type" disabled value="physical">
    <s-option value="physical">Physical product</s-option>
    <s-option value="digital">Digital product</s-option>
    <s-option value="service">Service</s-option>
    <s-option value="gift-card">Gift card</s-option>
  </s-select>
  ```

  ##### html

  ```html
  <s-select label="Product type" disabled value="physical">
    <s-option value="physical">Physical product</s-option>
    <s-option value="digital">Digital product</s-option>
    <s-option value="service">Service</s-option>
    <s-option value="gift-card">Gift card</s-option>
  </s-select>
  ```

</page>

<page>
---
title: Switch
description: Give users a clear way to toggle options on or off.
api_name: app-home
source_url:
  html: 'https://shopify.dev/docs/api/app-home/polaris-web-components/forms/switch'
  md: 'https://shopify.dev/docs/api/app-home/polaris-web-components/forms/switch.md'
---

# Switch

Give users a clear way to toggle options on or off.

## Properties

- **accessibilityLabel**

  **string**

  A label used for users using assistive technologies like screen readers. When set, any children or `label` supplied will not be announced. This can also be used to display a control without a visual label, while still providing context to users using screen readers.

- **checked**

  **boolean**

  **Default: false**

  Whether the control is active.

- **defaultChecked**

  **boolean**

  **Default: false**

  Whether the control is active by default.

- **details**

  **string**

  Additional text to provide context or guidance for the field. This text is displayed along with the field and its label to offer more information or instructions to the user.

  This will also be exposed to screen reader users.

- **disabled**

  **boolean**

  **Default: false**

  Disables the field, disallowing any interaction.

- **error**

  **string**

  Indicate an error to the user. The field will be given a specific stylistic treatment to communicate problems that have to be resolved immediately.

- **id**

  **string**

  A unique identifier for the element.

- **label**

  **string**

  Visual content to use as the control label.

- **labelAccessibilityVisibility**

  **"visible" | "exclusive"**

  **Default: 'visible'**

  Changes the visibility of the component's label.

  - `visible`: the label is visible to all users.
  - `exclusive`: the label is visually hidden but remains in the accessibility tree.

- **name**

  **string**

  An identifier for the field that is unique within the nearest containing form.

- **required**

  **boolean**

  **Default: false**

  Whether the field needs a value. This requirement adds semantic value to the field, but it will not cause an error to appear automatically. If you want to present an error when this field is empty, you can do so with the `error` property.

- **value**

  **string**

  The value used in form data when the checkbox is checked.

## Events

Learn more about [registering events](https://shopify.dev/docs/api/app-home/using-polaris-components#event-handling).

- **change**

  **CallbackEventListener<'input'>**

- **input**

  **CallbackEventListener<'input'>**

### CallbackEventListener

```ts
(EventListener & {
      (event: CallbackEvent<T>): void;
    }) | null
```

### CallbackEvent

```ts
Event & {
  currentTarget: HTMLElementTagNameMap[T];
}
```

Examples

### Examples

- #### Code

  ##### jsx

  ```jsx
  <s-switch
    label="Enable feature"
    details="Ensure all criteria are met before enabling"
   />
  ```

  ##### html

  ```html
  <s-switch
    label="Enable feature"
    details="Ensure all criteria are met before enabling"
  ></s-switch>
  ```

- #### Basic switch

  ##### Description

  Standard toggle switch for enabling or disabling merchant preferences. This example demonstrates a simple switch with a label, allowing users to toggle a single setting on or off.

  ##### jsx

  ```jsx
  <s-switch id="basic-switch" label="Enable notifications" />
  ```

  ##### html

  ```html
  <s-switch id="basic-switch" label="Enable notifications"></s-switch>
  ```

- #### Disabled switch

  ##### Description

  Locked switch with explanatory text for unavailable premium features. This example shows a switch that is visually disabled and cannot be interacted with, typically used to indicate a feature is not currently available.

  ##### jsx

  ```jsx
  <s-switch
    id="disabled-switch"
    label="Feature locked (Premium plan required)"
    checked={true}
    disabled={true}
   />
  ```

  ##### html

  ```html
  <s-switch
    id="disabled-switch"
    label="Feature locked (Premium plan required)"
    checked="true"
    disabled="true"
  ></s-switch>
  ```

- #### Form integration

  ##### Description

  Multiple switches within a form for notification preferences submission. This example illustrates how switches can be used together in a form to allow users to configure multiple related settings simultaneously.

  ##### jsx

  ```jsx
  <form>
    <s-switch
      id="email-notifications"
      label="Email notifications"
      name="emailNotifications"
      value="enabled"
     />
    <s-switch
      id="sms-notifications"
      label="SMS notifications"
      name="smsNotifications"
      value="enabled"
     />
  </form>
  ```

  ##### html

  ```html
  <form>
    <s-switch
      id="email-notifications"
      label="Email notifications"
      name="emailNotifications"
      value="enabled"
    ></s-switch>
    <s-switch
      id="sms-notifications"
      label="SMS notifications"
      name="smsNotifications"
      value="enabled"
    ></s-switch>
  </form>
  ```

- #### Hidden label for accessibility

  ##### Description

  Switch with visually hidden label that remains accessible to screen readers. This example demonstrates how to create a switch with a label that is only perceivable by assistive technologies, maintaining accessibility while minimizing visual clutter.

  ##### jsx

  ```jsx
  <s-switch
    id="hidden-label-switch"
    labelAccessibilityVisibility="exclusive"
    label="Toggle feature"
    checked={true}
   />
  ```

  ##### html

  ```html
  <s-switch
    id="hidden-label-switch"
    labelAccessibilityVisibility="exclusive"
    label="Toggle feature"
    checked="true"
  ></s-switch>
  ```

- #### With details and error

  ##### Description

  Required switch with validation error and contextual details for user guidance. This example shows a switch that requires user interaction, provides additional context through details, and displays an error message when validation fails.

  ##### jsx

  ```jsx
  <s-switch
    id="terms-switch"
    label="Agree to terms and conditions"
    details="You must agree to continue with the purchase"
    error="Agreement is required"
    name="termsAgreement"
    required={true}
    value="agreed"
   />
  ```

  ##### html

  ```html
  <s-switch
    id="terms-switch"
    label="Agree to terms and conditions"
    details="You must agree to continue with the purchase"
    error="Agreement is required"
    name="termsAgreement"
    required="true"
    value="agreed"
  ></s-switch>
  ```

- #### Switch with accessibility label

  ##### Description

  Switch with enhanced accessibility description for screen reader users. This example illustrates how to provide a more descriptive accessibility label that provides additional context beyond the visible label.

  ##### jsx

  ```jsx
  <s-switch
    id="event-switch"
    label="Feature toggle"
    accessibilityLabel="Toggle feature on or off"
   />
  ```

  ##### html

  ```html
  <s-switch
    id="event-switch"
    label="Feature toggle"
    accessibilityLabel="Toggle feature on or off"
  ></s-switch>
  ```

- #### Settings panel with Stack

  ##### Description

  Group of related switches arranged in a vertical stack for settings configuration. This example demonstrates how to use the Stack component to create a clean, organized layout for multiple related switch settings.

  ##### jsx

  ```jsx
  <s-stack gap="small-200">
    <s-switch id="notifications-setting" label="Push notifications" />
    <s-switch id="autosave-setting" label="Auto-save drafts" />
    <s-switch
      id="analytics-setting"
      label="Usage analytics"
      checked={true}
     />
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack gap="base">
    <s-switch id="notifications-setting" label="Push notifications"></s-switch>
    <s-switch id="autosave-setting" label="Auto-save drafts"></s-switch>
    <s-switch
      id="analytics-setting"
      label="Usage analytics"
      checked="true"
    ></s-switch>
  </s-stack>
  ```

</page>

<page>
---
title: TextArea
description: >-
  Collect longer text content from users with a multi-line input that expands
  automatically.
api_name: app-home
source_url:
  html: 'https://shopify.dev/docs/api/app-home/polaris-web-components/forms/textarea'
  md: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/forms/textarea.md
---

# Text​Area

Collect longer text content from users with a multi-line input that expands automatically.

## Properties

- **autocomplete**

  **"on" | "off" | TextAutocompleteField | \`section-${string} one-time-code\` | "shipping one-time-code" | "billing one-time-code" | \`section-${string} shipping one-time-code\` | \`section-${string} billing one-time-code\` | \`section-${string} language\` | \`section-${string} organization\` | \`section-${string} additional-name\` | \`section-${string} address-level1\` | \`section-${string} address-level2\` | \`section-${string} address-level3\` | \`section-${string} address-level4\` | \`section-${string} address-line1\` | \`section-${string} address-line2\` | \`section-${string} address-line3\` | \`section-${string} country-name\` | \`section-${string} country\` | \`section-${string} family-name\` | \`section-${string} given-name\` | \`section-${string} honorific-prefix\` | \`section-${string} honorific-suffix\` | \`section-${string} name\` | \`section-${string} nickname\` | \`section-${string} organization-title\` | \`section-${string} postal-code\` | \`section-${string} sex\` | \`section-${string} street-address\` | \`section-${string} transaction-currency\` | \`section-${string} username\` | \`section-${string} cc-additional-name\` | \`section-${string} cc-family-name\` | \`section-${string} cc-given-name\` | \`section-${string} cc-name\` | \`section-${string} cc-type\` | "shipping language" | "shipping organization" | "shipping additional-name" | "shipping address-level1" | "shipping address-level2" | "shipping address-level3" | "shipping address-level4" | "shipping address-line1" | "shipping address-line2" | "shipping address-line3" | "shipping country-name" | "shipping country" | "shipping family-name" | "shipping given-name" | "shipping honorific-prefix" | "shipping honorific-suffix" | "shipping name" | "shipping nickname" | "shipping organization-title" | "shipping postal-code" | "shipping sex" | "shipping street-address" | "shipping transaction-currency" | "shipping username" | "shipping cc-additional-name" | "shipping cc-family-name" | "shipping cc-given-name" | "shipping cc-name" | "shipping cc-type" | "billing language" | "billing organization" | "billing additional-name" | "billing address-level1" | "billing address-level2" | "billing address-level3" | "billing address-level4" | "billing address-line1" | "billing address-line2" | "billing address-line3" | "billing country-name" | "billing country" | "billing family-name" | "billing given-name" | "billing honorific-prefix" | "billing honorific-suffix" | "billing name" | "billing nickname" | "billing organization-title" | "billing postal-code" | "billing sex" | "billing street-address" | "billing transaction-currency" | "billing username" | "billing cc-additional-name" | "billing cc-family-name" | "billing cc-given-name" | "billing cc-name" | "billing cc-type" | \`section-${string} shipping language\` | \`section-${string} shipping organization\` | \`section-${string} shipping additional-name\` | \`section-${string} shipping address-level1\` | \`section-${string} shipping address-level2\` | \`section-${string} shipping address-level3\` | \`section-${string} shipping address-level4\` | \`section-${string} shipping address-line1\` | \`section-${string} shipping address-line2\` | \`section-${string} shipping address-line3\` | \`section-${string} shipping country-name\` | \`section-${string} shipping country\` | \`section-${string} shipping family-name\` | \`section-${string} shipping given-name\` | \`section-${string} shipping honorific-prefix\` | \`section-${string} shipping honorific-suffix\` | \`section-${string} shipping name\` | \`section-${string} shipping nickname\` | \`section-${string} shipping organization-title\` | \`section-${string} shipping postal-code\` | \`section-${string} shipping sex\` | \`section-${string} shipping street-address\` | \`section-${string} shipping transaction-currency\` | \`section-${string} shipping username\` | \`section-${string} shipping cc-additional-name\` | \`section-${string} shipping cc-family-name\` | \`section-${string} shipping cc-given-name\` | \`section-${string} shipping cc-name\` | \`section-${string} shipping cc-type\` | \`section-${string} billing language\` | \`section-${string} billing organization\` | \`section-${string} billing additional-name\` | \`section-${string} billing address-level1\` | \`section-${string} billing address-level2\` | \`section-${string} billing address-level3\` | \`section-${string} billing address-level4\` | \`section-${string} billing address-line1\` | \`section-${string} billing address-line2\` | \`section-${string} billing address-line3\` | \`section-${string} billing country-name\` | \`section-${string} billing country\` | \`section-${string} billing family-name\` | \`section-${string} billing given-name\` | \`section-${string} billing honorific-prefix\` | \`section-${string} billing honorific-suffix\` | \`section-${string} billing name\` | \`section-${string} billing nickname\` | \`section-${string} billing organization-title\` | \`section-${string} billing postal-code\` | \`section-${string} billing sex\` | \`section-${string} billing street-address\` | \`section-${string} billing transaction-currency\` | \`section-${string} billing username\` | \`section-${string} billing cc-additional-name\` | \`section-${string} billing cc-family-name\` | \`section-${string} billing cc-given-name\` | \`section-${string} billing cc-name\` | \`section-${string} billing cc-type\`**

  **Default: 'on' for everything else**

  A hint as to the intended content of the field.

  When set to `on` (the default), this property indicates that the field should support autofill, but you do not have any more semantic information on the intended contents.

  When set to `off`, you are indicating that this field contains sensitive information, or contents that are never saved, like one-time codes.

  Alternatively, you can provide value which describes the specific data you would like to be entered into this field during autofill.

- **defaultValue**

  **string**

  The default value for the field.

- **details**

  **string**

  Additional text to provide context or guidance for the field. This text is displayed along with the field and its label to offer more information or instructions to the user.

  This will also be exposed to screen reader users.

- **disabled**

  **boolean**

  **Default: false**

  Disables the field, disallowing any interaction.

- **error**

  **string**

  Indicate an error to the user. The field will be given a specific stylistic treatment to communicate problems that have to be resolved immediately.

- **id**

  **string**

  A unique identifier for the element.

- **label**

  **string**

  Content to use as the field label.

- **labelAccessibilityVisibility**

  **"visible" | "exclusive"**

  **Default: 'visible'**

  Changes the visibility of the component's label.

  - `visible`: the label is visible to all users.
  - `exclusive`: the label is visually hidden but remains in the accessibility tree.

- **maxLength**

  **number**

  **Default: Infinity**

  Specifies the maximum number of characters allowed.

- **minLength**

  **number**

  **Default: 0**

  Specifies the min number of characters allowed.

- **name**

  **string**

  An identifier for the field that is unique within the nearest containing form.

- **placeholder**

  **string**

  A short hint that describes the expected value of the field.

- **readOnly**

  **boolean**

  **Default: false**

  The field cannot be edited by the user. It is focusable will be announced by screen readers.

- **required**

  **boolean**

  **Default: false**

  Whether the field needs a value. This requirement adds semantic value to the field, but it will not cause an error to appear automatically. If you want to present an error when this field is empty, you can do so with the `error` property.

- **rows**

  **number**

  **Default: 2**

  A number of visible text lines.

- **value**

  **string**

  The current value for the field. If omitted, the field will be empty.

### TextAutocompleteField

```ts
'language' | 'organization' | 'additional-name' | 'address-level1' | 'address-level2' | 'address-level3' | 'address-level4' | 'address-line1' | 'address-line2' | 'address-line3' | 'country-name' | 'country' | 'family-name' | 'given-name' | 'honorific-prefix' | 'honorific-suffix' | 'name' | 'nickname' | 'one-time-code' | 'organization-title' | 'postal-code' | 'sex' | 'street-address' | 'transaction-currency' | 'username' | 'cc-additional-name' | 'cc-family-name' | 'cc-given-name' | 'cc-name' | 'cc-type'
```

## Events

Learn more about [registering events](https://shopify.dev/docs/api/app-home/using-polaris-components#event-handling).

- **blur**

  **CallbackEventListener<'input'>**

- **change**

  **CallbackEventListener<'input'>**

- **focus**

  **CallbackEventListener<'input'>**

- **input**

  **CallbackEventListener<'input'>**

### CallbackEventListener

```ts
(EventListener & {
      (event: CallbackEvent<T>): void;
    }) | null
```

### CallbackEvent

```ts
Event & {
  currentTarget: HTMLElementTagNameMap[T];
}
```

Examples

### Examples

- #### Code

  ##### jsx

  ```jsx
  <s-text-area
    label="Shipping address"
    value="1776 Barnes Street, Orlando, FL 32801"
    rows={3}
   />
  ```

  ##### html

  ```html
  <s-text-area
    label="Shipping address"
    value="1776 Barnes Street, Orlando, FL 32801"
    rows="3"
  ></s-text-area>
  ```

- #### Basic usage

  ##### Description

  Demonstrates a simple text area for collecting product descriptions with a placeholder and autocomplete disabled.

  ##### jsx

  ```jsx
  <s-text-area
    label="Product description"
    placeholder="Enter a detailed description..."
    autocomplete="off"
   />
  ```

  ##### html

  ```html
  <s-text-area
    label="Product description"
    placeholder="Enter a detailed description..."
    autocomplete="off"
  ></s-text-area>
  ```

- #### Seo meta description with character limit

  ##### Description

  Showcases a text area for writing SEO meta descriptions with a character limit of 160, providing guidance text and a multi-line input.

  ##### jsx

  ```jsx
  <s-text-area
    label="Meta description"
    max-length={160}
    details="Appears in search results. Keep under 160 characters for best visibility."
    placeholder="Write a compelling description that will appear in Google search results..."
    rows={3}
    autocomplete="off"
   />
  ```

  ##### html

  ```html
  <s-text-area
    label="Meta description"
    max-length="160"
    details="Appears in search results. Keep under 160 characters for best visibility."
    placeholder="Write a compelling description that will appear in Google search results..."
    rows="3"
    autocomplete="off"
  ></s-text-area>
  ```

- #### With error state

  ##### Description

  Demonstrates the error state of a text area with a minimum length requirement, showing how validation errors are displayed in a commerce context.

  ##### jsx

  ```jsx
  <s-text-area
    label="Reason for return"
    error="Please provide a detailed explanation for the return request. This helps us improve our products and process the refund faster."
    minLength={20}
    placeholder="Explain why the customer is returning this item..."
    rows={3}
    autocomplete="off"
   />
  ```

  ##### html

  ```html
  <s-text-area
    label="Reason for return"
    error="Please provide a detailed explanation for the return request. This helps us improve our products and process the refund faster."
    minLength="20"
    placeholder="Explain why the customer is returning this item..."
    rows="3"
    autocomplete="off"
  ></s-text-area>
  ```

- #### Product care instructions

  ##### Description

  Illustrates a text area for entering detailed product care instructions, with an expanded height and supporting guidance text.

  ##### jsx

  ```jsx
  <s-text-area
    label="Care instructions"
    rows={6}
    details="Detailed care instructions help customers maintain their purchases and reduce returns."
    placeholder="Provide washing, storage, and maintenance instructions..."
    autocomplete="off"
   />
  ```

  ##### html

  ```html
  <s-text-area
    label="Care instructions"
    rows="6"
    details="Detailed care instructions help customers maintain their purchases and reduce returns."
    placeholder="Provide washing, storage, and maintenance instructions..."
    autocomplete="off"
  ></s-text-area>
  ```

</page>

<page>
---
title: TextField
description: >-
  Lets users enter or edit text within a single-line input. Use to collect
  short, free-form information from users.
api_name: app-home
source_url:
  html: 'https://shopify.dev/docs/api/app-home/polaris-web-components/forms/textfield'
  md: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/forms/textfield.md
---

# Text​Field

Lets users enter or edit text within a single-line input. Use to collect short, free-form information from users.

## TextField

A text input field that allows users to enter and edit text.

- **autocomplete**

  **"on" | "off" | TextAutocompleteField | \`section-${string} one-time-code\` | "shipping one-time-code" | "billing one-time-code" | \`section-${string} shipping one-time-code\` | \`section-${string} billing one-time-code\` | \`section-${string} language\` | \`section-${string} organization\` | \`section-${string} additional-name\` | \`section-${string} address-level1\` | \`section-${string} address-level2\` | \`section-${string} address-level3\` | \`section-${string} address-level4\` | \`section-${string} address-line1\` | \`section-${string} address-line2\` | \`section-${string} address-line3\` | \`section-${string} country-name\` | \`section-${string} country\` | \`section-${string} family-name\` | \`section-${string} given-name\` | \`section-${string} honorific-prefix\` | \`section-${string} honorific-suffix\` | \`section-${string} name\` | \`section-${string} nickname\` | \`section-${string} organization-title\` | \`section-${string} postal-code\` | \`section-${string} sex\` | \`section-${string} street-address\` | \`section-${string} transaction-currency\` | \`section-${string} username\` | \`section-${string} cc-additional-name\` | \`section-${string} cc-family-name\` | \`section-${string} cc-given-name\` | \`section-${string} cc-name\` | \`section-${string} cc-type\` | "shipping language" | "shipping organization" | "shipping additional-name" | "shipping address-level1" | "shipping address-level2" | "shipping address-level3" | "shipping address-level4" | "shipping address-line1" | "shipping address-line2" | "shipping address-line3" | "shipping country-name" | "shipping country" | "shipping family-name" | "shipping given-name" | "shipping honorific-prefix" | "shipping honorific-suffix" | "shipping name" | "shipping nickname" | "shipping organization-title" | "shipping postal-code" | "shipping sex" | "shipping street-address" | "shipping transaction-currency" | "shipping username" | "shipping cc-additional-name" | "shipping cc-family-name" | "shipping cc-given-name" | "shipping cc-name" | "shipping cc-type" | "billing language" | "billing organization" | "billing additional-name" | "billing address-level1" | "billing address-level2" | "billing address-level3" | "billing address-level4" | "billing address-line1" | "billing address-line2" | "billing address-line3" | "billing country-name" | "billing country" | "billing family-name" | "billing given-name" | "billing honorific-prefix" | "billing honorific-suffix" | "billing name" | "billing nickname" | "billing organization-title" | "billing postal-code" | "billing sex" | "billing street-address" | "billing transaction-currency" | "billing username" | "billing cc-additional-name" | "billing cc-family-name" | "billing cc-given-name" | "billing cc-name" | "billing cc-type" | \`section-${string} shipping language\` | \`section-${string} shipping organization\` | \`section-${string} shipping additional-name\` | \`section-${string} shipping address-level1\` | \`section-${string} shipping address-level2\` | \`section-${string} shipping address-level3\` | \`section-${string} shipping address-level4\` | \`section-${string} shipping address-line1\` | \`section-${string} shipping address-line2\` | \`section-${string} shipping address-line3\` | \`section-${string} shipping country-name\` | \`section-${string} shipping country\` | \`section-${string} shipping family-name\` | \`section-${string} shipping given-name\` | \`section-${string} shipping honorific-prefix\` | \`section-${string} shipping honorific-suffix\` | \`section-${string} shipping name\` | \`section-${string} shipping nickname\` | \`section-${string} shipping organization-title\` | \`section-${string} shipping postal-code\` | \`section-${string} shipping sex\` | \`section-${string} shipping street-address\` | \`section-${string} shipping transaction-currency\` | \`section-${string} shipping username\` | \`section-${string} shipping cc-additional-name\` | \`section-${string} shipping cc-family-name\` | \`section-${string} shipping cc-given-name\` | \`section-${string} shipping cc-name\` | \`section-${string} shipping cc-type\` | \`section-${string} billing language\` | \`section-${string} billing organization\` | \`section-${string} billing additional-name\` | \`section-${string} billing address-level1\` | \`section-${string} billing address-level2\` | \`section-${string} billing address-level3\` | \`section-${string} billing address-level4\` | \`section-${string} billing address-line1\` | \`section-${string} billing address-line2\` | \`section-${string} billing address-line3\` | \`section-${string} billing country-name\` | \`section-${string} billing country\` | \`section-${string} billing family-name\` | \`section-${string} billing given-name\` | \`section-${string} billing honorific-prefix\` | \`section-${string} billing honorific-suffix\` | \`section-${string} billing name\` | \`section-${string} billing nickname\` | \`section-${string} billing organization-title\` | \`section-${string} billing postal-code\` | \`section-${string} billing sex\` | \`section-${string} billing street-address\` | \`section-${string} billing transaction-currency\` | \`section-${string} billing username\` | \`section-${string} billing cc-additional-name\` | \`section-${string} billing cc-family-name\` | \`section-${string} billing cc-given-name\` | \`section-${string} billing cc-name\` | \`section-${string} billing cc-type\`**

  **Default: 'on' for everything else**

  A hint as to the intended content of the field.

  When set to `on` (the default), this property indicates that the field should support autofill, but you do not have any more semantic information on the intended contents.

  When set to `off`, you are indicating that this field contains sensitive information, or contents that are never saved, like one-time codes.

  Alternatively, you can provide value which describes the specific data you would like to be entered into this field during autofill.

- **defaultValue**

  **string**

  The default value for the field.

- **details**

  **string**

  Additional text to provide context or guidance for the field. This text is displayed along with the field and its label to offer more information or instructions to the user.

  This will also be exposed to screen reader users.

- **disabled**

  **boolean**

  **Default: false**

  Disables the field, disallowing any interaction.

- **error**

  **string**

  Indicate an error to the user. The field will be given a specific stylistic treatment to communicate problems that have to be resolved immediately.

- **icon**

  **"replace" | "search" | "split" | "link" | "edit" | "product" | "variant" | "collection" | "select" | "info" | "incomplete" | "complete" | "color" | "money" | "order" | "code" | "adjust" | "affiliate" | "airplane" | "alert-bubble" | "alert-circle" | "alert-diamond" | "alert-location" | "alert-octagon" | "alert-octagon-filled" | "alert-triangle" | "alert-triangle-filled" | "align-horizontal-centers" | "app-extension" | "apps" | "archive" | "arrow-down" | "arrow-down-circle" | "arrow-down-right" | "arrow-left" | "arrow-left-circle" | "arrow-right" | "arrow-right-circle" | "arrow-up" | "arrow-up-circle" | "arrow-up-right" | "arrows-in-horizontal" | "arrows-out-horizontal" | "asterisk" | "attachment" | "automation" | "backspace" | "bag" | "bank" | "barcode" | "battery-low" | "bill" | "blank" | "blog" | "bolt" | "bolt-filled" | "book" | "book-open" | "bug" | "bullet" | "business-entity" | "button" | "button-press" | "calculator" | "calendar" | "calendar-check" | "calendar-compare" | "calendar-list" | "calendar-time" | "camera" | "camera-flip" | "caret-down" | "caret-left" | "caret-right" | "caret-up" | "cart" | "cart-abandoned" | "cart-discount" | "cart-down" | "cart-filled" | "cart-sale" | "cart-send" | "cart-up" | "cash-dollar" | "cash-euro" | "cash-pound" | "cash-rupee" | "cash-yen" | "catalog-product" | "categories" | "channels" | "channels-filled" | "chart-cohort" | "chart-donut" | "chart-funnel" | "chart-histogram-first" | "chart-histogram-first-last" | "chart-histogram-flat" | "chart-histogram-full" | "chart-histogram-growth" | "chart-histogram-last" | "chart-histogram-second-last" | "chart-horizontal" | "chart-line" | "chart-popular" | "chart-stacked" | "chart-vertical" | "chat" | "chat-new" | "chat-referral" | "check" | "check-circle" | "check-circle-filled" | "checkbox" | "chevron-down" | "chevron-down-circle" | "chevron-left" | "chevron-left-circle" | "chevron-right" | "chevron-right-circle" | "chevron-up" | "chevron-up-circle" | "circle" | "circle-dashed" | "clipboard" | "clipboard-check" | "clipboard-checklist" | "clock" | "clock-list" | "clock-revert" | "code-add" | "collection-featured" | "collection-list" | "collection-reference" | "color-none" | "compass" | "compose" | "confetti" | "connect" | "content" | "contract" | "corner-pill" | "corner-round" | "corner-square" | "credit-card" | "credit-card-cancel" | "credit-card-percent" | "credit-card-reader" | "credit-card-reader-chip" | "credit-card-reader-tap" | "credit-card-secure" | "credit-card-tap-chip" | "crop" | "currency-convert" | "cursor" | "cursor-banner" | "cursor-option" | "data-presentation" | "data-table" | "database" | "database-add" | "database-connect" | "delete" | "delivered" | "delivery" | "desktop" | "disabled" | "disabled-filled" | "discount" | "discount-add" | "discount-automatic" | "discount-code" | "discount-remove" | "dns-settings" | "dock-floating" | "dock-side" | "domain" | "domain-landing-page" | "domain-new" | "domain-redirect" | "download" | "drag-drop" | "drag-handle" | "drawer" | "duplicate" | "email" | "email-follow-up" | "email-newsletter" | "empty" | "enabled" | "enter" | "envelope" | "envelope-soft-pack" | "eraser" | "exchange" | "exit" | "export" | "external" | "eye-check-mark" | "eye-dropper" | "eye-dropper-list" | "eye-first" | "eyeglasses" | "fav" | "favicon" | "file" | "file-list" | "filter" | "filter-active" | "flag" | "flip-horizontal" | "flip-vertical" | "flower" | "folder" | "folder-add" | "folder-down" | "folder-remove" | "folder-up" | "food" | "foreground" | "forklift" | "forms" | "games" | "gauge" | "geolocation" | "gift" | "gift-card" | "git-branch" | "git-commit" | "git-repository" | "globe" | "globe-asia" | "globe-europe" | "globe-lines" | "globe-list" | "graduation-hat" | "grid" | "hashtag" | "hashtag-decimal" | "hashtag-list" | "heart" | "hide" | "hide-filled" | "home" | "home-filled" | "icons" | "identity-card" | "image" | "image-add" | "image-alt" | "image-explore" | "image-magic" | "image-none" | "image-with-text-overlay" | "images" | "import" | "in-progress" | "incentive" | "incoming" | "info-filled" | "inheritance" | "inventory" | "inventory-edit" | "inventory-list" | "inventory-transfer" | "inventory-updated" | "iq" | "key" | "keyboard" | "keyboard-filled" | "keyboard-hide" | "keypad" | "label-printer" | "language" | "language-translate" | "layout-block" | "layout-buy-button" | "layout-buy-button-horizontal" | "layout-buy-button-vertical" | "layout-column-1" | "layout-columns-2" | "layout-columns-3" | "layout-footer" | "layout-header" | "layout-logo-block" | "layout-popup" | "layout-rows-2" | "layout-section" | "layout-sidebar-left" | "layout-sidebar-right" | "lightbulb" | "link-list" | "list-bulleted" | "list-bulleted-filled" | "list-numbered" | "live" | "live-critical" | "live-none" | "location" | "location-none" | "lock" | "map" | "markets" | "markets-euro" | "markets-rupee" | "markets-yen" | "maximize" | "measurement-size" | "measurement-size-list" | "measurement-volume" | "measurement-volume-list" | "measurement-weight" | "measurement-weight-list" | "media-receiver" | "megaphone" | "mention" | "menu" | "menu-filled" | "menu-horizontal" | "menu-vertical" | "merge" | "metafields" | "metaobject" | "metaobject-list" | "metaobject-reference" | "microphone" | "microphone-muted" | "minimize" | "minus" | "minus-circle" | "mobile" | "money-none" | "money-split" | "moon" | "nature" | "note" | "note-add" | "notification" | "number-one" | "order-batches" | "order-draft" | "order-filled" | "order-first" | "order-fulfilled" | "order-repeat" | "order-unfulfilled" | "orders-status" | "organization" | "outdent" | "outgoing" | "package" | "package-cancel" | "package-fulfilled" | "package-on-hold" | "package-reassign" | "package-returned" | "page" | "page-add" | "page-attachment" | "page-clock" | "page-down" | "page-heart" | "page-list" | "page-reference" | "page-remove" | "page-report" | "page-up" | "pagination-end" | "pagination-start" | "paint-brush-flat" | "paint-brush-round" | "paper-check" | "partially-complete" | "passkey" | "paste" | "pause-circle" | "payment" | "payment-capture" | "payout" | "payout-dollar" | "payout-euro" | "payout-pound" | "payout-rupee" | "payout-yen" | "person" | "person-add" | "person-exit" | "person-filled" | "person-list" | "person-lock" | "person-remove" | "person-segment" | "personalized-text" | "phablet" | "phone" | "phone-down" | "phone-down-filled" | "phone-in" | "phone-out" | "pin" | "pin-remove" | "plan" | "play" | "play-circle" | "plus" | "plus-circle" | "plus-circle-down" | "plus-circle-filled" | "plus-circle-up" | "point-of-sale" | "point-of-sale-register" | "price-list" | "print" | "product-add" | "product-cost" | "product-filled" | "product-list" | "product-reference" | "product-remove" | "product-return" | "product-unavailable" | "profile" | "profile-filled" | "question-circle" | "question-circle-filled" | "radio-control" | "receipt" | "receipt-dollar" | "receipt-euro" | "receipt-folded" | "receipt-paid" | "receipt-pound" | "receipt-refund" | "receipt-rupee" | "receipt-yen" | "receivables" | "redo" | "referral-code" | "refresh" | "remove-background" | "reorder" | "replay" | "reset" | "return" | "reward" | "rocket" | "rotate-left" | "rotate-right" | "sandbox" | "save" | "savings" | "scan-qr-code" | "search-add" | "search-list" | "search-recent" | "search-resource" | "send" | "settings" | "share" | "shield-check-mark" | "shield-none" | "shield-pending" | "shield-person" | "shipping-label" | "shipping-label-cancel" | "shopcodes" | "slideshow" | "smiley-happy" | "smiley-joy" | "smiley-neutral" | "smiley-sad" | "social-ad" | "social-post" | "sort" | "sort-ascending" | "sort-descending" | "sound" | "sports" | "star" | "star-circle" | "star-filled" | "star-half" | "star-list" | "status" | "status-active" | "stop-circle" | "store" | "store-import" | "store-managed" | "store-online" | "sun" | "table" | "table-masonry" | "tablet" | "target" | "tax" | "team" | "text" | "text-align-center" | "text-align-left" | "text-align-right" | "text-block" | "text-bold" | "text-color" | "text-font" | "text-font-list" | "text-grammar" | "text-in-columns" | "text-in-rows" | "text-indent" | "text-indent-remove" | "text-italic" | "text-quote" | "text-title" | "text-underline" | "text-with-image" | "theme" | "theme-edit" | "theme-store" | "theme-template" | "three-d-environment" | "thumbs-down" | "thumbs-up" | "tip-jar" | "toggle-off" | "toggle-on" | "transaction" | "transaction-fee-add" | "transaction-fee-dollar" | "transaction-fee-euro" | "transaction-fee-pound" | "transaction-fee-rupee" | "transaction-fee-yen" | "transfer" | "transfer-in" | "transfer-internal" | "transfer-out" | "truck" | "undo" | "unknown-device" | "unlock" | "upload" | "variant-list" | "video" | "video-list" | "view" | "viewport-narrow" | "viewport-short" | "viewport-tall" | "viewport-wide" | "wallet" | "wand" | "watch" | "wifi" | "work" | "work-list" | "wrench" | "x" | "x-circle" | "x-circle-filled" | AnyString**

  **Default: ''**

  The type of icon to be displayed in the field.

- **id**

  **string**

  A unique identifier for the element.

- **label**

  **string**

  Content to use as the field label.

- **labelAccessibilityVisibility**

  **"visible" | "exclusive"**

  **Default: 'visible'**

  Changes the visibility of the component's label.

  - `visible`: the label is visible to all users.
  - `exclusive`: the label is visually hidden but remains in the accessibility tree.

- **maxLength**

  **number**

  **Default: Infinity**

  Specifies the maximum number of characters allowed.

- **minLength**

  **number**

  **Default: 0**

  Specifies the min number of characters allowed.

- **name**

  **string**

  An identifier for the field that is unique within the nearest containing form.

- **placeholder**

  **string**

  A short hint that describes the expected value of the field.

- **prefix**

  **string**

  **Default: ''**

  A value to be displayed immediately before the editable portion of the field.

  This is useful for displaying an implied part of the value, such as "https://" or "+353".

  This cannot be edited by the user, and it isn't included in the value of the field.

  It may not be displayed until the user has interacted with the input. For example, an inline label may take the place of the prefix until the user focuses the input.

- **readOnly**

  **boolean**

  **Default: false**

  The field cannot be edited by the user. It is focusable will be announced by screen readers.

- **required**

  **boolean**

  **Default: false**

  Whether the field needs a value. This requirement adds semantic value to the field, but it will not cause an error to appear automatically. If you want to present an error when this field is empty, you can do so with the `error` property.

- **suffix**

  **string**

  **Default: ''**

  A value to be displayed immediately after the editable portion of the field.

  This is useful for displaying an implied part of the value, such as "@shopify.com", or "%".

  This cannot be edited by the user, and it isn't included in the value of the field.

  It may not be displayed until the user has interacted with the input. For example, an inline label may take the place of the suffix until the user focuses the input.

- **value**

  **string**

  The current value for the field. If omitted, the field will be empty.

### TextAutocompleteField

```ts
'language' | 'organization' | 'additional-name' | 'address-level1' | 'address-level2' | 'address-level3' | 'address-level4' | 'address-line1' | 'address-line2' | 'address-line3' | 'country-name' | 'country' | 'family-name' | 'given-name' | 'honorific-prefix' | 'honorific-suffix' | 'name' | 'nickname' | 'one-time-code' | 'organization-title' | 'postal-code' | 'sex' | 'street-address' | 'transaction-currency' | 'username' | 'cc-additional-name' | 'cc-family-name' | 'cc-given-name' | 'cc-name' | 'cc-type'
```

### AnyString

Prevents widening string literal types in a union to \`string\`.

```ts
string & {}
```

## Slots

- **accessory**

  **HTMLElement**

  The accessory to display in the text field.

## Events

Learn more about [registering events](https://shopify.dev/docs/api/app-home/using-polaris-components#event-handling).

- **blur**

  **CallbackEventListener<'input'>**

- **change**

  **CallbackEventListener<'input'>**

- **focus**

  **CallbackEventListener<'input'>**

- **input**

  **CallbackEventListener<'input'>**

### CallbackEventListener

```ts
(EventListener & {
      (event: CallbackEvent<T>): void;
    }) | null
```

### CallbackEvent

```ts
Event & {
  currentTarget: HTMLElementTagNameMap[T];
}
```

Examples

### Examples

- #### Code

  ##### jsx

  ```jsx
  <s-text-field
    label="Store name"
    value="Jaded Pixel"
    placeholder="Become a merchant"
   />
  ```

  ##### html

  ```html
  <s-text-field
    label="Store name"
    value="Jaded Pixel"
    placeholder="Become a merchant"
  ></s-text-field>
  ```

- #### Basic usage

  ##### Description

  Demonstrates a simple text input field for entering a store name with autocomplete turned off, providing a clean and straightforward input experience.

  ##### jsx

  ```jsx
  <s-text-field label="Store name" autocomplete="off" />
  ```

  ##### html

  ```html
  <s-text-field label="Store name" autocomplete="off"></s-text-field>
  ```

- #### With icon

  ##### Description

  Showcases a text field enhanced with a search icon and a placeholder, creating a visually intuitive input for searching products.

  ##### jsx

  ```jsx
  <s-text-field
    label="Search"
    icon="search"
    placeholder="Search products..."
   />
  ```

  ##### html

  ```html
  <s-text-field
    label="Search"
    icon="search"
    placeholder="Search products..."
  ></s-text-field>
  ```

- #### Specific error messages for merchant context

  ##### Description

  Demonstrates the importance of providing clear, actionable, and context-specific error messages that guide merchants toward correct input and understanding.

  ##### jsx

  ```jsx
  <s-stack gap="base">
    {/* Generic error (avoid) */}
    <s-text-field label="Product weight" error="Invalid value" />

    {/* Specific error (preferred) */}
    <s-text-field
      label="Product weight"
      error="Weight must be greater than 0 and less than 500 pounds for shipping calculations"
     />

    {/* Business rule error */}
    <s-text-field
      label="SKU"
      error="SKU 'TSHIRT-001' already exists. SKUs must be unique across all products."
     />
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack gap="base">
    <!-- Generic error (avoid) -->
    <s-text-field label="Product weight" error="Invalid value"></s-text-field>

    <!-- Specific error (preferred) -->
    <s-text-field
      label="Product weight"
      error="Weight must be greater than 0 and less than 500 pounds for shipping calculations"
    ></s-text-field>

    <!-- Business rule error -->
    <s-text-field
      label="SKU"
      error="SKU 'TSHIRT-001' already exists. SKUs must be unique across all products."
    ></s-text-field>
  </s-stack>
  ```

- #### Required field with validation

  ##### Description

  Illustrates a text field marked as required, ensuring that users must provide input before form submission, with built-in validation support.

  ##### jsx

  ```jsx
  <s-text-field label="Store name" required />
  ```

  ##### html

  ```html
  <s-text-field label="Store name" required></s-text-field>
  ```

- #### With prefix and suffix

  ##### Description

  Displays text field usage with prefix and suffix

  ##### jsx

  ```jsx
  <s-stack gap="small">
      <s-text-field
      label="Phone number"
      prefix="+03"
      />
      <s-text-field
      label="Credit Card Number"
      value="1234 5678 9012 3456"
      suffix="VISA"
      />
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack gap="small">
    <s-text-field label="Phone number" prefix="+03" />
    <s-text-field
      label="Credit Card Number"
      value="1234 5678 9012 3456"
      suffix="VISA"
    />
  </s-stack>
  ```

- #### With accessory

  ##### Description

  Demonstrates the flexibility of adding interactive elements like buttons to text fields, enabling immediate actions based on the entered input.

  ##### jsx

  ```jsx
  <>
    <s-tooltip id="info-tooltip">This is info tooltip</s-tooltip>
    <s-text-field label="Discount code">
      <s-icon slot="accessory" interestFor="info-tooltip" type="info" />
    </s-text-field>
  </>
  ```

  ##### html

  ```html
  <s-tooltip id="info-tooltip">This is info tooltip</s-tooltip>
  <s-text-field label="Discount code">
    <s-icon slot="accessory" interestFor="info-tooltip" type="info" />
  </s-text-field>
  ```

</page>

<page>
---
title: URLField
description: Collect URLs from users with built-in formatting and validation.
api_name: app-home
source_url:
  html: 'https://shopify.dev/docs/api/app-home/polaris-web-components/forms/urlfield'
  md: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/forms/urlfield.md
---

# URLField

Collect URLs from users with built-in formatting and validation.

## URLField

- **autocomplete**

  **"on" | "off" | \`section-${string} url\` | \`section-${string} photo\` | \`section-${string} impp\` | \`section-${string} home impp\` | \`section-${string} mobile impp\` | \`section-${string} fax impp\` | \`section-${string} pager impp\` | "shipping url" | "shipping photo" | "shipping impp" | "shipping home impp" | "shipping mobile impp" | "shipping fax impp" | "shipping pager impp" | "billing url" | "billing photo" | "billing impp" | "billing home impp" | "billing mobile impp" | "billing fax impp" | "billing pager impp" | \`section-${string} shipping url\` | \`section-${string} shipping photo\` | \`section-${string} shipping impp\` | \`section-${string} shipping home impp\` | \`section-${string} shipping mobile impp\` | \`section-${string} shipping fax impp\` | \`section-${string} shipping pager impp\` | \`section-${string} billing url\` | \`section-${string} billing photo\` | \`section-${string} billing impp\` | \`section-${string} billing home impp\` | \`section-${string} billing mobile impp\` | \`section-${string} billing fax impp\` | \`section-${string} billing pager impp\` | URLAutocompleteField**

  **Default: 'on' for everything else**

  A hint as to the intended content of the field.

  When set to `on` (the default), this property indicates that the field should support autofill, but you do not have any more semantic information on the intended contents.

  When set to `off`, you are indicating that this field contains sensitive information, or contents that are never saved, like one-time codes.

  Alternatively, you can provide value which describes the specific data you would like to be entered into this field during autofill.

- **defaultValue**

  **string**

  The default value for the field.

- **details**

  **string**

  Additional text to provide context or guidance for the field. This text is displayed along with the field and its label to offer more information or instructions to the user.

  This will also be exposed to screen reader users.

- **disabled**

  **boolean**

  **Default: false**

  Disables the field, disallowing any interaction.

- **error**

  **string**

  Indicate an error to the user. The field will be given a specific stylistic treatment to communicate problems that have to be resolved immediately.

- **id**

  **string**

  A unique identifier for the element.

- **label**

  **string**

  Content to use as the field label.

- **labelAccessibilityVisibility**

  **"visible" | "exclusive"**

  **Default: 'visible'**

  Changes the visibility of the component's label.

  - `visible`: the label is visible to all users.
  - `exclusive`: the label is visually hidden but remains in the accessibility tree.

- **maxLength**

  **number**

  **Default: Infinity**

  Specifies the maximum number of characters allowed.

- **minLength**

  **number**

  **Default: 0**

  Specifies the min number of characters allowed.

- **name**

  **string**

  An identifier for the field that is unique within the nearest containing form.

- **placeholder**

  **string**

  A short hint that describes the expected value of the field.

- **readOnly**

  **boolean**

  **Default: false**

  The field cannot be edited by the user. It is focusable will be announced by screen readers.

- **required**

  **boolean**

  **Default: false**

  Whether the field needs a value. This requirement adds semantic value to the field, but it will not cause an error to appear automatically. If you want to present an error when this field is empty, you can do so with the `error` property.

- **value**

  **string**

  The current value for the field. If omitted, the field will be empty.

### URLAutocompleteField

```ts
'url' | 'photo' | 'impp' | 'home impp' | 'mobile impp' | 'fax impp' | 'pager impp'
```

## Events

Learn more about [registering events](https://shopify.dev/docs/api/app-home/using-polaris-components#event-handling).

- **blur**

  **CallbackEventListener<'input'>**

- **change**

  **CallbackEventListener<'input'>**

- **focus**

  **CallbackEventListener<'input'>**

- **input**

  **CallbackEventListener<'input'>**

### CallbackEventListener

```ts
(EventListener & {
      (event: CallbackEvent<T>): void;
    }) | null
```

### CallbackEvent

```ts
Event & {
  currentTarget: HTMLElementTagNameMap[T];
}
```

Examples

### Examples

- #### Code

  ##### jsx

  ```jsx
  <s-url-field
    label="Your website"
    details="Join the partner ecosystem"
    placeholder="https://shopify.com/partner"
   />
  ```

  ##### html

  ```html
  <s-url-field
    label="Your website"
    details="Join the partner ecosystem"
    placeholder="https://shopify.com/partner"
  ></s-url-field>
  ```

- #### Basic usage

  ##### Description

  Demonstrates a simple URL input field with a label and placeholder, showing the minimal configuration needed for collecting a URL.

  ##### jsx

  ```jsx
  <s-stack gap="base">
    {/* Simple URL input */}
    <s-url-field
      label="Website URL"
      placeholder="https://example.com"
     />
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack gap="base">
    <!-- Simple URL input -->
    <s-url-field
      label="Website URL"
      placeholder="https://example.com"
    ></s-url-field>
  </s-stack>
  ```

- #### With validation

  ##### Description

  Shows a URL input field with built-in validation, including required status, minimum and maximum length constraints, and a custom error message for invalid inputs.

  ##### jsx

  ```jsx
  <s-url-field
    label="Company website"
    required
    minLength={10}
    maxLength={200}
    error="Please enter a valid website URL"
    />
  ```

  ##### html

  ```html
  <s-url-field
    label="Company website"
    required
    minLength="10"
    maxLength="200"
    error="Please enter a valid website URL"
  ></s-url-field>
  ```

- #### With default value

  ##### Description

  Illustrates a URL field pre-populated with a default value, set to read-only mode to prevent user modifications.

  ##### jsx

  ```jsx
  <s-stack gap="base">
    <s-url-field
      label="Profile URL"
      defaultValue="https://shop.myshopify.com"
      readOnly
    />
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack gap="base">
    <s-url-field
      label="Profile URL"
      value="https://shop.myshopify.com"
      readOnly
    ></s-url-field>
  </s-stack>
  ```

- #### Disabled state

  ##### Description

  Shows a URL field in a disabled state, displaying a pre-filled URL that cannot be edited by the user.

  ##### jsx

  ```jsx
  <s-url-field
    label="Store URL"
    value="https://your-store.myshopify.com"
    disabled
  />
  ```

  ##### html

  ```html
  <s-url-field
    label="Store URL"
    value="https://your-store.myshopify.com"
    disabled
  ></s-url-field>
  ```

</page>

<page>
---
title: Box
description: >-
  A generic container that provides a flexible alternative for custom designs
  not achievable with existing components. Use it to apply styling such as
  backgrounds, padding, or borders, or to nest and group other components. The
  contents of Box always maintain their natural size, making it especially
  useful within layout components that would otherwise stretch their children.
api_name: app-home
source_url:
  html: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/layout-and-structure/box
  md: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/layout-and-structure/box.md
---

# Box

A generic container that provides a flexible alternative for custom designs not achievable with existing components. Use it to apply styling such as backgrounds, padding, or borders, or to nest and group other components. The contents of Box always maintain their natural size, making it especially useful within layout components that would otherwise stretch their children.

## Properties

- **accessibilityLabel**

  **string**

  A label that describes the purpose or contents of the element. When set, it will be announced to users using assistive technologies and will provide them with more context.

  Only use this when the element's content is not enough context for users using assistive technologies.

- **accessibilityRole**

  **AccessibilityRole**

  **Default: 'generic'**

  Sets the semantic meaning of the component’s content. When set, the role will be used by assistive technologies to help users navigate the page.

- **accessibilityVisibility**

  **"visible" | "hidden" | "exclusive"**

  **Default: 'visible'**

  Changes the visibility of the element.

  - `visible`: the element is visible to all users.
  - `hidden`: the element is removed from the accessibility tree but remains visible.
  - `exclusive`: the element is visually hidden but remains in the accessibility tree.

- **background**

  **BackgroundColorKeyword**

  **Default: 'transparent'**

  Adjust the background of the component.

- **blockSize**

  **SizeUnitsOrAuto**

  **Default: 'auto'**

  Adjust the [block size](https://developer.mozilla.org/en-US/docs/Web/CSS/block-size).

- **border**

  **BorderShorthand**

  **Default: 'none' - equivalent to \`none base auto\`.**

  Set the border via the shorthand property.

  This can be a size, optionally followed by a color, optionally followed by a style.

  If the color is not specified, it will be `base`.

  If the style is not specified, it will be `auto`.

  Values can be overridden by `borderWidth`, `borderStyle`, and `borderColor`.

- **borderColor**

  **"" | ColorKeyword**

  **Default: '' - meaning no override**

  Adjust the color of the border.

- **borderRadius**

  **MaybeAllValuesShorthandProperty\<BoxBorderRadii>**

  **Default: 'none'**

  Adjust the radius of the border.

- **borderStyle**

  **"" | MaybeAllValuesShorthandProperty\<BoxBorderStyles>**

  **Default: '' - meaning no override**

  Adjust the style of the border.

- **borderWidth**

  **"" | MaybeAllValuesShorthandProperty<"small" | "small-100" | "base" | "large" | "large-100" | "none">**

  **Default: '' - meaning no override**

  Adjust the width of the border.

- **display**

  **MaybeResponsive<"auto" | "none">**

  **Default: 'auto'**

  Sets the outer [display](https://developer.mozilla.org/en-US/docs/Web/CSS/display) type of the component. The outer type sets a component's participation in [flow layout](https://developer.mozilla.org/en-US/docs/Web/CSS/CSS_flow_layout).

  - `auto` the component's initial value. The actual value depends on the component and context.
  - `none` hides the component from display and removes it from the accessibility tree, making it invisible to screen readers.

- **inlineSize**

  **SizeUnitsOrAuto**

  **Default: 'auto'**

  Adjust the [inline size](https://developer.mozilla.org/en-US/docs/Web/CSS/inline-size).

- **maxBlockSize**

  **SizeUnitsOrNone**

  **Default: 'none'**

  Adjust the [maximum block size](https://developer.mozilla.org/en-US/docs/Web/CSS/max-block-size).

- **maxInlineSize**

  **SizeUnitsOrNone**

  **Default: 'none'**

  Adjust the [maximum inline size](https://developer.mozilla.org/en-US/docs/Web/CSS/max-inline-size).

- **minBlockSize**

  **SizeUnits**

  **Default: '0'**

  Adjust the [minimum block size](https://developer.mozilla.org/en-US/docs/Web/CSS/min-block-size).

- **minInlineSize**

  **SizeUnits**

  **Default: '0'**

  Adjust the [minimum inline size](https://developer.mozilla.org/en-US/docs/Web/CSS/min-inline-size).

- **overflow**

  **"visible" | "hidden"**

  **Default: 'visible'**

  Sets the overflow behavior of the element.

  - `hidden`: clips the content when it is larger than the element’s container. The element will not be scrollable and the users will not be able to access the clipped content by dragging or using a scroll wheel on a mouse.
  - `visible`: the content that extends beyond the element’s container is visible.

- **padding**

  **MaybeResponsive\<MaybeAllValuesShorthandProperty\<PaddingKeyword>>**

  **Default: 'none'**

  Adjust the padding of all edges.

  [1-to-4-value syntax](https://developer.mozilla.org/en-US/docs/Web/CSS/Shorthand_properties#edges_of_a_box) is supported. Note that, contrary to the CSS, it uses flow-relative values and the order is:

  - 4 values: `block-start inline-end block-end inline-start`
  - 3 values: `block-start inline block-end`
  - 2 values: `block inline`

  For example:

  - `large` means block-start, inline-end, block-end and inline-start paddings are `large`.
  - `large none` means block-start and block-end paddings are `large`, inline-start and inline-end paddings are `none`.
  - `large none large` means block-start padding is `large`, inline-end padding is `none`, block-end padding is `large` and inline-start padding is `none`.
  - `large none large small` means block-start padding is `large`, inline-end padding is `none`, block-end padding is `large` and inline-start padding is `small`.

  A padding value of `auto` will use the default padding for the closest container that has had its usual padding removed.

  `padding` also accepts a [responsive value](https://shopify.dev/docs/api/app-home/using-polaris-components#responsive-values) string with the supported PaddingKeyword as a query value.

- **paddingBlock**

  **MaybeResponsive<"" | MaybeTwoValuesShorthandProperty\<PaddingKeyword>>**

  **Default: '' - meaning no override**

  Adjust the block-padding.

  - `large none` means block-start padding is `large`, block-end padding is `none`.

  This overrides the block value of `padding`.

  `paddingBlock` also accepts a [responsive value](https://shopify.dev/docs/api/app-home/using-polaris-components#responsive-values) string with the supported PaddingKeyword as a query value.

- **paddingBlockEnd**

  **MaybeResponsive<"" | PaddingKeyword>**

  **Default: '' - meaning no override**

  Adjust the block-end padding.

  This overrides the block-end value of `paddingBlock`.

  `paddingBlockEnd` also accepts a [responsive value](https://shopify.dev/docs/api/app-home/using-polaris-components#responsive-values) string with the supported PaddingKeyword as a query value.

- **paddingBlockStart**

  **MaybeResponsive<"" | PaddingKeyword>**

  **Default: '' - meaning no override**

  Adjust the block-start padding.

  This overrides the block-start value of `paddingBlock`.

  `paddingBlockStart` also accepts a [responsive value](https://shopify.dev/docs/api/app-home/using-polaris-components#responsive-values) string with the supported PaddingKeyword as a query value.

- **paddingInline**

  **MaybeResponsive<"" | MaybeTwoValuesShorthandProperty\<PaddingKeyword>>**

  **Default: '' - meaning no override**

  Adjust the inline padding.

  - `large none` means inline-start padding is `large`, inline-end padding is `none`.

  This overrides the inline value of `padding`.

  `paddingInline` also accepts a [responsive value](https://shopify.dev/docs/api/app-home/using-polaris-components#responsive-values) string with the supported PaddingKeyword as a query value.

- **paddingInlineEnd**

  **MaybeResponsive<"" | PaddingKeyword>**

  **Default: '' - meaning no override**

  Adjust the inline-end padding.

  This overrides the inline-end value of `paddingInline`.

  `paddingInlineEnd` also accepts a [responsive value](https://shopify.dev/docs/api/app-home/using-polaris-components#responsive-values) string with the supported PaddingKeyword as a query value.

- **paddingInlineStart**

  **MaybeResponsive<"" | PaddingKeyword>**

  **Default: '' - meaning no override**

  Adjust the inline-start padding.

  This overrides the inline-start value of `paddingInline`.

  `paddingInlineStart` also accepts a [responsive value](https://shopify.dev/docs/api/app-home/using-polaris-components#responsive-values) string with the supported PaddingKeyword as a query value.

### AccessibilityRole

```ts
'main' | 'header' | 'footer' | 'section' | 'region' | 'aside' | 'navigation' | 'ordered-list' | 'list-item' | 'list-item-separator' | 'unordered-list' | 'separator' | 'status' | 'alert' | 'generic' | 'presentation' | 'none'
```

### BackgroundColorKeyword

```ts
'transparent' | ColorKeyword
```

### ColorKeyword

```ts
'subdued' | 'base' | 'strong'
```

### SizeUnitsOrAuto

```ts
SizeUnits | 'auto'
```

### SizeUnits

```ts
`${number}px` | `${number}%` | `0`
```

### BorderShorthand

Represents a shorthand for defining a border. It can be a combination of size, optionally followed by color, optionally followed by style.

```ts
BorderSizeKeyword | `${BorderSizeKeyword} ${ColorKeyword}` | `${BorderSizeKeyword} ${ColorKeyword} ${BorderStyleKeyword}`
```

### BorderSizeKeyword

```ts
SizeKeyword | 'none'
```

### SizeKeyword

```ts
'small-500' | 'small-400' | 'small-300' | 'small-200' | 'small-100' | 'small' | 'base' | 'large' | 'large-100' | 'large-200' | 'large-300' | 'large-400' | 'large-500'
```

### BorderStyleKeyword

```ts
'none' | 'solid' | 'dashed' | 'dotted' | 'auto'
```

### MaybeAllValuesShorthandProperty

```ts
T | `${T} ${T}` | `${T} ${T} ${T}` | `${T} ${T} ${T} ${T}`
```

### BoxBorderRadii

```ts
'small' | 'small-200' | 'small-100' | 'base' | 'large' | 'large-100' | 'large-200' | 'none'
```

### BoxBorderStyles

```ts
'auto' | 'none' | 'solid' | 'dashed'
```

### MaybeResponsive

```ts
T | `@container${string}`
```

### SizeUnitsOrNone

```ts
SizeUnits | 'none'
```

### PaddingKeyword

```ts
SizeKeyword | 'none'
```

### MaybeTwoValuesShorthandProperty

```ts
T | `${T} ${T}`
```

## Slots

- **children**

  **HTMLElement**

  The content of the Box.

Examples

### Examples

- #### Code

  ##### jsx

  ```jsx
  <>
    <s-box padding="base">Available for iPad, iPhone, and Android.</s-box>

    <s-box padding="base" background="subdued" border="base" borderRadius="base">
      Available for iPad, iPhone, and Android.
    </s-box>
  </>
  ```

  ##### html

  ```html
  <s-box padding="base">Available for iPad, iPhone, and Android.</s-box>

  <s-box padding="base" background="subdued" border="base" borderRadius="base">
    Available for iPad, iPhone, and Android.
  </s-box>
  ```

- #### Basic container

  ##### Description

  Demonstrates creating a simple container with padding, base background, border, and rounded corners to group and highlight product information.

  ##### jsx

  ```jsx
  <s-box
    padding="base"
    background="base"
    borderWidth="base"
    borderColor="base"
    borderRadius="base"
  >
    <s-paragraph>Product information</s-paragraph>
  </s-box>
  ```

  ##### html

  ```html
  <s-box
    padding="base"
    background="base"
    borderWidth="base"
    borderColor="base"
    borderRadius="base"
  >
    <s-paragraph>Product information</s-paragraph>
  </s-box>
  ```

- #### Responsive shipping notice

  ##### Description

  Illustrates using a box with responsive padding to create an adaptable container for shipping information that can adjust to different screen or container sizes.

  ##### jsx

  ```jsx
  <s-query-container>
    <s-box
      padding="@container (inline-size > 400px) base, large-200"
      background="base"
      borderWidth="base"
      borderColor="base"
    >
      <s-text>Your order will be processed within 2-3 business days</s-text>
    </s-box>
  </s-query-container>
  ```

  ##### html

  ```html
  <s-query-container>
    <s-box
      padding="@container (inline-size > 400px) base, large-200"
      background="base"
      borderWidth="base"
      borderColor="base"
    >
      <s-paragraph>Your order will be processed within 2-3 business days</s-paragraph>
    </s-box>
  </s-query-container>
  ```

- #### Accessible status messages

  ##### Description

  Shows how to use boxes with ARIA roles and visibility controls to create semantic, screen-reader-friendly status and informational messages.

  ##### jsx

  ```jsx
  <s-stack gap="base">
    <s-box
      accessibilityRole="status"
      padding="base"
      background="strong"
      borderRadius="base"
    >
      <s-paragraph>Payment failed</s-paragraph>
    </s-box>

    <s-box accessibilityVisibility="exclusive">
      <s-paragraph>Price includes tax and shipping</s-paragraph>
    </s-box>
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack gap="base">
    <s-box
      accessibilityRole="status"
      padding="base"
      background="strong"
      borderRadius="base"
    >
      <s-paragraph>Payment failed</s-paragraph>
    </s-box>

    <s-box accessibilityVisibility="exclusive">
      <s-paragraph>Price includes tax and shipping</s-paragraph>
    </s-box>
  </s-stack>
  ```

- #### Nested hierarchical containers

  ##### Description

  Demonstrates creating nested, hierarchical layouts using multiple boxes, showing how boxes can be combined to organize related content sections with different styling.

  ##### jsx

  ```jsx
  <s-stack gap="base">
    {/* Inventory status section */}
    <s-box
      padding="base"
      background="base"
      borderRadius="base"
      borderWidth="base"
      borderColor="base"
    >
      <s-stack gap="base">
        <s-box padding="small-100" background="subdued" borderRadius="small">
          <s-paragraph>In stock: 45 units</s-paragraph>
        </s-box>
        <s-box padding="small-100" background="subdued" borderRadius="small">
          <s-paragraph>Low stock alert: 5 units</s-paragraph>
        </s-box>
      </s-stack>
    </s-box>

    {/* Product information section */}
    <s-box
      padding="base"
      background="base"
      borderRadius="base"
      borderWidth="base"
      borderColor="base"
    >
      <s-stack gap="base">
        <s-heading>Product sales</s-heading>
        <s-paragraph color="subdued">No recent sales of this product</s-paragraph>
        <s-link>View details</s-link>
      </s-stack>
    </s-box>
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack gap="base">
    <!-- Inventory status section -->
    <s-box
      padding="base"
      background="base"
      borderRadius="base"
      borderWidth="base"
      borderColor="base"
    >
      <s-stack gap="base">
        <s-box padding="small-100" background="subdued" borderRadius="small">
          <s-paragraph>In stock: 45 units</s-paragraph>
        </s-box>
        <s-box padding="small-100" background="subdued" borderRadius="small">
          <s-paragraph>Low stock alert: 5 units</s-paragraph>
        </s-box>
      </s-stack>
    </s-box>

    <!-- Product information section -->
    <s-box
      padding="base"
      background="base"
      borderRadius="base"
      borderWidth="base"
      borderColor="base"
    >
      <s-stack gap="base">
        <s-heading>Product sales</s-heading>
        <s-paragraph color="subdued">No recent sales of this product</s-paragraph>
        <s-link>View details</s-link>
      </s-stack>
    </s-box>
  </s-stack>
  ```

## Useful for

- Creating custom designs when you can't build what you need with the existing components.
- Setting up specific stylings such as background colors, paddings, and borders.
- Nesting with other components.

## Best practices

- Use for structural layouts with consistent spacing patterns
- Avoid adding too many borders that may visually fragment the interface

</page>

<page>
---
title: Divider
description: Create clear visual separation between elements in your user interface.
api_name: app-home
source_url:
  html: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/layout-and-structure/divider
  md: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/layout-and-structure/divider.md
---

# Divider

Create clear visual separation between elements in your user interface.

## Properties

- **color**

  **"base" | "strong"**

  **Default: 'base'**

  Modify the color to be more or less intense.

- **direction**

  **"inline" | "block"**

  **Default: 'inline'**

  Specify the direction of the divider. This uses [logical properties](https://developer.mozilla.org/en-US/docs/Web/CSS/CSS_logical_properties_and_values).

Examples

### Examples

- #### Code

  ##### jsx

  ```jsx
  <s-divider />
  ```

  ##### html

  ```html
  <s-divider></s-divider>
  ```

- #### Basic usage

  ##### Description

  Demonstrates the default divider with standard base color and inline direction.

  ##### jsx

  ```jsx
  <s-divider />
  ```

  ##### html

  ```html
  <s-divider></s-divider>
  ```

- #### Custom color

  ##### Description

  Shows a divider with a strong color variant for increased visual emphasis.

  ##### jsx

  ```jsx
  <s-divider color="strong" />
  ```

  ##### html

  ```html
  <s-divider color="strong"></s-divider>
  ```

- #### Custom direction

  ##### Description

  Illustrates using a block-direction divider within an inline stack to create vertical separation between items.

  ##### jsx

  ```jsx
  <s-stack direction="inline" gap="base">
    <s-text>Item 1</s-text>
    <s-divider direction="block" />
    <s-text>Item 2</s-text>
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack direction="inline" gap="base">
    <s-text>Item 1</s-text>
    <s-divider direction="block"></s-divider>
    <s-text>Item 2</s-text>
  </s-stack>
  ```

- #### Separating form sections

  ##### Description

  Uses a divider to visually group and separate different sections of a form, improving readability and user comprehension.

  ##### jsx

  ```jsx
  <s-stack gap="base">
    <s-text-field label="Store name" />
    <s-text-field label="Description" />
    <s-divider />
    <s-text-field label="Email address" />
    <s-text-field label="Phone number" />
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack gap="base">
    <s-text-field label="Store name"></s-text-field>
    <s-text-field label="Description"></s-text-field>
    <s-divider></s-divider>
    <s-text-field label="Email address"></s-text-field>
    <s-text-field label="Phone number"></s-text-field>
  </s-stack>
  ```

- #### Organizing settings panels

  ##### Description

  Demonstrates using a divider to logically separate basic and advanced settings in a configuration panel.

  ##### jsx

  ```jsx
  <s-box padding="base">
    <s-stack gap="base">
      <s-switch label="Email notifications" />
      <s-switch label="Auto-save" />
      <s-divider />
      <s-switch label="Advanced settings" />
      <s-switch label="Developer tools" />
    </s-stack>
  </s-box>
  ```

  ##### html

  ```html
  <s-box padding="base">
    <s-stack gap="base">
      <s-switch label="Email notifications"></s-switch>
      <s-switch label="Auto-save"></s-switch>
      <s-divider></s-divider>
      <s-switch label="Advanced settings"></s-switch>
      <s-switch label="Developer tools"></s-switch>
    </s-stack>
  </s-box>
  ```

- #### Visual breaks in section layouts

  ##### Description

  Shows how dividers can be used to create clean, segmented sections within a section, improving information hierarchy.

  ##### jsx

  ```jsx
  <s-box padding="large-400" background="base">
    <s-stack gap="base">
      <s-heading>Order summary</s-heading>
      <s-text>3 items</s-text>
      <s-divider />
      <s-heading>Shipping address</s-heading>
      <s-text>123 Main Street, Toronto ON</s-text>
      <s-divider />
      <s-heading>Payment method</s-heading>
      <s-text>•••• 4242</s-text>
    </s-stack>
  </s-box>
  ```

  ##### html

  ```html
  <s-box padding="large-400" background="base">
    <s-stack gap="base">
      <s-heading>Order summary</s-heading>
      <s-text>3 items</s-text>
      <s-divider></s-divider>
      <s-heading>Shipping address</s-heading>
      <s-text>123 Main Street, Toronto ON</s-text>
      <s-divider></s-divider>
      <s-heading>Payment method</s-heading>
      <s-text>•••• 4242</s-text>
    </s-stack>
  </s-box>
  ```

- #### Separating content sections

  ##### Description

  Illustrates using dividers to create clear, visually distinct sections for different metrics or content blocks.

  ##### jsx

  ```jsx
  <s-stack gap="base">
    <s-box padding="base">
      <s-text>150 orders</s-text>
    </s-box>
    <s-divider />
    <s-box padding="base">
      <s-text>$2,400 revenue</s-text>
    </s-box>
    <s-divider />
    <s-box padding="base">
      <s-text>89 customers</s-text>
    </s-box>
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack gap="base">
    <s-box padding="base">
      <s-text>150 orders</s-text>
    </s-box>
    <s-divider></s-divider>
    <s-box padding="base">
      <s-text>$2,400 revenue</s-text>
    </s-box>
    <s-divider></s-divider>
    <s-box padding="base">
      <s-text>89 customers</s-text>
    </s-box>
  </s-stack>
  ```

## Useful for

- Separating elements inside sections.
- Visually grouping related content in forms and lists.

</page>

<page>
---
title: Grid
description: >-
  Use `s-grid` to organize your content in a matrix of rows and columns and make
  responsive layouts for pages. Grid follows the same pattern as the [CSS grid
  layout](https://developer.mozilla.org/en-US/docs/Web/CSS/CSS_grid_layout).
api_name: app-home
source_url:
  html: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/layout-and-structure/grid
  md: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/layout-and-structure/grid.md
---

# Grid

Use `s-grid` to organize your content in a matrix of rows and columns and make responsive layouts for pages. Grid follows the same pattern as the [CSS grid layout](https://developer.mozilla.org/en-US/docs/Web/CSS/CSS_grid_layout).

## Properties

- **accessibilityLabel**

  **string**

  A label that describes the purpose or contents of the element. When set, it will be announced to users using assistive technologies and will provide them with more context.

  Only use this when the element's content is not enough context for users using assistive technologies.

- **accessibilityRole**

  **AccessibilityRole**

  **Default: 'generic'**

  Sets the semantic meaning of the component’s content. When set, the role will be used by assistive technologies to help users navigate the page.

- **accessibilityVisibility**

  **"visible" | "hidden" | "exclusive"**

  **Default: 'visible'**

  Changes the visibility of the element.

  - `visible`: the element is visible to all users.
  - `hidden`: the element is removed from the accessibility tree but remains visible.
  - `exclusive`: the element is visually hidden but remains in the accessibility tree.

- **alignContent**

  **"" | AlignContentKeyword**

  **Default: '' - meaning no override**

  Aligns the grid along the block axis.

  This overrides the block value of `placeContent`.

- **alignItems**

  **"" | AlignItemsKeyword**

  **Default: '' - meaning no override**

  Aligns the grid items along the block axis.

- **background**

  **BackgroundColorKeyword**

  **Default: 'transparent'**

  Adjust the background of the component.

- **blockSize**

  **SizeUnitsOrAuto**

  **Default: 'auto'**

  Adjust the [block size](https://developer.mozilla.org/en-US/docs/Web/CSS/block-size).

- **border**

  **BorderShorthand**

  **Default: 'none' - equivalent to \`none base auto\`.**

  Set the border via the shorthand property.

  This can be a size, optionally followed by a color, optionally followed by a style.

  If the color is not specified, it will be `base`.

  If the style is not specified, it will be `auto`.

  Values can be overridden by `borderWidth`, `borderStyle`, and `borderColor`.

- **borderColor**

  **"" | ColorKeyword**

  **Default: '' - meaning no override**

  Adjust the color of the border.

- **borderRadius**

  **MaybeAllValuesShorthandProperty\<BoxBorderRadii>**

  **Default: 'none'**

  Adjust the radius of the border.

- **borderStyle**

  **"" | MaybeAllValuesShorthandProperty\<BoxBorderStyles>**

  **Default: '' - meaning no override**

  Adjust the style of the border.

- **borderWidth**

  **"" | MaybeAllValuesShorthandProperty<"small" | "small-100" | "base" | "large" | "large-100" | "none">**

  **Default: '' - meaning no override**

  Adjust the width of the border.

- **columnGap**

  **MaybeResponsive<"" | SpacingKeyword>**

  **Default: '' - meaning no override**

  Adjust spacing between elements in the inline axis.

  This overrides the column value of `gap`. `columnGap` either accepts:

  - a single [SpacingKeyword](https://shopify.dev/docs/api/app-home/using-polaris-components#scale) value (e.g. `large-100`)
  - OR a [responsive value](https://shopify.dev/docs/api/app-home/using-polaris-components#responsive-values) string with the supported SpacingKeyword as a query value.

- **display**

  **MaybeResponsive<"auto" | "none">**

  **Default: 'auto'**

  Sets the outer [display](https://developer.mozilla.org/en-US/docs/Web/CSS/display) type of the component. The outer type sets a component's participation in [flow layout](https://developer.mozilla.org/en-US/docs/Web/CSS/CSS_flow_layout).

  - `auto` the component's initial value. The actual value depends on the component and context.
  - `none` hides the component from display and removes it from the accessibility tree, making it invisible to screen readers.

- **gap**

  **MaybeResponsive\<MaybeTwoValuesShorthandProperty\<SpacingKeyword>>**

  **Default: 'none'**

  Adjust spacing between elements.

  `gap` can either accept:

  - a single [SpacingKeyword](https://shopify.dev/docs/api/app-home/using-polaris-components#scale) value applied to both axes (e.g. `large-100`)
  - OR a pair of values (eg `large-100 large-500`) can be used to set the inline and block axes respectively
  - OR a [responsive value](https://shopify.dev/docs/api/app-home/using-polaris-components#responsive-values) string with the supported SpacingKeyword as a query value.

- **gridTemplateColumns**

  **string**

  **Default: 'none'**

  Define columns and specify their size. `gridTemplateColumns` either accepts:

  - [track sizing values](https://developer.mozilla.org/en-US/docs/Web/CSS/CSS_grid_layout/Basic_concepts_of_grid_layout#fixed_and_flexible_track_sizes) (e.g. `1fr auto`)
  - OR [responsive values](https://shopify.dev/docs/api/app-home/using-polaris-components#responsive-values) string with the supported track sizing values as a query value.

- **gridTemplateRows**

  **string**

  **Default: 'none'**

  Define rows and specify their size. `gridTemplateRows` either accepts:

  - [track sizing values](https://developer.mozilla.org/en-US/docs/Web/CSS/CSS_grid_layout/Basic_concepts_of_grid_layout#fixed_and_flexible_track_sizes) (e.g. `1fr auto`)
  - OR [responsive values](https://shopify.dev/docs/api/app-home/using-polaris-components#responsive-values) string with the supported track sizing values as a query value.

- **inlineSize**

  **SizeUnitsOrAuto**

  **Default: 'auto'**

  Adjust the [inline size](https://developer.mozilla.org/en-US/docs/Web/CSS/inline-size).

- **justifyContent**

  **"" | JustifyContentKeyword**

  **Default: '' - meaning no override**

  Aligns the grid along the inline axis.

  This overrides the inline value of `placeContent`.

- **justifyItems**

  **"" | JustifyItemsKeyword**

  **Default: '' - meaning no override**

  Aligns the grid items along the inline axis.

- **maxBlockSize**

  **SizeUnitsOrNone**

  **Default: 'none'**

  Adjust the [maximum block size](https://developer.mozilla.org/en-US/docs/Web/CSS/max-block-size).

- **maxInlineSize**

  **SizeUnitsOrNone**

  **Default: 'none'**

  Adjust the [maximum inline size](https://developer.mozilla.org/en-US/docs/Web/CSS/max-inline-size).

- **minBlockSize**

  **SizeUnits**

  **Default: '0'**

  Adjust the [minimum block size](https://developer.mozilla.org/en-US/docs/Web/CSS/min-block-size).

- **minInlineSize**

  **SizeUnits**

  **Default: '0'**

  Adjust the [minimum inline size](https://developer.mozilla.org/en-US/docs/Web/CSS/min-inline-size).

- **overflow**

  **"visible" | "hidden"**

  **Default: 'visible'**

  Sets the overflow behavior of the element.

  - `hidden`: clips the content when it is larger than the element’s container. The element will not be scrollable and the users will not be able to access the clipped content by dragging or using a scroll wheel on a mouse.
  - `visible`: the content that extends beyond the element’s container is visible.

- **padding**

  **MaybeResponsive\<MaybeAllValuesShorthandProperty\<PaddingKeyword>>**

  **Default: 'none'**

  Adjust the padding of all edges.

  [1-to-4-value syntax](https://developer.mozilla.org/en-US/docs/Web/CSS/Shorthand_properties#edges_of_a_box) is supported. Note that, contrary to the CSS, it uses flow-relative values and the order is:

  - 4 values: `block-start inline-end block-end inline-start`
  - 3 values: `block-start inline block-end`
  - 2 values: `block inline`

  For example:

  - `large` means block-start, inline-end, block-end and inline-start paddings are `large`.
  - `large none` means block-start and block-end paddings are `large`, inline-start and inline-end paddings are `none`.
  - `large none large` means block-start padding is `large`, inline-end padding is `none`, block-end padding is `large` and inline-start padding is `none`.
  - `large none large small` means block-start padding is `large`, inline-end padding is `none`, block-end padding is `large` and inline-start padding is `small`.

  A padding value of `auto` will use the default padding for the closest container that has had its usual padding removed.

  `padding` also accepts a [responsive value](https://shopify.dev/docs/api/app-home/using-polaris-components#responsive-values) string with the supported PaddingKeyword as a query value.

- **paddingBlock**

  **MaybeResponsive<"" | MaybeTwoValuesShorthandProperty\<PaddingKeyword>>**

  **Default: '' - meaning no override**

  Adjust the block-padding.

  - `large none` means block-start padding is `large`, block-end padding is `none`.

  This overrides the block value of `padding`.

  `paddingBlock` also accepts a [responsive value](https://shopify.dev/docs/api/app-home/using-polaris-components#responsive-values) string with the supported PaddingKeyword as a query value.

- **paddingBlockEnd**

  **MaybeResponsive<"" | PaddingKeyword>**

  **Default: '' - meaning no override**

  Adjust the block-end padding.

  This overrides the block-end value of `paddingBlock`.

  `paddingBlockEnd` also accepts a [responsive value](https://shopify.dev/docs/api/app-home/using-polaris-components#responsive-values) string with the supported PaddingKeyword as a query value.

- **paddingBlockStart**

  **MaybeResponsive<"" | PaddingKeyword>**

  **Default: '' - meaning no override**

  Adjust the block-start padding.

  This overrides the block-start value of `paddingBlock`.

  `paddingBlockStart` also accepts a [responsive value](https://shopify.dev/docs/api/app-home/using-polaris-components#responsive-values) string with the supported PaddingKeyword as a query value.

- **paddingInline**

  **MaybeResponsive<"" | MaybeTwoValuesShorthandProperty\<PaddingKeyword>>**

  **Default: '' - meaning no override**

  Adjust the inline padding.

  - `large none` means inline-start padding is `large`, inline-end padding is `none`.

  This overrides the inline value of `padding`.

  `paddingInline` also accepts a [responsive value](https://shopify.dev/docs/api/app-home/using-polaris-components#responsive-values) string with the supported PaddingKeyword as a query value.

- **paddingInlineEnd**

  **MaybeResponsive<"" | PaddingKeyword>**

  **Default: '' - meaning no override**

  Adjust the inline-end padding.

  This overrides the inline-end value of `paddingInline`.

  `paddingInlineEnd` also accepts a [responsive value](https://shopify.dev/docs/api/app-home/using-polaris-components#responsive-values) string with the supported PaddingKeyword as a query value.

- **paddingInlineStart**

  **MaybeResponsive<"" | PaddingKeyword>**

  **Default: '' - meaning no override**

  Adjust the inline-start padding.

  This overrides the inline-start value of `paddingInline`.

  `paddingInlineStart` also accepts a [responsive value](https://shopify.dev/docs/api/app-home/using-polaris-components#responsive-values) string with the supported PaddingKeyword as a query value.

- **placeContent**

  **"normal normal" | "normal stretch" | "normal start" | "normal end" | "normal center" | "normal unsafe start" | "normal unsafe end" | "normal unsafe center" | "normal safe start" | "normal safe end" | "normal safe center" | "stretch normal" | "stretch stretch" | "stretch start" | "stretch end" | "stretch center" | "stretch unsafe start" | "stretch unsafe end" | "stretch unsafe center" | "stretch safe start" | "stretch safe end" | "stretch safe center" | "baseline normal" | "baseline stretch" | "baseline start" | "baseline end" | "baseline center" | "baseline unsafe start" | "baseline unsafe end" | "baseline unsafe center" | "baseline safe start" | "baseline safe end" | "baseline safe center" | "first baseline normal" | "first baseline stretch" | "first baseline start" | "first baseline end" | "first baseline center" | "first baseline unsafe start" | "first baseline unsafe end" | "first baseline unsafe center" | "first baseline safe start" | "first baseline safe end" | "first baseline safe center" | "last baseline normal" | "last baseline stretch" | "last baseline start" | "last baseline end" | "last baseline center" | "last baseline unsafe start" | "last baseline unsafe end" | "last baseline unsafe center" | "last baseline safe start" | "last baseline safe end" | "last baseline safe center" | "start normal" | "start stretch" | "start start" | "start end" | "start center" | "start unsafe start" | "start unsafe end" | "start unsafe center" | "start safe start" | "start safe end" | "start safe center" | "end normal" | "end stretch" | "end start" | "end end" | "end center" | "end unsafe start" | "end unsafe end" | "end unsafe center" | "end safe start" | "end safe end" | "end safe center" | "center normal" | "center stretch" | "center start" | "center end" | "center center" | "center unsafe start" | "center unsafe end" | "center unsafe center" | "center safe start" | "center safe end" | "center safe center" | "unsafe start normal" | "unsafe start stretch" | "unsafe start start" | "unsafe start end" | "unsafe start center" | "unsafe start unsafe start" | "unsafe start unsafe end" | "unsafe start unsafe center" | "unsafe start safe start" | "unsafe start safe end" | "unsafe start safe center" | "unsafe end normal" | "unsafe end stretch" | "unsafe end start" | "unsafe end end" | "unsafe end center" | "unsafe end unsafe start" | "unsafe end unsafe end" | "unsafe end unsafe center" | "unsafe end safe start" | "unsafe end safe end" | "unsafe end safe center" | "unsafe center normal" | "unsafe center stretch" | "unsafe center start" | "unsafe center end" | "unsafe center center" | "unsafe center unsafe start" | "unsafe center unsafe end" | "unsafe center unsafe center" | "unsafe center safe start" | "unsafe center safe end" | "unsafe center safe center" | "safe start normal" | "safe start stretch" | "safe start start" | "safe start end" | "safe start center" | "safe start unsafe start" | "safe start unsafe end" | "safe start unsafe center" | "safe start safe start" | "safe start safe end" | "safe start safe center" | "safe end normal" | "safe end stretch" | "safe end start" | "safe end end" | "safe end center" | "safe end unsafe start" | "safe end unsafe end" | "safe end unsafe center" | "safe end safe start" | "safe end safe end" | "safe end safe center" | "safe center normal" | "safe center stretch" | "safe center start" | "safe center end" | "safe center center" | "safe center unsafe start" | "safe center unsafe end" | "safe center unsafe center" | "safe center safe start" | "safe center safe end" | "safe center safe center" | AlignContentKeyword | "normal space-between" | "normal space-around" | "normal space-evenly" | "baseline space-between" | "baseline space-around" | "baseline space-evenly" | "first baseline space-between" | "first baseline space-around" | "first baseline space-evenly" | "last baseline space-between" | "last baseline space-around" | "last baseline space-evenly" | "start space-between" | "start space-around" | "start space-evenly" | "end space-between" | "end space-around" | "end space-evenly" | "center space-between" | "center space-around" | "center space-evenly" | "unsafe start space-between" | "unsafe start space-around" | "unsafe start space-evenly" | "unsafe end space-between" | "unsafe end space-around" | "unsafe end space-evenly" | "unsafe center space-between" | "unsafe center space-around" | "unsafe center space-evenly" | "safe start space-between" | "safe start space-around" | "safe start space-evenly" | "safe end space-between" | "safe end space-around" | "safe end space-evenly" | "safe center space-between" | "safe center space-around" | "safe center space-evenly" | "stretch space-between" | "stretch space-around" | "stretch space-evenly" | "space-between normal" | "space-between start" | "space-between end" | "space-between center" | "space-between unsafe start" | "space-between unsafe end" | "space-between unsafe center" | "space-between safe start" | "space-between safe end" | "space-between safe center" | "space-between stretch" | "space-between space-between" | "space-between space-around" | "space-between space-evenly" | "space-around normal" | "space-around start" | "space-around end" | "space-around center" | "space-around unsafe start" | "space-around unsafe end" | "space-around unsafe center" | "space-around safe start" | "space-around safe end" | "space-around safe center" | "space-around stretch" | "space-around space-between" | "space-around space-around" | "space-around space-evenly" | "space-evenly normal" | "space-evenly start" | "space-evenly end" | "space-evenly center" | "space-evenly unsafe start" | "space-evenly unsafe end" | "space-evenly unsafe center" | "space-evenly safe start" | "space-evenly safe end" | "space-evenly safe center" | "space-evenly stretch" | "space-evenly space-between" | "space-evenly space-around" | "space-evenly space-evenly"**

  **Default: 'normal normal'**

  A shorthand property for `justify-content` and `align-content`.

- **placeItems**

  **AlignItemsKeyword | "normal normal" | "normal stretch" | "normal baseline" | "normal first baseline" | "normal last baseline" | "normal start" | "normal end" | "normal center" | "normal unsafe start" | "normal unsafe end" | "normal unsafe center" | "normal safe start" | "normal safe end" | "normal safe center" | "stretch normal" | "stretch stretch" | "stretch baseline" | "stretch first baseline" | "stretch last baseline" | "stretch start" | "stretch end" | "stretch center" | "stretch unsafe start" | "stretch unsafe end" | "stretch unsafe center" | "stretch safe start" | "stretch safe end" | "stretch safe center" | "baseline normal" | "baseline stretch" | "baseline baseline" | "baseline first baseline" | "baseline last baseline" | "baseline start" | "baseline end" | "baseline center" | "baseline unsafe start" | "baseline unsafe end" | "baseline unsafe center" | "baseline safe start" | "baseline safe end" | "baseline safe center" | "first baseline normal" | "first baseline stretch" | "first baseline baseline" | "first baseline first baseline" | "first baseline last baseline" | "first baseline start" | "first baseline end" | "first baseline center" | "first baseline unsafe start" | "first baseline unsafe end" | "first baseline unsafe center" | "first baseline safe start" | "first baseline safe end" | "first baseline safe center" | "last baseline normal" | "last baseline stretch" | "last baseline baseline" | "last baseline first baseline" | "last baseline last baseline" | "last baseline start" | "last baseline end" | "last baseline center" | "last baseline unsafe start" | "last baseline unsafe end" | "last baseline unsafe center" | "last baseline safe start" | "last baseline safe end" | "last baseline safe center" | "start normal" | "start stretch" | "start baseline" | "start first baseline" | "start last baseline" | "start start" | "start end" | "start center" | "start unsafe start" | "start unsafe end" | "start unsafe center" | "start safe start" | "start safe end" | "start safe center" | "end normal" | "end stretch" | "end baseline" | "end first baseline" | "end last baseline" | "end start" | "end end" | "end center" | "end unsafe start" | "end unsafe end" | "end unsafe center" | "end safe start" | "end safe end" | "end safe center" | "center normal" | "center stretch" | "center baseline" | "center first baseline" | "center last baseline" | "center start" | "center end" | "center center" | "center unsafe start" | "center unsafe end" | "center unsafe center" | "center safe start" | "center safe end" | "center safe center" | "unsafe start normal" | "unsafe start stretch" | "unsafe start baseline" | "unsafe start first baseline" | "unsafe start last baseline" | "unsafe start start" | "unsafe start end" | "unsafe start center" | "unsafe start unsafe start" | "unsafe start unsafe end" | "unsafe start unsafe center" | "unsafe start safe start" | "unsafe start safe end" | "unsafe start safe center" | "unsafe end normal" | "unsafe end stretch" | "unsafe end baseline" | "unsafe end first baseline" | "unsafe end last baseline" | "unsafe end start" | "unsafe end end" | "unsafe end center" | "unsafe end unsafe start" | "unsafe end unsafe end" | "unsafe end unsafe center" | "unsafe end safe start" | "unsafe end safe end" | "unsafe end safe center" | "unsafe center normal" | "unsafe center stretch" | "unsafe center baseline" | "unsafe center first baseline" | "unsafe center last baseline" | "unsafe center start" | "unsafe center end" | "unsafe center center" | "unsafe center unsafe start" | "unsafe center unsafe end" | "unsafe center unsafe center" | "unsafe center safe start" | "unsafe center safe end" | "unsafe center safe center" | "safe start normal" | "safe start stretch" | "safe start baseline" | "safe start first baseline" | "safe start last baseline" | "safe start start" | "safe start end" | "safe start center" | "safe start unsafe start" | "safe start unsafe end" | "safe start unsafe center" | "safe start safe start" | "safe start safe end" | "safe start safe center" | "safe end normal" | "safe end stretch" | "safe end baseline" | "safe end first baseline" | "safe end last baseline" | "safe end start" | "safe end end" | "safe end center" | "safe end unsafe start" | "safe end unsafe end" | "safe end unsafe center" | "safe end safe start" | "safe end safe end" | "safe end safe center" | "safe center normal" | "safe center stretch" | "safe center baseline" | "safe center first baseline" | "safe center last baseline" | "safe center start" | "safe center end" | "safe center center" | "safe center unsafe start" | "safe center unsafe end" | "safe center unsafe center" | "safe center safe start" | "safe center safe end" | "safe center safe center"**

  **Default: 'normal normal'**

  A shorthand property for `justify-items` and `align-items`.

- **rowGap**

  **MaybeResponsive<"" | SpacingKeyword>**

  **Default: '' - meaning no override**

  Adjust spacing between elements in the block axis.

  This overrides the row value of `gap`. `rowGap` either accepts:

  - a single [SpacingKeyword](https://shopify.dev/docs/api/app-home/using-polaris-components#scale) value (e.g. `large-100`)
  - OR a [responsive value](https://shopify.dev/docs/api/app-home/using-polaris-components#responsive-values) string with the supported SpacingKeyword as a query value.

### AccessibilityRole

```ts
'main' | 'header' | 'footer' | 'section' | 'region' | 'aside' | 'navigation' | 'ordered-list' | 'list-item' | 'list-item-separator' | 'unordered-list' | 'separator' | 'status' | 'alert' | 'generic' | 'presentation' | 'none'
```

### AlignContentKeyword

Align content sets the distribution of space between and around content items along a flexbox's cross axis, or a grid or block-level element's block axis.

```ts
'normal' | BaselinePosition | ContentDistribution | OverflowPosition | ContentPosition
```

### BaselinePosition

```ts
'baseline' | 'first baseline' | 'last baseline'
```

### ContentDistribution

```ts
'space-between' | 'space-around' | 'space-evenly' | 'stretch'
```

### OverflowPosition

```ts
`unsafe ${ContentPosition}` | `safe ${ContentPosition}`
```

### ContentPosition

```ts
'center' | 'start' | 'end'
```

### AlignItemsKeyword

Align items sets the align-self value on all direct children as a group.

```ts
'normal' | 'stretch' | BaselinePosition | OverflowPosition | ContentPosition
```

### BackgroundColorKeyword

```ts
'transparent' | ColorKeyword
```

### ColorKeyword

```ts
'subdued' | 'base' | 'strong'
```

### SizeUnitsOrAuto

```ts
SizeUnits | 'auto'
```

### SizeUnits

```ts
`${number}px` | `${number}%` | `0`
```

### BorderShorthand

Represents a shorthand for defining a border. It can be a combination of size, optionally followed by color, optionally followed by style.

```ts
BorderSizeKeyword | `${BorderSizeKeyword} ${ColorKeyword}` | `${BorderSizeKeyword} ${ColorKeyword} ${BorderStyleKeyword}`
```

### BorderSizeKeyword

```ts
SizeKeyword | 'none'
```

### SizeKeyword

```ts
'small-500' | 'small-400' | 'small-300' | 'small-200' | 'small-100' | 'small' | 'base' | 'large' | 'large-100' | 'large-200' | 'large-300' | 'large-400' | 'large-500'
```

### BorderStyleKeyword

```ts
'none' | 'solid' | 'dashed' | 'dotted' | 'auto'
```

### MaybeAllValuesShorthandProperty

```ts
T | `${T} ${T}` | `${T} ${T} ${T}` | `${T} ${T} ${T} ${T}`
```

### BoxBorderRadii

```ts
'small' | 'small-200' | 'small-100' | 'base' | 'large' | 'large-100' | 'large-200' | 'none'
```

### BoxBorderStyles

```ts
'auto' | 'none' | 'solid' | 'dashed'
```

### MaybeResponsive

```ts
T | `@container${string}`
```

### SpacingKeyword

```ts
SizeKeyword | 'none'
```

### MaybeTwoValuesShorthandProperty

```ts
T | `${T} ${T}`
```

### JustifyContentKeyword

Justify content defines how the browser distributes space between and around content items along the main-axis of a flex container, and the inline axis of a grid container.

```ts
'normal' | ContentDistribution | OverflowPosition | ContentPosition
```

### JustifyItemsKeyword

Justify items defines the default justify-self for all items of the box, giving them all a default way of justifying each box along the appropriate axis.

```ts
'normal' | 'stretch' | BaselinePosition | OverflowPosition | ContentPosition
```

### SizeUnitsOrNone

```ts
SizeUnits | 'none'
```

### PaddingKeyword

```ts
SizeKeyword | 'none'
```

## Slots

- **children**

  **HTMLElement**

  The content of the Grid.

## GridItem

Display content within a single item of a grid layout.

- **accessibilityLabel**

  **string**

  A label that describes the purpose or contents of the element. When set, it will be announced to users using assistive technologies and will provide them with more context.

  Only use this when the element's content is not enough context for users using assistive technologies.

- **accessibilityRole**

  **AccessibilityRole**

  **Default: 'generic'**

  Sets the semantic meaning of the component’s content. When set, the role will be used by assistive technologies to help users navigate the page.

- **accessibilityVisibility**

  **"visible" | "hidden" | "exclusive"**

  **Default: 'visible'**

  Changes the visibility of the element.

  - `visible`: the element is visible to all users.
  - `hidden`: the element is removed from the accessibility tree but remains visible.
  - `exclusive`: the element is visually hidden but remains in the accessibility tree.

- **background**

  **BackgroundColorKeyword**

  **Default: 'transparent'**

  Adjust the background of the component.

- **blockSize**

  **SizeUnitsOrAuto**

  **Default: 'auto'**

  Adjust the [block size](https://developer.mozilla.org/en-US/docs/Web/CSS/block-size).

- **border**

  **BorderShorthand**

  **Default: 'none' - equivalent to \`none base auto\`.**

  Set the border via the shorthand property.

  This can be a size, optionally followed by a color, optionally followed by a style.

  If the color is not specified, it will be `base`.

  If the style is not specified, it will be `auto`.

  Values can be overridden by `borderWidth`, `borderStyle`, and `borderColor`.

- **borderColor**

  **"" | ColorKeyword**

  **Default: '' - meaning no override**

  Adjust the color of the border.

- **borderRadius**

  **MaybeAllValuesShorthandProperty\<BoxBorderRadii>**

  **Default: 'none'**

  Adjust the radius of the border.

- **borderStyle**

  **"" | MaybeAllValuesShorthandProperty\<BoxBorderStyles>**

  **Default: '' - meaning no override**

  Adjust the style of the border.

- **borderWidth**

  **"" | MaybeAllValuesShorthandProperty<"small" | "small-100" | "base" | "large" | "large-100" | "none">**

  **Default: '' - meaning no override**

  Adjust the width of the border.

- **display**

  **MaybeResponsive<"auto" | "none">**

  **Default: 'auto'**

  Sets the outer [display](https://developer.mozilla.org/en-US/docs/Web/CSS/display) type of the component. The outer type sets a component's participation in [flow layout](https://developer.mozilla.org/en-US/docs/Web/CSS/CSS_flow_layout).

  - `auto` the component's initial value. The actual value depends on the component and context.
  - `none` hides the component from display and removes it from the accessibility tree, making it invisible to screen readers.

- **gridColumn**

  **"auto" | \`span ${number}\`**

  **Default: 'auto'**

  Number of columns the item will span across

- **gridRow**

  **"auto" | \`span ${number}\`**

  **Default: 'auto'**

  Number of rows the item will span across

- **inlineSize**

  **SizeUnitsOrAuto**

  **Default: 'auto'**

  Adjust the [inline size](https://developer.mozilla.org/en-US/docs/Web/CSS/inline-size).

- **maxBlockSize**

  **SizeUnitsOrNone**

  **Default: 'none'**

  Adjust the [maximum block size](https://developer.mozilla.org/en-US/docs/Web/CSS/max-block-size).

- **maxInlineSize**

  **SizeUnitsOrNone**

  **Default: 'none'**

  Adjust the [maximum inline size](https://developer.mozilla.org/en-US/docs/Web/CSS/max-inline-size).

- **minBlockSize**

  **SizeUnits**

  **Default: '0'**

  Adjust the [minimum block size](https://developer.mozilla.org/en-US/docs/Web/CSS/min-block-size).

- **minInlineSize**

  **SizeUnits**

  **Default: '0'**

  Adjust the [minimum inline size](https://developer.mozilla.org/en-US/docs/Web/CSS/min-inline-size).

- **overflow**

  **"visible" | "hidden"**

  **Default: 'visible'**

  Sets the overflow behavior of the element.

  - `hidden`: clips the content when it is larger than the element’s container. The element will not be scrollable and the users will not be able to access the clipped content by dragging or using a scroll wheel on a mouse.
  - `visible`: the content that extends beyond the element’s container is visible.

- **padding**

  **MaybeResponsive\<MaybeAllValuesShorthandProperty\<PaddingKeyword>>**

  **Default: 'none'**

  Adjust the padding of all edges.

  [1-to-4-value syntax](https://developer.mozilla.org/en-US/docs/Web/CSS/Shorthand_properties#edges_of_a_box) is supported. Note that, contrary to the CSS, it uses flow-relative values and the order is:

  - 4 values: `block-start inline-end block-end inline-start`
  - 3 values: `block-start inline block-end`
  - 2 values: `block inline`

  For example:

  - `large` means block-start, inline-end, block-end and inline-start paddings are `large`.
  - `large none` means block-start and block-end paddings are `large`, inline-start and inline-end paddings are `none`.
  - `large none large` means block-start padding is `large`, inline-end padding is `none`, block-end padding is `large` and inline-start padding is `none`.
  - `large none large small` means block-start padding is `large`, inline-end padding is `none`, block-end padding is `large` and inline-start padding is `small`.

  A padding value of `auto` will use the default padding for the closest container that has had its usual padding removed.

  `padding` also accepts a [responsive value](https://shopify.dev/docs/api/app-home/using-polaris-components#responsive-values) string with the supported PaddingKeyword as a query value.

- **paddingBlock**

  **MaybeResponsive<"" | MaybeTwoValuesShorthandProperty\<PaddingKeyword>>**

  **Default: '' - meaning no override**

  Adjust the block-padding.

  - `large none` means block-start padding is `large`, block-end padding is `none`.

  This overrides the block value of `padding`.

  `paddingBlock` also accepts a [responsive value](https://shopify.dev/docs/api/app-home/using-polaris-components#responsive-values) string with the supported PaddingKeyword as a query value.

- **paddingBlockEnd**

  **MaybeResponsive<"" | PaddingKeyword>**

  **Default: '' - meaning no override**

  Adjust the block-end padding.

  This overrides the block-end value of `paddingBlock`.

  `paddingBlockEnd` also accepts a [responsive value](https://shopify.dev/docs/api/app-home/using-polaris-components#responsive-values) string with the supported PaddingKeyword as a query value.

- **paddingBlockStart**

  **MaybeResponsive<"" | PaddingKeyword>**

  **Default: '' - meaning no override**

  Adjust the block-start padding.

  This overrides the block-start value of `paddingBlock`.

  `paddingBlockStart` also accepts a [responsive value](https://shopify.dev/docs/api/app-home/using-polaris-components#responsive-values) string with the supported PaddingKeyword as a query value.

- **paddingInline**

  **MaybeResponsive<"" | MaybeTwoValuesShorthandProperty\<PaddingKeyword>>**

  **Default: '' - meaning no override**

  Adjust the inline padding.

  - `large none` means inline-start padding is `large`, inline-end padding is `none`.

  This overrides the inline value of `padding`.

  `paddingInline` also accepts a [responsive value](https://shopify.dev/docs/api/app-home/using-polaris-components#responsive-values) string with the supported PaddingKeyword as a query value.

- **paddingInlineEnd**

  **MaybeResponsive<"" | PaddingKeyword>**

  **Default: '' - meaning no override**

  Adjust the inline-end padding.

  This overrides the inline-end value of `paddingInline`.

  `paddingInlineEnd` also accepts a [responsive value](https://shopify.dev/docs/api/app-home/using-polaris-components#responsive-values) string with the supported PaddingKeyword as a query value.

- **paddingInlineStart**

  **MaybeResponsive<"" | PaddingKeyword>**

  **Default: '' - meaning no override**

  Adjust the inline-start padding.

  This overrides the inline-start value of `paddingInline`.

  `paddingInlineStart` also accepts a [responsive value](https://shopify.dev/docs/api/app-home/using-polaris-components#responsive-values) string with the supported PaddingKeyword as a query value.

### AccessibilityRole

```ts
'main' | 'header' | 'footer' | 'section' | 'region' | 'aside' | 'navigation' | 'ordered-list' | 'list-item' | 'list-item-separator' | 'unordered-list' | 'separator' | 'status' | 'alert' | 'generic' | 'presentation' | 'none'
```

### BackgroundColorKeyword

```ts
'transparent' | ColorKeyword
```

### ColorKeyword

```ts
'subdued' | 'base' | 'strong'
```

### SizeUnitsOrAuto

```ts
SizeUnits | 'auto'
```

### SizeUnits

```ts
`${number}px` | `${number}%` | `0`
```

### BorderShorthand

Represents a shorthand for defining a border. It can be a combination of size, optionally followed by color, optionally followed by style.

```ts
BorderSizeKeyword | `${BorderSizeKeyword} ${ColorKeyword}` | `${BorderSizeKeyword} ${ColorKeyword} ${BorderStyleKeyword}`
```

### BorderSizeKeyword

```ts
SizeKeyword | 'none'
```

### SizeKeyword

```ts
'small-500' | 'small-400' | 'small-300' | 'small-200' | 'small-100' | 'small' | 'base' | 'large' | 'large-100' | 'large-200' | 'large-300' | 'large-400' | 'large-500'
```

### BorderStyleKeyword

```ts
'none' | 'solid' | 'dashed' | 'dotted' | 'auto'
```

### MaybeAllValuesShorthandProperty

```ts
T | `${T} ${T}` | `${T} ${T} ${T}` | `${T} ${T} ${T} ${T}`
```

### BoxBorderRadii

```ts
'small' | 'small-200' | 'small-100' | 'base' | 'large' | 'large-100' | 'large-200' | 'none'
```

### BoxBorderStyles

```ts
'auto' | 'none' | 'solid' | 'dashed'
```

### MaybeResponsive

```ts
T | `@container${string}`
```

### SizeUnitsOrNone

```ts
SizeUnits | 'none'
```

### PaddingKeyword

```ts
SizeKeyword | 'none'
```

### MaybeTwoValuesShorthandProperty

```ts
T | `${T} ${T}`
```

## Slots

- **children**

  **HTMLElement**

  The content of the GridItem.

Examples

### Examples

- #### Code

  ##### jsx

  ```jsx
  <s-grid
    gridTemplateColumns="repeat(2, 1fr)"
    gap="small"
    justifyContent="center"
  >
    <s-grid-item gridColumn="span 2" border="base" borderStyle="dashed">
      Summary of sales
    </s-grid-item>
    <s-grid-item gridColumn="span 1" border="base" borderStyle="dashed">
      Orders
    </s-grid-item>
    <s-grid-item gridColumn="auto" border="base" borderStyle="dashed">
      Customers
    </s-grid-item>
  </s-grid>
  ```

  ##### html

  ```html
  <s-grid
    gridTemplateColumns="repeat(2, 1fr)"
    gap="small"
    justifyContent="center"
  >
    <s-grid-item gridColumn="span 2" border="base" borderStyle="dashed">
      Summary of sales
    </s-grid-item>
    <s-grid-item gridColumn="span 1" border="base" borderStyle="dashed">
      Orders
    </s-grid-item>
    <s-grid-item gridColumn="auto" border="base" borderStyle="dashed">
      Customers
    </s-grid-item>
  </s-grid>
  ```

- #### Basic two-column layout

  ##### Description

  Simple 12-column grid system with equal-width left and right columns.

  ##### jsx

  ```jsx
  <s-grid gridTemplateColumns="repeat(12, 1fr)" gap="base">
    <s-grid-item gridColumn="span 6" gridRow="span 1">
      <s-section>
        <s-text>Left column</s-text>
      </s-section>
    </s-grid-item>
    <s-grid-item gridColumn="span 6" gridRow="span 1">
      <s-section>
        <s-text>Right column</s-text>
      </s-section>
    </s-grid-item>
  </s-grid>
  ```

  ##### html

  ```html
  <s-grid gridTemplateColumns="repeat(12, 1fr)" gap="base">
    <s-grid-item gridColumn="span 6" gridRow="span 1">
      <s-section>
        <s-text>Left column</s-text>
      </s-section>
    </s-grid-item>
    <s-grid-item gridColumn="span 6" gridRow="span 1">
      <s-section>
        <s-text>Right column</s-text>
      </s-section>
    </s-grid-item>
  </s-grid>
  ```

- #### Layout with spans

  ##### Description

  Grid layout with full-width, half-width, and third-width column arrangements.

  ##### jsx

  ```jsx
  <s-stack gap="base">
    <s-grid gridTemplateColumns="repeat(12, 1fr)" gap="base">
      <s-grid-item gridColumn="span 12" gridRow="span 1">
        <s-section>
          <s-text>Full width field</s-text>
        </s-section>
      </s-grid-item>
      <s-grid-item gridColumn="span 6" gridRow="span 2">
        <s-section>
          <s-text>Half width field</s-text>
        </s-section>
      </s-grid-item>
      <s-grid-item gridColumn="span 6" gridRow="span 2">
        <s-section>
          <s-text>Half width field</s-text>
        </s-section>
      </s-grid-item>
      <s-grid-item gridColumn="span 4" gridRow="span 3">
        <s-section>
          <s-text>Third width field</s-text>
        </s-section>
      </s-grid-item>
      <s-grid-item gridColumn="span 4" gridRow="span 3">
        <s-section>
          <s-text>Third width field</s-text>
        </s-section>
      </s-grid-item>
      <s-grid-item gridColumn="span 4" gridRow="span 3">
        <s-section>
          <s-text>Third width field</s-text>
        </s-section>
      </s-grid-item>
    </s-grid>
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack gap="base">
    <s-grid gridTemplateColumns="repeat(12, 1fr)" gap="base">
      <s-grid-item gridColumn="span 12" gridRow="span 1">
        <s-section>
          <s-text>Full width field</s-text>
        </s-section>
      </s-grid-item>
      <s-grid-item gridColumn="span 6" gridRow="span 2">
        <s-section>
          <s-text>Half width field</s-text>
        </s-section>
      </s-grid-item>
      <s-grid-item gridColumn="span 6" gridRow="span 2">
        <s-section>
          <s-text>Half width field</s-text>
        </s-section>
      </s-grid-item>
      <s-grid-item gridColumn="span 4" gridRow="span 3">
        <s-section>
          <s-text>Third width field</s-text>
        </s-section>
      </s-grid-item>
      <s-grid-item gridColumn="span 4" gridRow="span 3">
        <s-section>
          <s-text>Third width field</s-text>
        </s-section>
      </s-grid-item>
      <s-grid-item gridColumn="span 4" gridRow="span 3">
        <s-section>
          <s-text>Third width field</s-text>
        </s-section>
      </s-grid-item>
    </s-grid>
  </s-stack>
  ```

- #### Responsive grid

  ##### Description

  Adaptive grid that automatically adjusts column count based on screen size.

  ##### jsx

  ```jsx
  <s-stack gap="base">
    <s-text type="strong">Narrow container (375px)</s-text>
    <s-box inlineSize="375px">
      <s-query-container>
        <s-grid
          gridTemplateColumns="@container (inline-size > 400px) 1fr 1fr 1fr, 1fr"
          gap="base"
        >
          <s-grid-item>
            <s-box padding="small" background="subdued">
              <s-text>Item 1</s-text>
            </s-box>
          </s-grid-item>
          <s-grid-item>
            <s-box padding="small" background="subdued">
              <s-text>Item 2</s-text>
            </s-box>
          </s-grid-item>
          <s-grid-item>
            <s-box padding="small" background="subdued">
              <s-text>Item 3</s-text>
            </s-box>
          </s-grid-item>
        </s-grid>
      </s-query-container>
    </s-box>

    <s-text type="strong">Wide container (450px)</s-text>
    <s-box inlineSize="450px">
      <s-query-container>
        <s-grid
          gridTemplateColumns="@container (inline-size > 400px) 1fr 1fr 1fr, 1fr"
          gap="base"
        >
          <s-grid-item>
            <s-box padding="small" background="subdued">
              <s-text>Item 1</s-text>
            </s-box>
          </s-grid-item>
          <s-grid-item>
            <s-box padding="small" background="subdued">
              <s-text>Item 2</s-text>
            </s-box>
          </s-grid-item>
          <s-grid-item>
            <s-box padding="small" background="subdued">
              <s-text>Item 3</s-text>
            </s-box>
          </s-grid-item>
        </s-grid>
      </s-query-container>
    </s-box>
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack gap="base">
    <s-text type="strong">Narrow container (375px)</s-text>
    <s-box inlineSize="375px">
      <s-query-container>
        <s-grid
          gridTemplateColumns="@container (inline-size > 400px) 1fr 1fr 1fr, 1fr"
          gap="base"
        >
          <s-grid-item>
            <s-box padding="small" background="subdued">
              <s-text>Item 1</s-text>
            </s-box>
          </s-grid-item>
          <s-grid-item>
            <s-box padding="small" background="subdued">
              <s-text>Item 2</s-text>
            </s-box>
          </s-grid-item>
          <s-grid-item>
            <s-box padding="small" background="subdued">
              <s-text>Item 3</s-text>
            </s-box>
          </s-grid-item>
        </s-grid>
      </s-query-container>
    </s-box>

    <s-text type="strong">Wide container (450px)</s-text>
    <s-box inlineSize="450px">
      <s-query-container>
        <s-grid
          gridTemplateColumns="@container (inline-size > 400px) 1fr 1fr 1fr, 1fr"
          gap="base"
        >
          <s-grid-item>
            <s-box padding="small" background="subdued">
              <s-text>Item 1</s-text>
            </s-box>
          </s-grid-item>
          <s-grid-item>
            <s-box padding="small" background="subdued">
              <s-text>Item 2</s-text>
            </s-box>
          </s-grid-item>
          <s-grid-item>
            <s-box padding="small" background="subdued">
              <s-text>Item 3</s-text>
            </s-box>
          </s-grid-item>
        </s-grid>
      </s-query-container>
    </s-box>
  </s-stack>
  ```

## Best practices

- Always configure layout properties when using Grid. At minimum, set gridTemplateColumns to define your column structure (e.g., repeat(12, 1fr) for a 12-column grid)
- Use gap to add spacing between grid items rather than adding margins to individual items
- Combine gridTemplateColumns with gridColumn on GridItem components to control how items span across columns

## Useful for

- Building form layouts where you want more than one field on the same row.
- Organizing content into a grid-like layout with columns and rows with consistent alignment and spacing.
- Creating responsive layouts with consistent spacing.

## Considerations

- Grid doesn't include any padding by default. If you need padding around your grid, use `base` to apply the default padding.
- Grid will allow children to overflow unless template rows/columns are properly set.
- Grid will always wrap children to a new line. If you need to control the wrapping behavior, use `s-stack` or `s-box`.

</page>

<page>
---
title: OrderedList
description: >-
  Displays a numbered list of related items in a specific sequence. Use to
  present step-by-step instructions, ranked items, or procedures where order
  matters.
api_name: app-home
source_url:
  html: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/layout-and-structure/orderedlist
  md: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/layout-and-structure/orderedlist.md
---

# Ordered​List

Displays a numbered list of related items in a specific sequence. Use to present step-by-step instructions, ranked items, or procedures where order matters.

## Slots

- **children**

  **HTMLElement**

  The items of the OrderedList.

  Only ListItems are accepted.

## ListItem

Represents a single item within an unordered or ordered list. Use only as a child of `s-unordered-list` or `s-ordered-list` components.

## Slots

- **children**

  **HTMLElement**

  The content of the ListItem.

Examples

### Examples

- #### Code

  ##### jsx

  ```jsx
  <s-ordered-list>
    <s-list-item>Red shirt</s-list-item>
    <s-list-item>Green shirt</s-list-item>
    <s-list-item>Blue shirt</s-list-item>
  </s-ordered-list>
  ```

  ##### html

  ```html
  <s-ordered-list>
    <s-list-item>Red shirt</s-list-item>
    <s-list-item>Green shirt</s-list-item>
    <s-list-item>Blue shirt</s-list-item>
  </s-ordered-list>
  ```

- #### Basic usage

  ##### Description

  Demonstrates a simple ordered list with three sequential steps.

  ##### jsx

  ```jsx
  <s-ordered-list>
    <s-list-item>Add products to your catalog</s-list-item>
    <s-list-item>Set up payment methods</s-list-item>
    <s-list-item>Configure shipping zones</s-list-item>
  </s-ordered-list>
  ```

  ##### html

  ```html
  <s-ordered-list>
    <s-list-item>Add products to your catalog</s-list-item>
    <s-list-item>Set up payment methods</s-list-item>
    <s-list-item>Configure shipping zones</s-list-item>
  </s-ordered-list>
  ```

- #### Order processing steps

  ##### Description

  Shows an ordered list with multiple steps in a workflow process.

  ##### jsx

  ```jsx
  <s-ordered-list>
    <s-list-item>Review order details and customer information</s-list-item>
    <s-list-item>Verify payment and billing address</s-list-item>
    <s-list-item>Check inventory availability for all items</s-list-item>
    <s-list-item>Generate fulfillment labels and packing slip</s-list-item>
    <s-list-item>Package items and update tracking information</s-list-item>
    <s-list-item>Send shipment confirmation to customer</s-list-item>
  </s-ordered-list>
  ```

  ##### html

  ```html
  <s-ordered-list>
    <s-list-item>Review order details and customer information</s-list-item>
    <s-list-item>Verify payment and billing address</s-list-item>
    <s-list-item>Check inventory availability for all items</s-list-item>
    <s-list-item>Generate fulfillment labels and packing slip</s-list-item>
    <s-list-item>Package items and update tracking information</s-list-item>
    <s-list-item>Send shipment confirmation to customer</s-list-item>
  </s-ordered-list>
  ```

- #### Product setup instructions

  ##### Description

  Illustrates a nested ordered list with sub-steps within main steps.

  ##### jsx

  ```jsx
  <s-ordered-list>
    <s-list-item>
      Create product listing with title and description
      <s-ordered-list>
        <s-list-item>Add high-quality product images</s-list-item>
        <s-list-item>Set SEO title and meta description</s-list-item>
      </s-ordered-list>
    </s-list-item>
    <s-list-item>Configure pricing and inventory tracking</s-list-item>
    <s-list-item>Set up product variants (size, color, material)</s-list-item>
    <s-list-item>Enable inventory tracking and set stock levels</s-list-item>
    <s-list-item>Review and publish product to storefront</s-list-item>
  </s-ordered-list>
  ```

  ##### html

  ```html
  <s-ordered-list>
    <s-list-item>
      Create product listing with title and description
      <s-ordered-list>
        <s-list-item>Add high-quality product images</s-list-item>
        <s-list-item>Set SEO title and meta description</s-list-item>
      </s-ordered-list>
    </s-list-item>
    <s-list-item>Configure pricing and inventory tracking</s-list-item>
    <s-list-item>Set up product variants (size, color, material)</s-list-item>
    <s-list-item>Enable inventory tracking and set stock levels</s-list-item>
    <s-list-item>Review and publish product to storefront</s-list-item>
  </s-ordered-list>
  ```

- #### Fulfillment process

  ##### Description

  Displays a complex nested list with multiple levels of sub-steps.

  ##### jsx

  ```jsx
  <s-ordered-list>
    <s-list-item>
      Process payment
      <s-ordered-list>
        <s-list-item>Verify card details</s-list-item>
        <s-list-item>Apply discount codes</s-list-item>
        <s-list-item>Calculate taxes</s-list-item>
      </s-ordered-list>
    </s-list-item>
    <s-list-item>
      Prepare shipment
      <s-ordered-list>
        <s-list-item>Print shipping label</s-list-item>
        <s-list-item>Pack items securely</s-list-item>
      </s-ordered-list>
    </s-list-item>
    <s-list-item>Update customer with tracking info</s-list-item>
  </s-ordered-list>
  ```

  ##### html

  ```html
  <s-ordered-list>
    <s-list-item>
      Process payment
      <s-ordered-list>
        <s-list-item>Verify card details</s-list-item>
        <s-list-item>Apply discount codes</s-list-item>
        <s-list-item>Calculate taxes</s-list-item>
      </s-ordered-list>
    </s-list-item>
    <s-list-item>
      Prepare shipment
      <s-ordered-list>
        <s-list-item>Print shipping label</s-list-item>
        <s-list-item>Pack items securely</s-list-item>
      </s-ordered-list>
    </s-list-item>
    <s-list-item>Update customer with tracking info</s-list-item>
  </s-ordered-list>
  ```

## Best practices

- Use to break up related content and improve scannability
- Phrase items consistently (start each with a noun or verb)
- Start each item with a capital letter
- Don't use commas or semicolons at the end of lines

</page>

<page>
---
title: QueryContainer
description: >-
  Establishes a query container for responsive design. Use `s-query-container`
  to define an element as a containment context, enabling child components or
  styles to adapt based on the container’s size.
api_name: app-home
source_url:
  html: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/layout-and-structure/querycontainer
  md: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/layout-and-structure/querycontainer.md
---

# Query​Container

Establishes a query container for responsive design. Use `s-query-container` to define an element as a containment context, enabling child components or styles to adapt based on the container’s size.

## Properties

Use to define an element as a containment context, enabling child components or styles to adapt based on the container’s size.

- **containerName**

  **string**

  **Default: ''**

  The name of the container, which can be used in your container queries to target this container specifically.

  We place the container name of `s-default` on every container. Because of this, it is not required to add a `containerName` identifier in your queries. For example, a `@container (inline-size <= 300px) none, auto` query is equivalent to `@container s-default (inline-size <= 300px) none, auto`.

  Any value set in `containerName` will be set alongside alongside `s-default`. For example, `containerName="my-container-name"` will result in a value of `s-default my-container-name` set on the `container-name` CSS property of the rendered HTML.

## Slots

- **children**

  **HTMLElement**

  The content of the container.

Examples

### Examples

- ####

  ##### jsx

  ```jsx
  <s-query-container>
    <s-box
      padding="@container (inline-size > 500px) large-500, none"
      background="strong"
    >
      Padding is applied when the inline-size > 500px
    </s-box>
  </s-query-container>
  ```

  ##### html

  ```html
  <s-query-container>
    <s-box
      padding="@container (inline-size > 500px) large-500, none"
      background="strong"
    >
      Padding is applied when the inline-size '>' 500px
    </s-box>
  </s-query-container>
  ```

- #### Basic Usage

  ##### Description

  Demonstrates the simplest way to use QueryContainer, wrapping content with a named container context.

  ##### jsx

  ```jsx
  <>
  <s-box inlineSize="375px">
    <s-query-container id="product-section" containerName="product">
      <s-box padding="@container product (inline-size > 400px) large-500, none" borderWidth="base" borderColor="base" borderRadius="base">
        <s-text>Padding is different depending on the container size</s-text>
      </s-box>
    </s-query-container>
  </s-box>

  <s-box inlineSize="450px">
    <s-query-container id="product-section" containerName="product">
      <s-box padding="@container product (inline-size > 400px) large-500, none" borderWidth="base" borderColor="base" borderRadius="base">
        <s-text>Padding is different depending on the container size</s-text>
      </s-box>
    </s-query-container>
  </s-box>
  </>
  ```

  ##### html

  ```html
  <s-box inlineSize="375px">
    <s-query-container id="product-section" containerName="product">
      <s-box padding="@container product (inline-size > 400px) large-500, none">
        <s-text>Padding is different depending on the container size</s-text>
      </s-box>
    </s-query-container>
  </s-box>

  <s-box inlineSize="450px">
    <s-query-container id="product-section" containerName="product">
      <s-box padding="@container product (inline-size > 400px) large-500, none">
        <s-text>Padding is different depending on the container size</s-text>
      </s-box>
    </s-query-container>
  </s-box>
  ```

</page>

<page>
---
title: Section
description: >-
  Groups related content into clearly-defined thematic areas. Sections have
  contextual styling that automatically adapts based on nesting depth. They also
  adjust heading levels to maintain a meaningful and accessible page structure.
api_name: app-home
source_url:
  html: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/layout-and-structure/section
  md: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/layout-and-structure/section.md
---

# Section

Groups related content into clearly-defined thematic areas. Sections have contextual styling that automatically adapts based on nesting depth. They also adjust heading levels to maintain a meaningful and accessible page structure.

## Properties

- **accessibilityLabel**

  **string**

  A label used to describe the section that will be announced by assistive technologies.

  When no `heading` property is provided or included as a children of the Section, you **must** provide an `accessibilityLabel` to describe the Section. This is important as it allows assistive technologies to provide the right context to users.

- **heading**

  **string**

  A title that describes the content of the section.

- **padding**

  **"base" | "none"**

  **Default: 'base'**

  Adjust the padding of all edges.

  - `base`: applies padding that is appropriate for the element. Note that it may result in no padding if this is the right design decision in a particular context.
  - `none`: removes all padding from the element. This can be useful when elements inside the Section need to span to the edge of the Section. For example, a full-width image. In this case, rely on `s-box` with a padding of 'base' to bring back the desired padding for the rest of the content.

## Slots

- **children**

  **HTMLElement**

  The content of the Section.

Examples

### Examples

- #### Code

  ##### jsx

  ```jsx
  <s-section heading="Online store dashboard">
    <s-paragraph>View a summary of your online store’s performance.</s-paragraph>
  </s-section>
  ```

  ##### html

  ```html
  <s-section heading="Online store dashboard">
    <s-paragraph>View a summary of your online store’s performance.</s-paragraph>
  </s-section>
  ```

- #### Top-Level Section with Form Elements

  ##### Description

  Demonstrates a level 1 section with a heading and multiple form fields. This example shows how sections provide visual hierarchy and structure for input elements.

  ##### jsx

  ```jsx
  <s-section heading="Customer information">
    <s-text-field label="First name" value="John" />
    <s-text-field label="Last name" value="Doe" />
    <s-email-field label="Email" value="john@example.com" />
  </s-section>
  ```

  ##### html

  ```html
  <!-- Level 1 section - elevated with shadow on desktop -->
  <s-section heading="Customer information">
    <s-text-field label="First name" value="John"></s-text-field>
    <s-text-field label="Last name" value="Doe"></s-text-field>
    <s-email-field label="Email" value="john@example.com"></s-email-field>
  </s-section>
  ```

- #### Nested Sections with Visual Level Differences

  ##### Description

  Illustrates how sections can be nested to create a hierarchical layout, with each nested section automatically adjusting its visual style. This example demonstrates the automatic visual leveling of nested sections.

  ##### jsx

  ```jsx
  <s-stack gap="base">
    {/* Level 1 section */}
    <s-section heading="Order details">
      <s-paragraph>Order #1234 placed on January 15, 2024</s-paragraph>

      {/* Level 2 section - nested with different visual treatment */}
      <s-section heading="Customer">
        <s-text-field label="Name" value="Jane Smith" />
        <s-text-field label="Email" value="jane@example.com" />

        {/* Level 3 section - further nested */}
        <s-section heading="Billing address">
          <s-text-field label="Street" value="123 Main St" />
          <s-text-field label="City" value="Toronto" />
        </s-section>
      </s-section>

      {/* Another Level 2 section */}
      <s-section heading="Items">
        <s-paragraph>2 items totaling $49.99</s-paragraph>
      </s-section>
    </s-section>
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack gap="base">
    <!-- Level 1 section -->
    <s-section heading="Order details">
      <s-paragraph>Order #1234 placed on January 15, 2024</s-paragraph>

      <!-- Level 2 section - nested with different visual treatment -->
      <s-section heading="Customer">
        <s-text-field label="Name" value="Jane Smith"></s-text-field>
        <s-text-field label="Email" value="jane@example.com"></s-text-field>

        <!-- Level 3 section - further nested -->
        <s-section heading="Billing address">
          <s-text-field label="Street" value="123 Main St"></s-text-field>
          <s-text-field label="City" value="Toronto"></s-text-field>
        </s-section>
      </s-section>

      <!-- Another Level 2 section -->
      <s-section heading="Items">
        <s-paragraph>2 items totaling $49.99</s-paragraph>
      </s-section>
    </s-section>
  </s-stack>
  ```

- #### Section with Accessibility Label

  ##### Description

  Shows how to add an accessibility label to a section, providing an additional hidden heading for screen readers while maintaining a visible heading.

  ##### jsx

  ```jsx
  <s-section
    heading="Payment summary"
    accessibilityLabel="Order payment breakdown and totals"
  >
    <s-stack>
      <s-paragraph>Subtotal: $42.99</s-paragraph>
      <s-paragraph>Tax: $5.59</s-paragraph>
      <s-paragraph>Shipping: $1.41</s-paragraph>
      <s-paragraph>
        <s-text type="strong">Total: $49.99</s-text>  
      </s-paragraph>
    </s-stack>
  </s-section>
  ```

  ##### html

  ```html
  <s-section
    heading="Payment summary"
    accessibilityLabel="Order payment breakdown and totals"
  >
    <s-stack gap="base">
      <s-paragraph>Subtotal: $42.99</s-paragraph>
      <s-paragraph>Tax: $5.59</s-paragraph>
      <s-paragraph>Shipping: $1.41</s-paragraph>
      <s-paragraph>
        <s-text type="strong">Total: $49.99</s-text>
      </s-paragraph>
    </s-stack>
  </s-section>
  ```

- #### Full-width Content Layout

  ##### Description

  Demonstrates using a section with \`padding="none"\` to create a full-width layout, ideal for displaying tables or other content that requires edge-to-edge rendering.

  ##### jsx

  ```jsx
  <s-section padding="none">
    <s-table>
      <s-table-header-row>
        <s-table-header listSlot="primary">Product</s-table-header>
        <s-table-header listSlot="labeled">Price</s-table-header>
        <s-table-header listSlot="inline">Status</s-table-header>
      </s-table-header-row>
      <s-table-body>
        <s-table-row>
          <s-table-cell>Cotton t-shirt</s-table-cell>
          <s-table-cell>$29.99</s-table-cell>
          <s-table-cell>
            <s-badge tone="success">Active</s-badge>
          </s-table-cell>
        </s-table-row>
      </s-table-body>
    </s-table>
  </s-section>
  ```

  ##### html

  ```html
  <s-section padding="none">
    <s-table>
      <s-table-header-row>
        <s-table-header listSlot="primary">Product</s-table-header>
        <s-table-header listSlot="labeled">Price</s-table-header>
        <s-table-header listSlot="inline">Status</s-table-header>
      </s-table-header-row>
      <s-table-body>
        <s-table-row>
          <s-table-cell>Cotton t-shirt</s-table-cell>
          <s-table-cell>$29.99</s-table-cell>
          <s-table-cell><s-badge tone="success">Active</s-badge></s-table-cell>
        </s-table-row>
      </s-table-body>
    </s-table>
  </s-section>
  ```

## Useful for

- Organizing your page in a logical structure based on nesting levels.
- Creating consistency across pages.

## Considerations

- When adding headings inside sections they automatically use a specific style, which helps keep the content organized and clear.
- Built-in spacing is added between nested sections, as well as between heading and content.
- **Level 1:** Display as responsive cards with background color, rounded corners, and shadow effects. Includes outer padding that can be removed when content like `s-table` needs to expand to the full width of the section.
- **Nested sections:** Don't have any background color or effects by default.

</page>

<page>
---
title: Stack
description: >-
  Organizes elements horizontally or vertically along the block or inline axis.
  Use to structure layouts, group related components, or control spacing between
  elements.
api_name: app-home
source_url:
  html: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/layout-and-structure/stack
  md: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/layout-and-structure/stack.md
---

# Stack

Organizes elements horizontally or vertically along the block or inline axis. Use to structure layouts, group related components, or control spacing between elements.

## Properties

- **accessibilityLabel**

  **string**

  A label that describes the purpose or contents of the element. When set, it will be announced to users using assistive technologies and will provide them with more context.

  Only use this when the element's content is not enough context for users using assistive technologies.

- **accessibilityRole**

  **AccessibilityRole**

  **Default: 'generic'**

  Sets the semantic meaning of the component’s content. When set, the role will be used by assistive technologies to help users navigate the page.

- **accessibilityVisibility**

  **"visible" | "hidden" | "exclusive"**

  **Default: 'visible'**

  Changes the visibility of the element.

  - `visible`: the element is visible to all users.
  - `hidden`: the element is removed from the accessibility tree but remains visible.
  - `exclusive`: the element is visually hidden but remains in the accessibility tree.

- **alignContent**

  **AlignContentKeyword**

  **Default: 'normal'**

  Aligns the Stack's children along the block axis.

  This overrides the block value of `alignContent`.

- **alignItems**

  **AlignItemsKeyword**

  **Default: 'normal'**

  Aligns the Stack's children along the block axis.

- **background**

  **BackgroundColorKeyword**

  **Default: 'transparent'**

  Adjust the background of the component.

- **blockSize**

  **SizeUnitsOrAuto**

  **Default: 'auto'**

  Adjust the [block size](https://developer.mozilla.org/en-US/docs/Web/CSS/block-size).

- **border**

  **BorderShorthand**

  **Default: 'none' - equivalent to \`none base auto\`.**

  Set the border via the shorthand property.

  This can be a size, optionally followed by a color, optionally followed by a style.

  If the color is not specified, it will be `base`.

  If the style is not specified, it will be `auto`.

  Values can be overridden by `borderWidth`, `borderStyle`, and `borderColor`.

- **borderColor**

  **"" | ColorKeyword**

  **Default: '' - meaning no override**

  Adjust the color of the border.

- **borderRadius**

  **MaybeAllValuesShorthandProperty\<BoxBorderRadii>**

  **Default: 'none'**

  Adjust the radius of the border.

- **borderStyle**

  **"" | MaybeAllValuesShorthandProperty\<BoxBorderStyles>**

  **Default: '' - meaning no override**

  Adjust the style of the border.

- **borderWidth**

  **"" | MaybeAllValuesShorthandProperty<"small" | "small-100" | "base" | "large" | "large-100" | "none">**

  **Default: '' - meaning no override**

  Adjust the width of the border.

- **columnGap**

  **MaybeResponsive<"" | SpacingKeyword>**

  **Default: '' - meaning no override**

  Adjust spacing between elements in the inline axis.

  This overrides the column value of `gap`. `columnGap` either accepts:

  - a single [SpacingKeyword](https://shopify.dev/docs/api/app-home/using-polaris-components#scale) value (e.g. `large-100`)
  - OR a [responsive value](https://shopify.dev/docs/api/app-home/using-polaris-components#responsive-values) string with the supported SpacingKeyword as a query value.

- **direction**

  **MaybeResponsive<"inline" | "block">**

  **Default: 'block'**

  Sets how the Stack's children are placed within the Stack.

  `direction` either accepts:

  - a single value either `inline` or `block`
  - OR a [responsive value](https://shopify.dev/docs/api/app-home/using-polaris-components#responsive-values) string with the supported SpacingKeyword as a query value.

- **display**

  **MaybeResponsive<"auto" | "none">**

  **Default: 'auto'**

  Sets the outer [display](https://developer.mozilla.org/en-US/docs/Web/CSS/display) type of the component. The outer type sets a component's participation in [flow layout](https://developer.mozilla.org/en-US/docs/Web/CSS/CSS_flow_layout).

  - `auto` the component's initial value. The actual value depends on the component and context.
  - `none` hides the component from display and removes it from the accessibility tree, making it invisible to screen readers.

- **gap**

  **MaybeResponsive\<MaybeTwoValuesShorthandProperty\<SpacingKeyword>>**

  **Default: 'none'**

  Adjust spacing between elements.

  `gap` can either accept:

  - a single [SpacingKeyword](https://shopify.dev/docs/api/app-home/using-polaris-components#scale) value applied to both axes (e.g. `large-100`)
  - OR a pair of values (eg `large-100 large-500`) can be used to set the inline and block axes respectively
  - OR a [responsive value](https://shopify.dev/docs/api/app-home/using-polaris-components#responsive-values) string with the supported SpacingKeyword as a query value.

- **inlineSize**

  **SizeUnitsOrAuto**

  **Default: 'auto'**

  Adjust the [inline size](https://developer.mozilla.org/en-US/docs/Web/CSS/inline-size).

- **justifyContent**

  **JustifyContentKeyword**

  **Default: 'normal'**

  Aligns the Stack's children along the inline axis.

- **maxBlockSize**

  **SizeUnitsOrNone**

  **Default: 'none'**

  Adjust the [maximum block size](https://developer.mozilla.org/en-US/docs/Web/CSS/max-block-size).

- **maxInlineSize**

  **SizeUnitsOrNone**

  **Default: 'none'**

  Adjust the [maximum inline size](https://developer.mozilla.org/en-US/docs/Web/CSS/max-inline-size).

- **minBlockSize**

  **SizeUnits**

  **Default: '0'**

  Adjust the [minimum block size](https://developer.mozilla.org/en-US/docs/Web/CSS/min-block-size).

- **minInlineSize**

  **SizeUnits**

  **Default: '0'**

  Adjust the [minimum inline size](https://developer.mozilla.org/en-US/docs/Web/CSS/min-inline-size).

- **overflow**

  **"visible" | "hidden"**

  **Default: 'visible'**

  Sets the overflow behavior of the element.

  - `hidden`: clips the content when it is larger than the element’s container. The element will not be scrollable and the users will not be able to access the clipped content by dragging or using a scroll wheel on a mouse.
  - `visible`: the content that extends beyond the element’s container is visible.

- **padding**

  **MaybeResponsive\<MaybeAllValuesShorthandProperty\<PaddingKeyword>>**

  **Default: 'none'**

  Adjust the padding of all edges.

  [1-to-4-value syntax](https://developer.mozilla.org/en-US/docs/Web/CSS/Shorthand_properties#edges_of_a_box) is supported. Note that, contrary to the CSS, it uses flow-relative values and the order is:

  - 4 values: `block-start inline-end block-end inline-start`
  - 3 values: `block-start inline block-end`
  - 2 values: `block inline`

  For example:

  - `large` means block-start, inline-end, block-end and inline-start paddings are `large`.
  - `large none` means block-start and block-end paddings are `large`, inline-start and inline-end paddings are `none`.
  - `large none large` means block-start padding is `large`, inline-end padding is `none`, block-end padding is `large` and inline-start padding is `none`.
  - `large none large small` means block-start padding is `large`, inline-end padding is `none`, block-end padding is `large` and inline-start padding is `small`.

  A padding value of `auto` will use the default padding for the closest container that has had its usual padding removed.

  `padding` also accepts a [responsive value](https://shopify.dev/docs/api/app-home/using-polaris-components#responsive-values) string with the supported PaddingKeyword as a query value.

- **paddingBlock**

  **MaybeResponsive<"" | MaybeTwoValuesShorthandProperty\<PaddingKeyword>>**

  **Default: '' - meaning no override**

  Adjust the block-padding.

  - `large none` means block-start padding is `large`, block-end padding is `none`.

  This overrides the block value of `padding`.

  `paddingBlock` also accepts a [responsive value](https://shopify.dev/docs/api/app-home/using-polaris-components#responsive-values) string with the supported PaddingKeyword as a query value.

- **paddingBlockEnd**

  **MaybeResponsive<"" | PaddingKeyword>**

  **Default: '' - meaning no override**

  Adjust the block-end padding.

  This overrides the block-end value of `paddingBlock`.

  `paddingBlockEnd` also accepts a [responsive value](https://shopify.dev/docs/api/app-home/using-polaris-components#responsive-values) string with the supported PaddingKeyword as a query value.

- **paddingBlockStart**

  **MaybeResponsive<"" | PaddingKeyword>**

  **Default: '' - meaning no override**

  Adjust the block-start padding.

  This overrides the block-start value of `paddingBlock`.

  `paddingBlockStart` also accepts a [responsive value](https://shopify.dev/docs/api/app-home/using-polaris-components#responsive-values) string with the supported PaddingKeyword as a query value.

- **paddingInline**

  **MaybeResponsive<"" | MaybeTwoValuesShorthandProperty\<PaddingKeyword>>**

  **Default: '' - meaning no override**

  Adjust the inline padding.

  - `large none` means inline-start padding is `large`, inline-end padding is `none`.

  This overrides the inline value of `padding`.

  `paddingInline` also accepts a [responsive value](https://shopify.dev/docs/api/app-home/using-polaris-components#responsive-values) string with the supported PaddingKeyword as a query value.

- **paddingInlineEnd**

  **MaybeResponsive<"" | PaddingKeyword>**

  **Default: '' - meaning no override**

  Adjust the inline-end padding.

  This overrides the inline-end value of `paddingInline`.

  `paddingInlineEnd` also accepts a [responsive value](https://shopify.dev/docs/api/app-home/using-polaris-components#responsive-values) string with the supported PaddingKeyword as a query value.

- **paddingInlineStart**

  **MaybeResponsive<"" | PaddingKeyword>**

  **Default: '' - meaning no override**

  Adjust the inline-start padding.

  This overrides the inline-start value of `paddingInline`.

  `paddingInlineStart` also accepts a [responsive value](https://shopify.dev/docs/api/app-home/using-polaris-components#responsive-values) string with the supported PaddingKeyword as a query value.

- **rowGap**

  **MaybeResponsive<"" | SpacingKeyword>**

  **Default: '' - meaning no override**

  Adjust spacing between elements in the block axis.

  This overrides the row value of `gap`. `rowGap` either accepts:

  - a single [SpacingKeyword](https://shopify.dev/docs/api/app-home/using-polaris-components#scale) value (e.g. `large-100`)
  - OR a [responsive value](https://shopify.dev/docs/api/app-home/using-polaris-components#responsive-values) string with the supported SpacingKeyword as a query value.

### AccessibilityRole

```ts
'main' | 'header' | 'footer' | 'section' | 'region' | 'aside' | 'navigation' | 'ordered-list' | 'list-item' | 'list-item-separator' | 'unordered-list' | 'separator' | 'status' | 'alert' | 'generic' | 'presentation' | 'none'
```

### AlignContentKeyword

Align content sets the distribution of space between and around content items along a flexbox's cross axis, or a grid or block-level element's block axis.

```ts
'normal' | BaselinePosition | ContentDistribution | OverflowPosition | ContentPosition
```

### BaselinePosition

```ts
'baseline' | 'first baseline' | 'last baseline'
```

### ContentDistribution

```ts
'space-between' | 'space-around' | 'space-evenly' | 'stretch'
```

### OverflowPosition

```ts
`unsafe ${ContentPosition}` | `safe ${ContentPosition}`
```

### ContentPosition

```ts
'center' | 'start' | 'end'
```

### AlignItemsKeyword

Align items sets the align-self value on all direct children as a group.

```ts
'normal' | 'stretch' | BaselinePosition | OverflowPosition | ContentPosition
```

### BackgroundColorKeyword

```ts
'transparent' | ColorKeyword
```

### ColorKeyword

```ts
'subdued' | 'base' | 'strong'
```

### SizeUnitsOrAuto

```ts
SizeUnits | 'auto'
```

### SizeUnits

```ts
`${number}px` | `${number}%` | `0`
```

### BorderShorthand

Represents a shorthand for defining a border. It can be a combination of size, optionally followed by color, optionally followed by style.

```ts
BorderSizeKeyword | `${BorderSizeKeyword} ${ColorKeyword}` | `${BorderSizeKeyword} ${ColorKeyword} ${BorderStyleKeyword}`
```

### BorderSizeKeyword

```ts
SizeKeyword | 'none'
```

### SizeKeyword

```ts
'small-500' | 'small-400' | 'small-300' | 'small-200' | 'small-100' | 'small' | 'base' | 'large' | 'large-100' | 'large-200' | 'large-300' | 'large-400' | 'large-500'
```

### BorderStyleKeyword

```ts
'none' | 'solid' | 'dashed' | 'dotted' | 'auto'
```

### MaybeAllValuesShorthandProperty

```ts
T | `${T} ${T}` | `${T} ${T} ${T}` | `${T} ${T} ${T} ${T}`
```

### BoxBorderRadii

```ts
'small' | 'small-200' | 'small-100' | 'base' | 'large' | 'large-100' | 'large-200' | 'none'
```

### BoxBorderStyles

```ts
'auto' | 'none' | 'solid' | 'dashed'
```

### MaybeResponsive

```ts
T | `@container${string}`
```

### SpacingKeyword

```ts
SizeKeyword | 'none'
```

### MaybeTwoValuesShorthandProperty

```ts
T | `${T} ${T}`
```

### JustifyContentKeyword

Justify content defines how the browser distributes space between and around content items along the main-axis of a flex container, and the inline axis of a grid container.

```ts
'normal' | ContentDistribution | OverflowPosition | ContentPosition
```

### SizeUnitsOrNone

```ts
SizeUnits | 'none'
```

### PaddingKeyword

```ts
SizeKeyword | 'none'
```

## Slots

- **children**

  **HTMLElement**

  The content of the Stack.

Examples

### Examples

- #### Code

  ##### jsx

  ```jsx
  <s-stack gap="base">
    <s-badge>Paid</s-badge>
    <s-badge>Processing</s-badge>
    <s-badge>Filled</s-badge>
    <s-badge>Completed</s-badge>
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack gap="base">
    <s-badge>Paid</s-badge>
    <s-badge>Processing</s-badge>
    <s-badge>Filled</s-badge>
    <s-badge>Completed</s-badge>
  </s-stack>
  ```

- #### Basic block stack (vertical)

  ##### Description

  Default vertical stacking layout with consistent spacing between text elements.

  ##### jsx

  ```jsx
  <s-stack gap="base">
    <s-text>First item</s-text>
    <s-text>Second item</s-text>
    <s-text>Third item</s-text>
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack gap="base">
    <s-text>First item</s-text>
    <s-text>Second item</s-text>
    <s-text>Third item</s-text>
  </s-stack>
  ```

- #### Inline stack (horizontal)

  ##### Description

  Horizontal layout for arranging badges or similar elements side by side.

  ##### jsx

  ```jsx
  <s-stack direction="inline" gap="large-100">
    <s-badge>Item 1</s-badge>
    <s-badge>Item 2</s-badge>
    <s-badge>Item 3</s-badge>
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack direction="inline" gap="large-100">
    <s-badge>Item 1</s-badge>
    <s-badge>Item 2</s-badge>
    <s-badge>Item 3</s-badge>
  </s-stack>
  ```

- #### Responsive stack with container queries

  ##### Description

  Advanced responsive layout that changes direction and spacing based on container size.

  ##### jsx

  ```jsx
  <s-query-container>
    <s-stack
      direction="@container (inline-size > 500px) inline, block"
      gap="@container (inline-size > 500px) base, small-300"
    >
      <s-box
        padding="large-100"
        borderColor="base"
        borderWidth="small"
        borderRadius="large-100"
      >
        Content 1
      </s-box>
      <s-box
        padding="large-100"
        borderColor="base"
        borderWidth="small"
        borderRadius="large-100"
      >
        Content 2
      </s-box>
    </s-stack>
  </s-query-container>
  ```

  ##### html

  ```html
  <s-query-container>
    <s-stack
      direction="@container (inline-size > 500px) inline, block"
      gap="@container (inline-size > 500px) base, small-300"
    >
      <s-box
        padding="large-100"
        borderColor="base"
        borderWidth="small"
        borderRadius="large-100"
      >
        Content 1
      </s-box>
      <s-box
        padding="large-100"
        borderColor="base"
        borderWidth="small"
        borderRadius="large-100"
      >
        Content 2
      </s-box>
    </s-stack>
  </s-query-container>
  ```

- #### Custom alignment

  ##### Description

  Horizontal stack with space-between justification and center alignment for balanced layouts.

  ##### jsx

  ```jsx
  <s-stack direction="inline" justifyContent="space-between" alignItems="center">
    <s-text>Left aligned</s-text>
    <s-text>Centered text</s-text>
    <s-text>Right aligned</s-text>
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack direction="inline" justifyContent="space-between" alignItems="center">
    <s-text>Left aligned</s-text>
    <s-text>Centered text</s-text>
    <s-text>Right aligned</s-text>
  </s-stack>
  ```

- #### Custom spacing

  ##### Description

  Stack with custom gap values and separate row/column gap controls for precise spacing.

  ##### jsx

  ```jsx
  <s-stack gap="large-100 large-500" rowGap="large-300" columnGap="large-200">
    <s-box
      padding="large-100"
      borderColor="base"
      borderWidth="small"
      borderRadius="large-100"
    >
      Box with custom spacing
    </s-box>
    <s-box
      padding="large-100"
      borderColor="base"
      borderWidth="small"
      borderRadius="large-100"
    >
      Another box
    </s-box>
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack gap="large-100 large-500" rowGap="large-300" columnGap="large-200">
    <s-box
      padding="large-100"
      borderColor="base"
      borderWidth="small"
      borderRadius="large-100"
    >
      Box with custom spacing
    </s-box>
    <s-box
      padding="large-100"
      borderColor="base"
      borderWidth="small"
      borderRadius="large-100"
    >
      Another box
    </s-box>
  </s-stack>
  ```

## Useful for

- Placing non-form items in rows or columns when sections don't work for your layout.
- Controlling the spacing between elements.
- For form layouts where you need multiple input fields on the same row, use `s-grid` instead.

## Considerations

- Stack doesn't add any padding by default. If you want padding around your stacked elements, use `base` to apply the default padding.
- When spacing becomes limited, Stack will always wrap children to a new line.

## Best practices

- Use smaller gaps between small elements and larger gaps between big ones.
- Maintain consistent spacing in stacks across all pages of your app.

</page>

<page>
---
title: Table
description: >-
  Display data clearly in rows and columns, helping users view, analyze, and
  compare information. Automatically renders as a list on small screens and a
  table on large ones.
api_name: app-home
source_url:
  html: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/layout-and-structure/table
  md: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/layout-and-structure/table.md
---

# Table

Display data clearly in rows and columns, helping users view, analyze, and compare information. Automatically renders as a list on small screens and a table on large ones.

## Properties

- **hasNextPage**

  **boolean**

  **Default: false**

  Whether there's an additional page of data.

- **hasPreviousPage**

  **boolean**

  **Default: false**

  Whether there's a previous page of data.

- **loading**

  **boolean**

  **Default: false**

  Whether the table is in a loading state, such as initial page load or loading the next page in a paginated table. When true, the table could be in an inert state, which prevents user interaction.

- **paginate**

  **boolean**

  **Default: false**

  Whether to use pagination controls.

- **variant**

  **"auto" | "list"**

  **Default: 'auto'**

  Sets the layout of the Table.

  - `list`: The Table is always displayed as a list.
  - `table`: The Table is always displayed as a table.
  - `auto`: The Table is displayed as a table on wide devices and as a list on narrow devices.

## Slots

- **children**

  **HTMLElement**

  The content of the Table.

- **filters**

  **HTMLElement**

  Additional filters to display in the table. For example, the `s-search-field` component can be used to filter the table data.

## Events

Learn more about [registering events](https://shopify.dev/docs/api/app-home/using-polaris-components#event-handling).

- **nextpage**

  **CallbackEventListener\<typeof tagName> | null**

- **previouspage**

  **CallbackEventListener\<typeof tagName> | null**

### CallbackEventListener

```ts
(EventListener & {
      (event: CallbackEvent<T>): void;
    }) | null
```

### CallbackEvent

```ts
Event & {
  currentTarget: HTMLElementTagNameMap[T];
}
```

## TableBody

Define the main content area of a table, containing rows and cells that display data.

## Slots

- **children**

  **HTMLElement**

  The body of the table. May not have any semantic meaning in the Table's `list` variant.

## TableCell

Display data within a cell in a table row.

## Slots

- **children**

  **HTMLElement**

  The content of the table cell.

## TableHeader

Display column names at the top of a table.

- **format**

  **HeaderFormat**

  The format of the column. Will automatically apply styling and alignment to cell content based on the value.

  - `base`: The base format for columns.
  - `currency`: Formats the column as currency.
  - `numeric`: Formats the column as a number.

- **listSlot**

  **ListSlotType**

  **Default: 'labeled'**

  Content designation for the table's `list` variant.

  - `primary`: The most important content. Only one column can have this designation.
  - `secondary`: The secondary content. Only one column can have this designation.
  - `kicker`: Content that is displayed before primary and secondary content, but with less visual prominence. Only one column can have this designation.
  - `inline`: Content that is displayed inline.
  - `labeled`: Each column with this designation displays as a heading-content pair.

### HeaderFormat

```ts
'base' | 'numeric' | 'currency'
```

### ListSlotType

```ts
'primary' | 'secondary' | 'kicker' | 'inline' | 'labeled'
```

## Slots

- **children**

  **HTMLElement**

  The heading of the column in the `table` variant, and the label of its data in `list` variant.

## TableHeaderRow

Define a header row in a table, displaying column names and enabling sorting.

## Slots

- **children**

  **HTMLElement**

  Contents of the table heading row; children should be `TableHeading` components.

## TableRow

Display a row of data within the body of a table.

- **clickDelegate**

  **string**

  The ID of an interactive element (e.g. `s-link`) in the row that will be the target of the click when the row is clicked. This is the primary action for the row; it should not be used for secondary actions.

  This is a click-only affordance, and does not introduce any keyboard or screen reader affordances. Which is why the target element must be in the table; so that keyboard and screen reader users can interact with it normally.

## Slots

- **children**

  **HTMLElement**

  The content of a TableRow, which should be `TableCell` components.

Examples

### Examples

- #### Code

  ##### jsx

  ```jsx
  <s-section padding="none">
    <s-table>
      <s-table-header-row>
        <s-table-header>Name</s-table-header>
        <s-table-header>Email</s-table-header>
        <s-table-header format="numeric">Orders placed</s-table-header>
        <s-table-header>Phone</s-table-header>
      </s-table-header-row>
      <s-table-body>
        <s-table-row>
          <s-table-cell>John Smith</s-table-cell>
          <s-table-cell>john@example.com</s-table-cell>
          <s-table-cell>23</s-table-cell>
          <s-table-cell>123-456-7890</s-table-cell>
        </s-table-row>
        <s-table-row>
          <s-table-cell>Jane Johnson</s-table-cell>
          <s-table-cell>jane@example.com</s-table-cell>
          <s-table-cell>15</s-table-cell>
          <s-table-cell>234-567-8901</s-table-cell>
        </s-table-row>
        <s-table-row>
          <s-table-cell>Brandon Williams</s-table-cell>
          <s-table-cell>brandon@example.com</s-table-cell>
          <s-table-cell>42</s-table-cell>
          <s-table-cell>345-678-9012</s-table-cell>
        </s-table-row>
      </s-table-body>
    </s-table>
  </s-section>
  ```

  ##### html

  ```html
  <s-section padding="none">
    <s-table>
      <s-table-header-row>
        <s-table-header>Name</s-table-header>
        <s-table-header>Email</s-table-header>
        <s-table-header format="numeric">Orders placed</s-table-header>
        <s-table-header>Phone</s-table-header>
      </s-table-header-row>
      <s-table-body>
        <s-table-row>
          <s-table-cell>John Smith</s-table-cell>
          <s-table-cell>john@example.com</s-table-cell>
          <s-table-cell>23</s-table-cell>
          <s-table-cell>123-456-7890</s-table-cell>
        </s-table-row>
        <s-table-row>
          <s-table-cell>Jane Johnson</s-table-cell>
          <s-table-cell>jane@example.com</s-table-cell>
          <s-table-cell>15</s-table-cell>
          <s-table-cell>234-567-8901</s-table-cell>
        </s-table-row>
        <s-table-row>
          <s-table-cell>Brandon Williams</s-table-cell>
          <s-table-cell>brandon@example.com</s-table-cell>
          <s-table-cell>42</s-table-cell>
          <s-table-cell>345-678-9012</s-table-cell>
        </s-table-row>
      </s-table-body>
    </s-table>
  </s-section>
  ```

- #### Basic Usage

  ##### Description

  Tables expand to full width by default.

  ##### jsx

  ```jsx
  <s-section padding="none">
    <s-table>
      <s-table-header-row>
        <s-table-header listSlot="primary">Product</s-table-header>
        <s-table-header listSlot="inline">Status</s-table-header>
        <s-table-header listSlot="labeled">Inventory</s-table-header>
        <s-table-header listSlot="labeled">Price</s-table-header>
      </s-table-header-row>

      <s-table-body>
        <s-table-row>
          <s-table-cell>Water bottle</s-table-cell>
          <s-table-cell>
            <s-badge tone="success">Active</s-badge>
          </s-table-cell>
          <s-table-cell>128</s-table-cell>
          <s-table-cell>$24.99</s-table-cell>
        </s-table-row>
        <s-table-row>
          <s-table-cell>T-shirt</s-table-cell>
          <s-table-cell>
            <s-badge tone="warning">Low stock</s-badge>
          </s-table-cell>
          <s-table-cell>15</s-table-cell>
          <s-table-cell>$19.99</s-table-cell>
        </s-table-row>
        <s-table-row>
          <s-table-cell>Cutting board</s-table-cell>
          <s-table-cell>
            <s-badge tone="critical">Out of stock</s-badge>
          </s-table-cell>
          <s-table-cell>0</s-table-cell>
          <s-table-cell>$34.99</s-table-cell>
        </s-table-row>
      </s-table-body>
    </s-table>
  </s-section>
  ```

  ##### html

  ```html
  <s-section padding="none">
    <s-table>
      <s-table-header-row>
        <s-table-header listSlot="primary">Product</s-table-header>
        <s-table-header listSlot="inline">Status</s-table-header>
        <s-table-header listSlot="labeled">Inventory</s-table-header>
        <s-table-header listSlot="labeled">Price</s-table-header>
      </s-table-header-row>

      <s-table-body>
        <s-table-row>
          <s-table-cell>Water bottle</s-table-cell>
          <s-table-cell>
            <s-badge tone="success">Active</s-badge>
          </s-table-cell>
          <s-table-cell>128</s-table-cell>
          <s-table-cell>$24.99</s-table-cell>
        </s-table-row>
        <s-table-row>
          <s-table-cell>T-shirt</s-table-cell>
          <s-table-cell>
            <s-badge tone="warning">Low stock</s-badge>
          </s-table-cell>
          <s-table-cell>15</s-table-cell>
          <s-table-cell>$19.99</s-table-cell>
        </s-table-row>
        <s-table-row>
          <s-table-cell>Cutting board</s-table-cell>
          <s-table-cell>
            <s-badge tone="critical">Out of stock</s-badge>
          </s-table-cell>
          <s-table-cell>0</s-table-cell>
          <s-table-cell>$34.99</s-table-cell>
        </s-table-row>
      </s-table-body>
    </s-table>
  </s-section>
  ```

- #### With Pagination

  ##### Description

  Add pagination controls for navigating large datasets.

  ##### jsx

  ```jsx
  <s-section padding="none">
    <s-table paginate hasPreviousPage hasNextPage>
      <s-table-header-row>
        <s-table-header listSlot="primary">Product</s-table-header>
        <s-table-header listSlot="inline">Status</s-table-header>
        <s-table-header listSlot="secondary" format="numeric">Sales</s-table-header>
      </s-table-header-row>

      <s-table-body>
        <s-table-row>
          <s-table-cell>Product 1</s-table-cell>
          <s-table-cell>Active</s-table-cell>
          <s-table-cell>250</s-table-cell>
        </s-table-row>
        <s-table-row>
          <s-table-cell>Product 2</s-table-cell>
          <s-table-cell>Active</s-table-cell>
          <s-table-cell>180</s-table-cell>
        </s-table-row>
        <s-table-row>
          <s-table-cell>Product 3</s-table-cell>
          <s-table-cell>Paused</s-table-cell>
          <s-table-cell>95</s-table-cell>
        </s-table-row>
      </s-table-body>
    </s-table>
  </s-section>
  ```

  ##### html

  ```html
  <s-section padding="none">
    <s-table paginate hasPreviousPage hasNextPage>
      <s-table-header-row>
        <s-table-header listSlot="primary">Product</s-table-header>
        <s-table-header listSlot="inline">Status</s-table-header>
        <s-table-header listSlot="secondary" format="numeric">Sales</s-table-header>
      </s-table-header-row>

      <s-table-body>
        <s-table-row>
          <s-table-cell>Product 1</s-table-cell>
          <s-table-cell>Active</s-table-cell>
          <s-table-cell>250</s-table-cell>
        </s-table-row>
        <s-table-row>
          <s-table-cell>Product 2</s-table-cell>
          <s-table-cell>Active</s-table-cell>
          <s-table-cell>180</s-table-cell>
        </s-table-row>
        <s-table-row>
          <s-table-cell>Product 3</s-table-cell>
          <s-table-cell>Paused</s-table-cell>
          <s-table-cell>95</s-table-cell>
        </s-table-row>
      </s-table-body>
    </s-table>
  </s-section>
  ```

- #### With Loading State

  ##### Description

  Display a loading state while fetching data.

  ##### jsx

  ```jsx
  <s-section padding="none">
    <s-table loading>
      <s-table-header-row>
        <s-table-header listSlot="primary">Product</s-table-header>
        <s-table-header listSlot="inline">Status</s-table-header>
        <s-table-header listSlot="labeled">Inventory</s-table-header>
      </s-table-header-row>

      <s-table-body>
        <s-table-row>
          <s-table-cell>Water bottle</s-table-cell>
          <s-table-cell>
            <s-badge tone="success">Active</s-badge>
          </s-table-cell>
          <s-table-cell>128</s-table-cell>
        </s-table-row>
        <s-table-row>
          <s-table-cell>T-shirt</s-table-cell>
          <s-table-cell>
            <s-badge tone="warning">Low stock</s-badge>
          </s-table-cell>
          <s-table-cell>15</s-table-cell>
        </s-table-row>
        <s-table-row>
          <s-table-cell>Cutting board</s-table-cell>
          <s-table-cell>
            <s-badge tone="critical">Out of stock</s-badge>
          </s-table-cell>
          <s-table-cell>0</s-table-cell>
        </s-table-row>
        <s-table-row>
          <s-table-cell>Notebook set</s-table-cell>
          <s-table-cell>
            <s-badge tone="success">Active</s-badge>
          </s-table-cell>
          <s-table-cell>245</s-table-cell>
        </s-table-row>
      </s-table-body>
    </s-table>
  </s-section>
  ```

  ##### html

  ```html
  <s-section padding="none">
    <s-table loading>
      <s-table-header-row>
        <s-table-header listSlot="primary">Product</s-table-header>
        <s-table-header listSlot="inline">Status</s-table-header>
        <s-table-header listSlot="labeled">Inventory</s-table-header>
      </s-table-header-row>

      <s-table-body>
        <s-table-row>
          <s-table-cell>Water bottle</s-table-cell>
          <s-table-cell>
            <s-badge tone="success">Active</s-badge>
          </s-table-cell>
          <s-table-cell>128</s-table-cell>
        </s-table-row>
        <s-table-row>
          <s-table-cell>T-shirt</s-table-cell>
          <s-table-cell>
            <s-badge tone="warning">Low stock</s-badge>
          </s-table-cell>
          <s-table-cell>15</s-table-cell>
        </s-table-row>
        <s-table-row>
          <s-table-cell>Cutting board</s-table-cell>
          <s-table-cell>
            <s-badge tone="critical">Out of stock</s-badge>
          </s-table-cell>
          <s-table-cell>0</s-table-cell>
        </s-table-row>
        <s-table-row>
          <s-table-cell>Notebook set</s-table-cell>
          <s-table-cell>
            <s-badge tone="success">Active</s-badge>
          </s-table-cell>
          <s-table-cell>245</s-table-cell>
        </s-table-row>
      </s-table-body>
    </s-table>
  </s-section>
  ```

- #### Full-width table with multiple columns

  ##### Description

  Display multiple columns in a full-width table.

  ##### jsx

  ```jsx
  <s-section padding="none">
    <s-table>
      <s-table-header-row>
        <s-table-header listSlot="primary">Product</s-table-header>
        <s-table-header listSlot="kicker">SKU</s-table-header>
        <s-table-header listSlot="inline">Status</s-table-header>
        <s-table-header listSlot="labeled" format="numeric">Inventory</s-table-header>
        <s-table-header listSlot="labeled" format="currency">Price</s-table-header>
        <s-table-header listSlot="labeled">Last updated</s-table-header>
      </s-table-header-row>

      <s-table-body>
        <s-table-row>
          <s-table-cell>Water bottle</s-table-cell>
          <s-table-cell>WB-001</s-table-cell>
          <s-table-cell>
            <s-badge tone="success">Active</s-badge>
          </s-table-cell>
          <s-table-cell>128</s-table-cell>
          <s-table-cell>$24.99</s-table-cell>
          <s-table-cell>2 hours ago</s-table-cell>
        </s-table-row>
        <s-table-row>
          <s-table-cell>T-shirt</s-table-cell>
          <s-table-cell>TS-002</s-table-cell>
          <s-table-cell>
            <s-badge tone="warning">Low stock</s-badge>
          </s-table-cell>
          <s-table-cell>15</s-table-cell>
          <s-table-cell>$19.99</s-table-cell>
          <s-table-cell>1 day ago</s-table-cell>
        </s-table-row>
        <s-table-row>
          <s-table-cell>Cutting board</s-table-cell>
          <s-table-cell>CB-003</s-table-cell>
          <s-table-cell>
            <s-badge tone="critical">Out of stock</s-badge>
          </s-table-cell>
          <s-table-cell>0</s-table-cell>
          <s-table-cell>$34.99</s-table-cell>
          <s-table-cell>3 days ago</s-table-cell>
        </s-table-row>
        <s-table-row>
          <s-table-cell>Notebook set</s-table-cell>
          <s-table-cell>NB-004</s-table-cell>
          <s-table-cell>
            <s-badge tone="success">Active</s-badge>
          </s-table-cell>
          <s-table-cell>245</s-table-cell>
          <s-table-cell>$12.99</s-table-cell>
          <s-table-cell>5 hours ago</s-table-cell>
        </s-table-row>
        <s-table-row>
          <s-table-cell>Stainless steel straws</s-table-cell>
          <s-table-cell>SS-005</s-table-cell>
          <s-table-cell>
            <s-badge tone="success">Active</s-badge>
          </s-table-cell>
          <s-table-cell>89</s-table-cell>
          <s-table-cell>$9.99</s-table-cell>
          <s-table-cell>1 hour ago</s-table-cell>
        </s-table-row>
      </s-table-body>
    </s-table>
  </s-section>
  ```

  ##### html

  ```html
  <s-section padding="none">
    <s-table>
      <s-table-header-row>
        <s-table-header listSlot="primary">Product</s-table-header>
        <s-table-header listSlot="kicker">SKU</s-table-header>
        <s-table-header listSlot="inline">Status</s-table-header>
        <s-table-header listSlot="labeled" format="numeric">Inventory</s-table-header>
        <s-table-header listSlot="labeled" format="currency">Price</s-table-header>
        <s-table-header listSlot="labeled">Last updated</s-table-header>
      </s-table-header-row>

      <s-table-body>
        <s-table-row>
          <s-table-cell>Water bottle</s-table-cell>
          <s-table-cell>WB-001</s-table-cell>
          <s-table-cell>
            <s-badge tone="success">Active</s-badge>
          </s-table-cell>
          <s-table-cell>128</s-table-cell>
          <s-table-cell>$24.99</s-table-cell>
          <s-table-cell>2 hours ago</s-table-cell>
        </s-table-row>
        <s-table-row>
          <s-table-cell>T-shirt</s-table-cell>
          <s-table-cell>TS-002</s-table-cell>
          <s-table-cell>
            <s-badge tone="warning">Low stock</s-badge>
          </s-table-cell>
          <s-table-cell>15</s-table-cell>
          <s-table-cell>$19.99</s-table-cell>
          <s-table-cell>1 day ago</s-table-cell>
        </s-table-row>
        <s-table-row>
          <s-table-cell>Cutting board</s-table-cell>
          <s-table-cell>CB-003</s-table-cell>
          <s-table-cell>
            <s-badge tone="critical">Out of stock</s-badge>
          </s-table-cell>
          <s-table-cell>0</s-table-cell>
          <s-table-cell>$34.99</s-table-cell>
          <s-table-cell>3 days ago</s-table-cell>
        </s-table-row>
        <s-table-row>
          <s-table-cell>Notebook set</s-table-cell>
          <s-table-cell>NB-004</s-table-cell>
          <s-table-cell>
            <s-badge tone="success">Active</s-badge>
          </s-table-cell>
          <s-table-cell>245</s-table-cell>
          <s-table-cell>$12.99</s-table-cell>
          <s-table-cell>5 hours ago</s-table-cell>
        </s-table-row>
        <s-table-row>
          <s-table-cell>Stainless steel straws</s-table-cell>
          <s-table-cell>SS-005</s-table-cell>
          <s-table-cell>
            <s-badge tone="success">Active</s-badge>
          </s-table-cell>
          <s-table-cell>89</s-table-cell>
          <s-table-cell>$9.99</s-table-cell>
          <s-table-cell>1 hour ago</s-table-cell>
        </s-table-row>
      </s-table-body>
    </s-table>
  </s-section>
  ```

- #### List Variant

  ##### Description

  Display data using the list variant with specialized slot types. List is the default variant on mobile devices.

  ##### jsx

  ```jsx
  <s-section padding="none">  
    <s-table variant="list">
      <s-table-header-row>
        <s-table-header listSlot="kicker">ID</s-table-header>
        <s-table-header listSlot="primary">Customer</s-table-header>
        <s-table-header listSlot="secondary">Email</s-table-header>
        <s-table-header listSlot="inline">Status</s-table-header>
        <s-table-header listSlot="labeled" format="numeric">Orders</s-table-header>
        <s-table-header listSlot="labeled" format="currency">Total spent</s-table-header>
      </s-table-header-row>
      <s-table-body>
        <s-table-row>
          <s-table-cell>#1001</s-table-cell>
          <s-table-cell>Sarah Johnson</s-table-cell>
          <s-table-cell>sarah@example.com</s-table-cell>
          <s-table-cell>
            <s-badge tone="success">Active</s-badge>
          </s-table-cell>
          <s-table-cell>23</s-table-cell>
          <s-table-cell>$1,245.50</s-table-cell>
        </s-table-row>
        <s-table-row>
          <s-table-cell>#1002</s-table-cell>
          <s-table-cell>Mike Chen</s-table-cell>
          <s-table-cell>mike@example.com</s-table-cell>
          <s-table-cell>
            <s-badge tone="neutral">Inactive</s-badge>
          </s-table-cell>
          <s-table-cell>7</s-table-cell>
          <s-table-cell>$432.75</s-table-cell>
        </s-table-row>
        <s-table-row>
          <s-table-cell>#1003</s-table-cell>
          <s-table-cell>Emma Davis</s-table-cell>
          <s-table-cell>emma@example.com</s-table-cell>
          <s-table-cell>
            <s-badge tone="success">Active</s-badge>
          </s-table-cell>
          <s-table-cell>15</s-table-cell>
          <s-table-cell>$892.25</s-table-cell>
        </s-table-row>
      </s-table-body>
    </s-table>
  </s-section>
  ```

  ##### html

  ```html
  <s-section padding="none">
    <s-table variant="list">
      <s-table-header-row>
        <s-table-header listSlot="kicker">ID</s-table-header>
        <s-table-header listSlot="primary">Customer</s-table-header>
        <s-table-header listSlot="secondary">Email</s-table-header>
        <s-table-header listSlot="inline">Status</s-table-header>
        <s-table-header listSlot="labeled" format="numeric">
          Orders
        </s-table-header>
        <s-table-header listSlot="labeled" format="currency">
          Total spent
        </s-table-header>
      </s-table-header-row>
      <s-table-body>
        <s-table-row>
          <s-table-cell>#1001</s-table-cell>
          <s-table-cell>Sarah Johnson</s-table-cell>
          <s-table-cell>sarah@example.com</s-table-cell>
          <s-table-cell>
            <s-badge tone="success">Active</s-badge>
          </s-table-cell>
          <s-table-cell>23</s-table-cell>
          <s-table-cell>$1,245.50</s-table-cell>
        </s-table-row>
        <s-table-row>
          <s-table-cell>#1002</s-table-cell>
          <s-table-cell>Mike Chen</s-table-cell>
          <s-table-cell>mike@example.com</s-table-cell>
          <s-table-cell>
            <s-badge tone="neutral">Inactive</s-badge>
          </s-table-cell>
          <s-table-cell>7</s-table-cell>
          <s-table-cell>$432.75</s-table-cell>
        </s-table-row>
        <s-table-row>
          <s-table-cell>#1003</s-table-cell>
          <s-table-cell>Emma Davis</s-table-cell>
          <s-table-cell>emma@example.com</s-table-cell>
          <s-table-cell>
            <s-badge tone="success">Active</s-badge>
          </s-table-cell>
          <s-table-cell>15</s-table-cell>
          <s-table-cell>$892.25</s-table-cell>
        </s-table-row>
      </s-table-body>
    </s-table>
  </s-section>
  ```

## Best practices

- Use when displaying data with 3 or more attributes per item
- All items should share the same structure and attributes
- Don't use when data varies significantly between items (use a list instead)
- Tables automatically transform into lists on mobile devices

## Related

[Composition - Index table](https://shopify.dev/docs/api/app-home/patterns/compositions/index-table)

</page>

<page>
---
title: UnorderedList
description: >-
  Displays a bulleted list of related items. Use to present collections of items
  or options where the sequence isn’t critical.
api_name: app-home
source_url:
  html: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/layout-and-structure/unorderedlist
  md: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/layout-and-structure/unorderedlist.md
---

# Unordered​List

Displays a bulleted list of related items. Use to present collections of items or options where the sequence isn’t critical.

## Slots

- **children**

  **HTMLElement**

  The items of the UnorderedList.

  Only ListItems are accepted.

## ListItem

Represents a single item within an unordered or ordered list. Use only as a child of `s-unordered-list` or `s-ordered-list` components.

## Slots

- **children**

  **HTMLElement**

  The content of the ListItem.

Examples

### Examples

- #### Code

  ##### jsx

  ```jsx
  <s-unordered-list>
    <s-list-item>Red shirt</s-list-item>
    <s-list-item>Green shirt</s-list-item>
    <s-list-item>Blue shirt</s-list-item>
  </s-unordered-list>
  ```

  ##### html

  ```html
  <s-unordered-list>
    <s-list-item>Red shirt</s-list-item>
    <s-list-item>Green shirt</s-list-item>
    <s-list-item>Blue shirt</s-list-item>
  </s-unordered-list>
  ```

- #### Unordered list with nested items

  ##### Description

  A standard unordered list with nested items demonstrating hierarchical content structure.

  ##### jsx

  ```jsx
  <s-stack gap="base">
    <s-box borderWidth="small-100" borderRadius="base" padding="base">
      <s-unordered-list>
        <s-list-item>Configure payment settings</s-list-item>
        <s-list-item>
          Set up shipping options
          <s-unordered-list>
            <s-list-item>Domestic shipping rates</s-list-item>
            <s-list-item>International shipping zones</s-list-item>
          </s-unordered-list>
        </s-list-item>
        <s-list-item>Add product descriptions</s-list-item>
      </s-unordered-list>
    </s-box>

    <s-box borderWidth="small-100" borderRadius="base" padding="base">
      <s-unordered-list>
        <s-list-item>Enable online payments</s-list-item>
        <s-list-item>Set up shipping rates</s-list-item>
        <s-list-item>Configure tax settings</s-list-item>
        <s-list-item>Add product descriptions</s-list-item>
      </s-unordered-list>
    </s-box>
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack gap="base">
    <s-box borderWidth="small-100" borderRadius="base" padding="base">
      <s-unordered-list>
        <s-list-item>Configure payment settings</s-list-item>
        <s-list-item>
          Set up shipping options
          <s-unordered-list>
            <s-list-item>Domestic shipping rates</s-list-item>
            <s-list-item>International shipping zones</s-list-item>
          </s-unordered-list>
        </s-list-item>
        <s-list-item>Add product descriptions</s-list-item>
      </s-unordered-list>
    </s-box>

    <s-box borderWidth="small-100" borderRadius="base" padding="base">
      <s-unordered-list>
        <s-list-item>Enable online payments</s-list-item>
        <s-list-item>Set up shipping rates</s-list-item>
        <s-list-item>Configure tax settings</s-list-item>
        <s-list-item>Add product descriptions</s-list-item>
      </s-unordered-list>
    </s-box>
  </s-stack>
  ```

## Best practices

- Use to break up related content and improve scannability
- Phrase items consistently (start each with a noun or verb)
- Start each item with a capital letter
- Don't use commas or semicolons at the end of lines

</page>

<page>
---
title: Avatar
description: 'Show a user’s profile image or initials in a compact, visual element.'
api_name: app-home
source_url:
  html: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/media-and-visuals/avatar
  md: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/media-and-visuals/avatar.md
---

# Avatar

Show a user’s profile image or initials in a compact, visual element.

## Properties

- **alt**

  **string**

  An alternative text that describes the avatar for the reader to understand what it is about or identify the user the avatar belongs to.

- **initials**

  **string**

  Initials to display in the avatar.

- **size**

  **"small" | "small-200" | "base" | "large" | "large-200"**

  **Default: 'base'**

  Size of the avatar.

- **src**

  **string**

  The URL or path to the image.

  Initials will be rendered as a fallback if `src` is not provided, fails to load or does not load quickly

## Events

Learn more about [registering events](https://shopify.dev/docs/api/app-home/using-polaris-components#event-handling).

- **error**

  **OnErrorEventHandler**

- **load**

  **CallbackEventListener\<typeof tagName> | null**

### CallbackEventListener

```ts
(EventListener & {
      (event: CallbackEvent<T>): void;
    }) | null
```

### CallbackEvent

```ts
Event & {
  currentTarget: HTMLElementTagNameMap[T];
}
```

Examples

### Examples

- #### Code

  ##### jsx

  ```jsx
  <s-avatar alt="John Doe" initials="JD" />
  ```

  ##### html

  ```html
  <s-avatar alt="John Doe" initials="JD"></s-avatar>
  ```

- #### Basic usage

  ##### Description

  Displays a customer avatar with their initials when no profile image is available.

  ##### jsx

  ```jsx
  <s-avatar initials="SC" alt="Sarah Chen" />
  ```

  ##### html

  ```html
  <s-avatar initials="SC" alt="Sarah Chen"></s-avatar>
  ```

- #### Default avatar (no props)

  ##### Description

  Shows a generic person icon placeholder when no user information is available.

  ##### jsx

  ```jsx
  <s-avatar alt="Customer" />
  ```

  ##### html

  ```html
  <s-avatar alt="Customer"></s-avatar>
  ```

- #### With image source and fallback

  ##### Description

  Loads a customer profile image with automatic fallback to initials if the image fails to load.

  ##### jsx

  ```jsx
  <s-avatar
    src="/customers/profile-123.jpg"
    initials="MR"
    alt="Maria Rodriguez"
    size="base"
   />
  ```

  ##### html

  ```html
  <s-avatar
    src="/customers/profile-123.jpg"
    initials="MR"
    alt="Maria Rodriguez"
    size="base"
  ></s-avatar>
  ```

- #### Size variations

  ##### Description

  Displays customer and merchant avatars in different sizes for various interface contexts.

  ##### jsx

  ```jsx
  <s-stack direction="inline" gap="base">
    <s-avatar initials="SC" alt="Store customer" size="small-200" />
    <s-avatar initials="MR" alt="Merchant representative" size="small" />
    <s-avatar initials="SM" alt="Store manager" size="base" />
    <s-avatar initials="SF" alt="Staff member" size="large" />
    <s-avatar initials="SO" alt="Store owner" size="large-200" />
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack direction="inline" gap="base">
    <s-avatar initials="SC" alt="Store customer" size="small-200"></s-avatar>
    <s-avatar initials="MR" alt="Merchant representative" size="small"></s-avatar>
    <s-avatar initials="SM" alt="Store manager" size="base"></s-avatar>
    <s-avatar initials="SF" alt="Staff member" size="large"></s-avatar>
    <s-avatar initials="SO" alt="Store owner" size="large-200"></s-avatar>
  </s-stack>
  ```

- #### Long initials handling

  ##### Description

  Shows how the component handles store and business names of varying lengths in commerce contexts.

  ##### jsx

  ```jsx
  <s-stack direction="inline" gap="base">
    <s-avatar initials="TC" alt="Tech customer" size="base" />
    <s-avatar initials="VIP" alt="VIP customer store" size="base" />
    <s-avatar initials="SHOP" alt="Shopify partner store" size="base" />
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack direction="inline" gap="base">
    <s-avatar initials="TC" alt="Tech customer" size="base"></s-avatar>
    <s-avatar initials="VIP" alt="VIP customer store" size="base"></s-avatar>
    <s-avatar initials="SHOP" alt="Shopify partner store" size="base"></s-avatar>
  </s-stack>
  ```

- #### Color consistency demo

  ##### Description

  Demonstrates that identical initials always receive the same color assignment across different store types.

  ##### jsx

  ```jsx
  <s-stack direction="inline" gap="base">
    <s-avatar initials="AB" alt="Apparel boutique" size="base" />
    <s-avatar initials="CD" alt="Coffee direct" size="base" />
    <s-avatar initials="EF" alt="Electronics franchise" size="base" />
    <s-avatar initials="AB" alt="Art books store" size="base" />
    {/* Note: Both AB avatars will have the same color */}
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack direction="inline" gap="base">
    <s-avatar initials="AB" alt="Apparel boutique" size="base"></s-avatar>
    <s-avatar initials="CD" alt="Coffee direct" size="base"></s-avatar>
    <s-avatar initials="EF" alt="Electronics franchise" size="base"></s-avatar>
    <s-avatar initials="AB" alt="Art books store" size="base"></s-avatar>
    <!-- Note: Both AB avatars will have the same color -->
  </s-stack>
  ```

- #### Error handling example

  ##### Description

  Shows automatic fallback to initials when customer or merchant profile images fail to load.

  ##### jsx

  ```jsx
  <s-avatar
    src="/invalid-customer-photo.jpg"
    initials="CS"
    alt="Customer support"
   />
  ```

  ##### html

  ```html
  <s-avatar
    src="/invalid-customer-photo.jpg"
    initials="CS"
    alt="Customer support"
  ></s-avatar>
  <!-- Will display "CS" initials when image fails -->
  ```

- #### In customer list context

  ##### Description

  Typical usage pattern for displaying customer avatars in order lists or customer listings.

  ##### jsx

  ```jsx
  <s-stack gap="base">
    <s-stack direction="inline" gap="small">
      <s-avatar
        src="/customers/merchant-alice.jpg"
        initials="AJ"
        alt="Alice's jewelry store"
        size="small"
       />
      <s-text>Alice's jewelry store</s-text>
    </s-stack>
    <s-stack direction="inline" gap="small">
      <s-avatar initials="BP" alt="Bob's pet supplies" size="small" />
      <s-text>Bob's pet supplies</s-text>
    </s-stack>
    <s-stack direction="inline" gap="small">
      <s-avatar
        src="/customers/charlie-cafe.jpg"
        initials="CC"
        alt="Charlie's coffee corner"
        size="small"
       />
      <s-text>Charlie's coffee corner</s-text>
    </s-stack>
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack gap="base">
    <s-stack direction="inline" gap="small">
      <s-avatar
        src="/customers/merchant-alice.jpg"
        initials="AJ"
        alt="Alice's jewelry store"
        size="small"
      ></s-avatar>
      <s-text>Alice's jewelry store</s-text>
    </s-stack>
    <s-stack direction="inline" gap="small">
      <s-avatar initials="BP" alt="Bob's pet supplies" size="small"></s-avatar>
      <s-text>Bob's pet supplies</s-text>
    </s-stack>
    <s-stack direction="inline" gap="small">
      <s-avatar
        src="/customers/charlie-cafe.jpg"
        initials="CC"
        alt="Charlie's coffee corner"
        size="small"
      ></s-avatar>
      <s-text>Charlie's coffee corner</s-text>
    </s-stack>
  </s-stack>
  ```

- #### Staff member profiles

  ##### Description

  Shows staff member avatars in different admin interface contexts.

  ##### jsx

  ```jsx
  <s-stack direction="inline" gap="large">
    <s-avatar
      src="/staff/manager-profile.jpg"
      initials="SM"
      alt="Store manager"
      size="large"
     />
    <s-avatar initials="CS" alt="Customer service rep" size="base" />
    <s-avatar initials="FT" alt="Fulfillment team lead" size="small" />
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack direction="inline" gap="large">
    <s-avatar
      src="/staff/manager-profile.jpg"
      initials="SM"
      alt="Store manager"
      size="large"
    ></s-avatar>
    <s-avatar initials="CS" alt="Customer service rep" size="base"></s-avatar>
    <s-avatar initials="FT" alt="Fulfillment team lead" size="small"></s-avatar>
  </s-stack>
  ```

- #### With Section component

  ##### Description

  Demonstrates avatar integration with other admin-ui components in a merchant section layout.

  ##### jsx

  ```jsx
  <s-section>
    <s-stack gap="base">
      <s-stack direction="inline" gap="small">
        <s-avatar
          src="/merchants/premium-store.jpg"
          initials="PS"
          alt="Premium store"
          size="base"
         />
        <s-stack gap="small-400">
          <s-heading>Premium store</s-heading>
          <s-text color="subdued">Shopify Plus merchant</s-text>
        </s-stack>
      </s-stack>
      <s-text>Monthly revenue: $45,000</s-text>
    </s-stack>
  </s-section>
  ```

  ##### html

  ```html
  <s-section>
    <s-stack gap="base">
      <s-stack direction="inline" gap="small">
        <s-avatar
          src="/merchants/premium-store.jpg"
          initials="PS"
          alt="Premium store"
          size="base"
        ></s-avatar>
        <s-stack gap="small-400">
          <s-heading>Premium store</s-heading>
          <s-text color="subdued">Shopify Plus merchant</s-text>
        </s-stack>
      </s-stack>
      <s-text>Monthly revenue: $45,000</s-text>
    </s-stack>
  </s-section>
  ```

- #### Fulfillment partner avatars

  ##### Description

  Shows avatars for different types of fulfillment partners in the Shopify ecosystem.

  ##### jsx

  ```jsx
  <s-stack gap="small">
    <s-stack direction="inline" gap="small">
      <s-avatar initials="3P" alt="3PL partner" size="small" />
      <s-text>Third-party logistics</s-text>
    </s-stack>
    <s-stack direction="inline" gap="small">
      <s-avatar initials="DS" alt="Dropship supplier" size="small" />
      <s-text>Dropship supplier</s-text>
    </s-stack>
    <s-stack direction="inline" gap="small">
      <s-avatar initials="WH" alt="Warehouse hub" size="small" />
      <s-text>Warehouse hub</s-text>
    </s-stack>
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack gap="small">
    <s-stack direction="inline" gap="small">
      <s-avatar initials="3P" alt="3PL partner" size="small"></s-avatar>
      <s-text>Third-party logistics</s-text>
    </s-stack>
    <s-stack direction="inline" gap="small">
      <s-avatar initials="DS" alt="Dropship supplier" size="small"></s-avatar>
      <s-text>Dropship supplier</s-text>
    </s-stack>
    <s-stack direction="inline" gap="small">
      <s-avatar initials="WH" alt="Warehouse hub" size="small"></s-avatar>
      <s-text>Warehouse hub</s-text>
    </s-stack>
  </s-stack>
  ```

## Useful for

- Identifying individuals or businesses
- Representing merchants, customers, or other entities visually
- Seeing visual indicators of people or businesses in lists, tables, or cards

## Best practices

- `small-200`: use in tightly condensed layouts
- `small`: use when the base size is too big for the layout, or when the avatar has less importance
- `base`: use as the default size
- `large`: use when an avatar is a focal point, such as on a single customer card
- `large-200`: use when extra emphasis is required

## Content guidelines

For avatars, we recommend using a format that describes what will show in the image:

- alt="Person's name" if avatar represents a person
- alt="Business's name" if avatar represents a business
- alt="" if the name appears next to the avatar as text

</page>

<page>
---
title: Icon
description: >-
  Renders a graphic symbol to visually communicate core parts of the product and
  available actions. Icons can act as wayfinding tools to help users quickly
  understand their location within the interface and common interaction
  patterns.
api_name: app-home
source_url:
  html: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/media-and-visuals/icon
  md: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/media-and-visuals/icon.md
---

# Icon

Renders a graphic symbol to visually communicate core parts of the product and available actions. Icons can act as wayfinding tools to help users quickly understand their location within the interface and common interaction patterns.

## Properties

- **color**

  **"base" | "subdued"**

  **Default: 'base'**

  Modify the color to be more or less intense.

- **interestFor**

  **string**

  ID of a component that should respond to interest (e.g. hover and focus) on this component.

- **size**

  **"small" | "base"**

  **Default: 'base'**

  Adjusts the size of the icon.

- **tone**

  **"info" | "success" | "warning" | "critical" | "auto" | "neutral" | "caution"**

  **Default: 'auto'**

  Sets the tone of the icon, based on the intention of the information being conveyed.

- **type**

  **"" | "replace" | "search" | "split" | "link" | "edit" | "product" | "variant" | "collection" | "select" | "info" | "incomplete" | "complete" | "color" | "money" | "order" | "code" | "adjust" | "affiliate" | "airplane" | "alert-bubble" | "alert-circle" | "alert-diamond" | "alert-location" | "alert-octagon" | "alert-octagon-filled" | "alert-triangle" | "alert-triangle-filled" | "align-horizontal-centers" | "app-extension" | "apps" | "archive" | "arrow-down" | "arrow-down-circle" | "arrow-down-right" | "arrow-left" | "arrow-left-circle" | "arrow-right" | "arrow-right-circle" | "arrow-up" | "arrow-up-circle" | "arrow-up-right" | "arrows-in-horizontal" | "arrows-out-horizontal" | "asterisk" | "attachment" | "automation" | "backspace" | "bag" | "bank" | "barcode" | "battery-low" | "bill" | "blank" | "blog" | "bolt" | "bolt-filled" | "book" | "book-open" | "bug" | "bullet" | "business-entity" | "button" | "button-press" | "calculator" | "calendar" | "calendar-check" | "calendar-compare" | "calendar-list" | "calendar-time" | "camera" | "camera-flip" | "caret-down" | "caret-left" | "caret-right" | "caret-up" | "cart" | "cart-abandoned" | "cart-discount" | "cart-down" | "cart-filled" | "cart-sale" | "cart-send" | "cart-up" | "cash-dollar" | "cash-euro" | "cash-pound" | "cash-rupee" | "cash-yen" | "catalog-product" | "categories" | "channels" | "channels-filled" | "chart-cohort" | "chart-donut" | "chart-funnel" | "chart-histogram-first" | "chart-histogram-first-last" | "chart-histogram-flat" | "chart-histogram-full" | "chart-histogram-growth" | "chart-histogram-last" | "chart-histogram-second-last" | "chart-horizontal" | "chart-line" | "chart-popular" | "chart-stacked" | "chart-vertical" | "chat" | "chat-new" | "chat-referral" | "check" | "check-circle" | "check-circle-filled" | "checkbox" | "chevron-down" | "chevron-down-circle" | "chevron-left" | "chevron-left-circle" | "chevron-right" | "chevron-right-circle" | "chevron-up" | "chevron-up-circle" | "circle" | "circle-dashed" | "clipboard" | "clipboard-check" | "clipboard-checklist" | "clock" | "clock-list" | "clock-revert" | "code-add" | "collection-featured" | "collection-list" | "collection-reference" | "color-none" | "compass" | "compose" | "confetti" | "connect" | "content" | "contract" | "corner-pill" | "corner-round" | "corner-square" | "credit-card" | "credit-card-cancel" | "credit-card-percent" | "credit-card-reader" | "credit-card-reader-chip" | "credit-card-reader-tap" | "credit-card-secure" | "credit-card-tap-chip" | "crop" | "currency-convert" | "cursor" | "cursor-banner" | "cursor-option" | "data-presentation" | "data-table" | "database" | "database-add" | "database-connect" | "delete" | "delivered" | "delivery" | "desktop" | "disabled" | "disabled-filled" | "discount" | "discount-add" | "discount-automatic" | "discount-code" | "discount-remove" | "dns-settings" | "dock-floating" | "dock-side" | "domain" | "domain-landing-page" | "domain-new" | "domain-redirect" | "download" | "drag-drop" | "drag-handle" | "drawer" | "duplicate" | "email" | "email-follow-up" | "email-newsletter" | "empty" | "enabled" | "enter" | "envelope" | "envelope-soft-pack" | "eraser" | "exchange" | "exit" | "export" | "external" | "eye-check-mark" | "eye-dropper" | "eye-dropper-list" | "eye-first" | "eyeglasses" | "fav" | "favicon" | "file" | "file-list" | "filter" | "filter-active" | "flag" | "flip-horizontal" | "flip-vertical" | "flower" | "folder" | "folder-add" | "folder-down" | "folder-remove" | "folder-up" | "food" | "foreground" | "forklift" | "forms" | "games" | "gauge" | "geolocation" | "gift" | "gift-card" | "git-branch" | "git-commit" | "git-repository" | "globe" | "globe-asia" | "globe-europe" | "globe-lines" | "globe-list" | "graduation-hat" | "grid" | "hashtag" | "hashtag-decimal" | "hashtag-list" | "heart" | "hide" | "hide-filled" | "home" | "home-filled" | "icons" | "identity-card" | "image" | "image-add" | "image-alt" | "image-explore" | "image-magic" | "image-none" | "image-with-text-overlay" | "images" | "import" | "in-progress" | "incentive" | "incoming" | "info-filled" | "inheritance" | "inventory" | "inventory-edit" | "inventory-list" | "inventory-transfer" | "inventory-updated" | "iq" | "key" | "keyboard" | "keyboard-filled" | "keyboard-hide" | "keypad" | "label-printer" | "language" | "language-translate" | "layout-block" | "layout-buy-button" | "layout-buy-button-horizontal" | "layout-buy-button-vertical" | "layout-column-1" | "layout-columns-2" | "layout-columns-3" | "layout-footer" | "layout-header" | "layout-logo-block" | "layout-popup" | "layout-rows-2" | "layout-section" | "layout-sidebar-left" | "layout-sidebar-right" | "lightbulb" | "link-list" | "list-bulleted" | "list-bulleted-filled" | "list-numbered" | "live" | "live-critical" | "live-none" | "location" | "location-none" | "lock" | "map" | "markets" | "markets-euro" | "markets-rupee" | "markets-yen" | "maximize" | "measurement-size" | "measurement-size-list" | "measurement-volume" | "measurement-volume-list" | "measurement-weight" | "measurement-weight-list" | "media-receiver" | "megaphone" | "mention" | "menu" | "menu-filled" | "menu-horizontal" | "menu-vertical" | "merge" | "metafields" | "metaobject" | "metaobject-list" | "metaobject-reference" | "microphone" | "microphone-muted" | "minimize" | "minus" | "minus-circle" | "mobile" | "money-none" | "money-split" | "moon" | "nature" | "note" | "note-add" | "notification" | "number-one" | "order-batches" | "order-draft" | "order-filled" | "order-first" | "order-fulfilled" | "order-repeat" | "order-unfulfilled" | "orders-status" | "organization" | "outdent" | "outgoing" | "package" | "package-cancel" | "package-fulfilled" | "package-on-hold" | "package-reassign" | "package-returned" | "page" | "page-add" | "page-attachment" | "page-clock" | "page-down" | "page-heart" | "page-list" | "page-reference" | "page-remove" | "page-report" | "page-up" | "pagination-end" | "pagination-start" | "paint-brush-flat" | "paint-brush-round" | "paper-check" | "partially-complete" | "passkey" | "paste" | "pause-circle" | "payment" | "payment-capture" | "payout" | "payout-dollar" | "payout-euro" | "payout-pound" | "payout-rupee" | "payout-yen" | "person" | "person-add" | "person-exit" | "person-filled" | "person-list" | "person-lock" | "person-remove" | "person-segment" | "personalized-text" | "phablet" | "phone" | "phone-down" | "phone-down-filled" | "phone-in" | "phone-out" | "pin" | "pin-remove" | "plan" | "play" | "play-circle" | "plus" | "plus-circle" | "plus-circle-down" | "plus-circle-filled" | "plus-circle-up" | "point-of-sale" | "point-of-sale-register" | "price-list" | "print" | "product-add" | "product-cost" | "product-filled" | "product-list" | "product-reference" | "product-remove" | "product-return" | "product-unavailable" | "profile" | "profile-filled" | "question-circle" | "question-circle-filled" | "radio-control" | "receipt" | "receipt-dollar" | "receipt-euro" | "receipt-folded" | "receipt-paid" | "receipt-pound" | "receipt-refund" | "receipt-rupee" | "receipt-yen" | "receivables" | "redo" | "referral-code" | "refresh" | "remove-background" | "reorder" | "replay" | "reset" | "return" | "reward" | "rocket" | "rotate-left" | "rotate-right" | "sandbox" | "save" | "savings" | "scan-qr-code" | "search-add" | "search-list" | "search-recent" | "search-resource" | "send" | "settings" | "share" | "shield-check-mark" | "shield-none" | "shield-pending" | "shield-person" | "shipping-label" | "shipping-label-cancel" | "shopcodes" | "slideshow" | "smiley-happy" | "smiley-joy" | "smiley-neutral" | "smiley-sad" | "social-ad" | "social-post" | "sort" | "sort-ascending" | "sort-descending" | "sound" | "sports" | "star" | "star-circle" | "star-filled" | "star-half" | "star-list" | "status" | "status-active" | "stop-circle" | "store" | "store-import" | "store-managed" | "store-online" | "sun" | "table" | "table-masonry" | "tablet" | "target" | "tax" | "team" | "text" | "text-align-center" | "text-align-left" | "text-align-right" | "text-block" | "text-bold" | "text-color" | "text-font" | "text-font-list" | "text-grammar" | "text-in-columns" | "text-in-rows" | "text-indent" | "text-indent-remove" | "text-italic" | "text-quote" | "text-title" | "text-underline" | "text-with-image" | "theme" | "theme-edit" | "theme-store" | "theme-template" | "three-d-environment" | "thumbs-down" | "thumbs-up" | "tip-jar" | "toggle-off" | "toggle-on" | "transaction" | "transaction-fee-add" | "transaction-fee-dollar" | "transaction-fee-euro" | "transaction-fee-pound" | "transaction-fee-rupee" | "transaction-fee-yen" | "transfer" | "transfer-in" | "transfer-internal" | "transfer-out" | "truck" | "undo" | "unknown-device" | "unlock" | "upload" | "variant-list" | "video" | "video-list" | "view" | "viewport-narrow" | "viewport-short" | "viewport-tall" | "viewport-wide" | "wallet" | "wand" | "watch" | "wifi" | "work" | "work-list" | "wrench" | "x" | "x-circle" | "x-circle-filled"**

  Specifies the type of icon that will be displayed.

Examples

### Examples

- #### Code

  ##### jsx

  ```jsx
  <s-stack direction="inline" gap="base">
    <s-icon type="home" />
    <s-icon type="order" />
    <s-icon type="product" />
    <s-icon type="settings" />
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack direction="inline" gap="base">
    <s-icon type="home"></s-icon>
    <s-icon type="order"></s-icon>
    <s-icon type="product"></s-icon>
    <s-icon type="settings"></s-icon>
  </s-stack>
  ```

- #### Basic usage

  ##### Description

  Standard icons for common merchant interface actions and navigation. Demonstrates rendering multiple icons in an inline stack, showing different types of icons used for navigation and actions.

  ##### jsx

  ```jsx
  <s-stack direction="inline" gap="base">
    <s-icon type="home" />
    <s-icon type="edit" />
    <s-icon type="duplicate" />
    <s-icon type="save" />
    <s-icon type="export" />
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack direction="inline" gap="base">
    <s-icon type="home"></s-icon>
    <s-icon type="edit"></s-icon>
    <s-icon type="duplicate"></s-icon>
    <s-icon type="save"></s-icon>
    <s-icon type="export"></s-icon>
  </s-stack>
  ```

- #### With semantic tone

  ##### Description

  Icons with color-coded tones to convey status and semantic meaning.

  ##### jsx

  ```jsx
  <s-stack direction="inline" gap="base">
    <s-icon type="alert-circle" tone="warning" />
    <s-icon type="check-circle" tone="success" />
    <s-icon type="info" tone="info" />
    <s-icon type="alert-triangle" tone="caution" />
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack direction="inline" gap="base">
    <s-icon type="alert-circle" tone="warning"></s-icon>
    <s-icon type="check-circle" tone="success"></s-icon>
    <s-icon type="info" tone="info"></s-icon>
    <s-icon type="alert-triangle" tone="caution"></s-icon>
  </s-stack>
  ```

- #### Small size

  ##### Description

  Compact icon sizing for space-constrained interfaces and inline usage. Shows how to render a small-sized icon that takes up minimal space while maintaining clarity.

  ##### jsx

  ```jsx
  <s-icon type="search" size="small" />
  ```

  ##### html

  ```html
  <s-icon type="search" size="small"></s-icon>
  ```

- #### Subdued color

  ##### Description

  Lower contrast icon for secondary actions and supporting information.

  ##### jsx

  ```jsx
  <s-icon type="question-circle" color="subdued" />
  ```

  ##### html

  ```html
  <s-icon type="question-circle" color="subdued"></s-icon>
  ```

- #### With id property

  ##### Description

  Icon with unique identifier for JavaScript targeting and styling. Demonstrates adding a specific ID to an icon, which can be used for JavaScript interactions, CSS styling, or accessibility purposes.

  ##### jsx

  ```jsx
  <s-icon type="settings" id="settings-icon" />
  ```

  ##### html

  ```html
  <s-icon type="settings" id="settings-icon"></s-icon>
  ```

- #### With interest relationship

  ##### Description

  Icon associated with interactive elements for enhanced accessibility context.

  ##### jsx

  ```jsx
  <>
    <s-tooltip id="info-tooltip">
      SKU must be unique across all products and cannot be changed after creation
    </s-tooltip>
    <s-icon type="info" tone="info" interestFor="info-tooltip" />
  </>
  ```

  ##### html

  ```html
  <s-tooltip id="info-tooltip">
    SKU must be unique across all products and cannot be changed after creation
  </s-tooltip>
  <s-icon type="info" tone="info" interestFor="info-tooltip" />
  ```

- #### In button components

  ##### Description

  Icons integrated within button components for clear action identification. Shows how icons can be added to buttons to visually reinforce the button's action, using both positive (add) and negative (delete) tones.

  ##### jsx

  ```jsx
  <s-button-group>
    <s-button slot="secondary-actions" icon="plus">
      Add product
    </s-button>
    <s-button slot="secondary-actions" icon="delete" tone="critical">
      Delete
    </s-button>
  </s-button-group>
  ```

  ##### html

  ```html
  <s-button-group>
    <s-button slot="secondary-actions" icon="plus">Add product</s-button>
    <s-button slot="secondary-actions" icon="delete" tone="critical">
      Delete
    </s-button>
  </s-button-group>
  ```

- #### In badge components

  ##### Description

  Icons combined with badges to enhance status communication and visual hierarchy. Demonstrates using icons with badges to provide visual status indicators, using success and warning tones to convey different states.

  ##### jsx

  ```jsx
  <s-stack direction="inline" gap="base">
    <s-badge tone="success" icon="check-circle">
      Active
    </s-badge>
    <s-badge tone="warning" icon="alert-triangle">
      Pending
    </s-badge>
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack direction="inline" gap="base">
    <s-badge tone="success" icon="check-circle">Active</s-badge>
    <s-badge tone="warning" icon="alert-triangle">Pending</s-badge>
  </s-stack>
  ```

## Available icons

Search and filter across all the available icons:

## Useful for

- Orienting themselves and understanding available actions
- Quickly identifying information and recognizing patterns

## Best practices

Icons should:

- Use the same icon consistently for the same meaning
- Appear next to related text labels
- Only be used when their meaning is clear

</page>

<page>
---
title: Image
description: >-
  Embeds an image within the interface and controls its presentation. Use to
  visually illustrate concepts, showcase products, or support user tasks and
  interactions.
api_name: app-home
source_url:
  html: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/media-and-visuals/image
  md: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/media-and-visuals/image.md
---

# Image

Embeds an image within the interface and controls its presentation. Use to visually illustrate concepts, showcase products, or support user tasks and interactions.

## Properties

- **accessibilityRole**

  **"none" | "presentation" | "img"**

  **Default: 'img'**

  Sets the semantic meaning of the component’s content. When set, the role will be used by assistive technologies to help users navigate the page.

- **alt**

  **string**

  **Default: \`''\`**

  An alternative text description that describe the image for the reader to understand what it is about. It is extremely useful for both users using assistive technology and sighted users. A well written description provides people with visual impairments the ability to participate in consuming non-text content. When a screen readers encounters an `s-image`, the description is read and announced aloud. If an image fails to load, potentially due to a poor connection, the `alt` is displayed on screen instead. This has the benefit of letting a sighted buyer know an image was meant to load here, but as an alternative, they’re still able to consume the text content. Read [considerations when writing alternative text](https://www.shopify.com/ca/blog/image-alt-text#4) to learn more.

- **aspectRatio**

  **\`${number}\` | \`${number}/${number}\` | \`${number}/ ${number}\` | \`${number} /${number}\` | \`${number} / ${number}\`**

  **Default: '1/1'**

  The aspect ratio of the image.

  The rendering of the image will depend on the `inlineSize` value:

  - `inlineSize="fill"`: the aspect ratio will be respected and the image will take the necessary space.
  - `inlineSize="auto"`: the image will not render until it has loaded and the aspect ratio will be ignored.

  For example, if the value is set as `50 / 100`, the getter returns `50 / 100`. If the value is set as `0.5`, the getter returns `0.5 / 1`.

- **border**

  **BorderShorthand**

  **Default: 'none' - equivalent to \`none base auto\`.**

  Set the border via the shorthand property.

  This can be a size, optionally followed by a color, optionally followed by a style.

  If the color is not specified, it will be `base`.

  If the style is not specified, it will be `auto`.

  Values can be overridden by `borderWidth`, `borderStyle`, and `borderColor`.

- **borderColor**

  **"" | ColorKeyword**

  **Default: '' - meaning no override**

  Adjust the color of the border.

- **borderRadius**

  **MaybeAllValuesShorthandProperty\<BoxBorderRadii>**

  **Default: 'none'**

  Adjust the radius of the border.

- **borderStyle**

  **"" | MaybeAllValuesShorthandProperty\<BoxBorderStyles>**

  **Default: '' - meaning no override**

  Adjust the style of the border.

- **borderWidth**

  **"" | MaybeAllValuesShorthandProperty<"small" | "small-100" | "base" | "large" | "large-100" | "none">**

  **Default: '' - meaning no override**

  Adjust the width of the border.

- **inlineSize**

  **"auto" | "fill"**

  **Default: 'fill'**

  The displayed inline width of the image.

  - `fill`: the image will takes up 100% of the available inline size.
  - `auto`: the image will be displayed at its natural size.

- **loading**

  **"eager" | "lazy"**

  **Default: 'eager'**

  Determines the loading behavior of the image:

  - `eager`: Immediately loads the image, irrespective of its position within the visible viewport.
  - `lazy`: Delays loading the image until it approaches a specified distance from the viewport.

- **objectFit**

  **"contain" | "cover"**

  **Default: 'contain'**

  Determines how the content of the image is resized to fit its container. The image is positioned in the center of the container.

- **sizes**

  **string**

  A set of media conditions and their corresponding sizes.

- **src**

  **string**

  The image source (either a remote URL or a local file resource).

  When the image is loading or no `src` is provided, a placeholder will be rendered.

- **srcSet**

  **string**

  A set of image sources and their width or pixel density descriptors.

  This overrides the `src` property.

### BorderShorthand

Represents a shorthand for defining a border. It can be a combination of size, optionally followed by color, optionally followed by style.

```ts
BorderSizeKeyword | `${BorderSizeKeyword} ${ColorKeyword}` | `${BorderSizeKeyword} ${ColorKeyword} ${BorderStyleKeyword}`
```

### BorderSizeKeyword

```ts
SizeKeyword | 'none'
```

### SizeKeyword

```ts
'small-500' | 'small-400' | 'small-300' | 'small-200' | 'small-100' | 'small' | 'base' | 'large' | 'large-100' | 'large-200' | 'large-300' | 'large-400' | 'large-500'
```

### ColorKeyword

```ts
'subdued' | 'base' | 'strong'
```

### BorderStyleKeyword

```ts
'none' | 'solid' | 'dashed' | 'dotted' | 'auto'
```

### MaybeAllValuesShorthandProperty

```ts
T | `${T} ${T}` | `${T} ${T} ${T}` | `${T} ${T} ${T} ${T}`
```

### BoxBorderRadii

```ts
'small' | 'small-200' | 'small-100' | 'base' | 'large' | 'large-100' | 'large-200' | 'none'
```

### BoxBorderStyles

```ts
'auto' | 'none' | 'solid' | 'dashed'
```

## Events

Learn more about [registering events](https://shopify.dev/docs/api/app-home/using-polaris-components#event-handling).

- **error**

  **OnErrorEventHandler**

- **load**

  **CallbackEventListener\<typeof tagName> | null**

### CallbackEventListener

```ts
(EventListener & {
      (event: CallbackEvent<T>): void;
    }) | null
```

### CallbackEvent

```ts
Event & {
  currentTarget: HTMLElementTagNameMap[T];
}
```

Examples

### Examples

- #### Code

  ##### jsx

  ```jsx
  <s-image
    src="https://cdn.shopify.com/static/images/polaris/image-wc_src.png"
    alt="Four pixelated characters ready to build amazing Shopify apps"
    aspectRatio="59/161"
    inlineSize="auto"
   />
  ```

  ##### html

  ```html
  <s-image
    src="https://cdn.shopify.com/static/images/polaris/image-wc_src.png"
    alt="Four pixelated characters ready to build amazing Shopify apps"
    aspectRatio="59/161"
    inlineSize="auto"
  ></s-image>
  ```

- #### Basic usage

  ##### Description

  Demonstrates the simplest implementation of an image component with a source and alt text.

  ##### jsx

  ```jsx
  <s-image src="https://cdn.shopify.com/static/sample-product/House-Plant1.png" alt="Product image" />
  ```

  ##### html

  ```html
  <s-image
    src="https://cdn.shopify.com/static/sample-product/House-Plant1.png"
    alt="Product image"
  ></s-image>
  ```

- #### Responsive layout with aspect ratio

  ##### Description

  Shows how to create a responsive image with a fixed 16:9 aspect ratio, set to cover the container, and loaded lazily.

  ##### jsx

  ```jsx
  <s-image
    src="https://cdn.shopify.com/static/sample-product/House-Plant1.png"
    alt="Featured product"
    aspectRatio="16/9"
    objectFit="cover"
    loading="lazy"
   />
  ```

  ##### html

  ```html
  <s-image
    src="https://cdn.shopify.com/static/sample-product/House-Plant1.png"
    alt="Featured product"
    aspectRatio="16/9"
    objectFit="cover"
    loading="lazy"
  ></s-image>
  ```

- #### Responsive images with srcset

  ##### Description

  Illustrates how to provide multiple image sources for different screen sizes and resolutions using srcSet and sizes attributes.

  ##### jsx

  ```jsx
  <s-image
    src="https://cdn.shopify.com/static/sample-product/House-Plant1.png"
    srcSet="https://cdn.shopify.com/static/sample-product/House-Plant1.png 400w,
            https://cdn.shopify.com/static/sample-product/House-Plant1.png 800w"
    sizes="(max-width: 600px) 100vw, (max-width: 1200px) 50vw, 400px"
    alt="Product detail"
    aspectRatio="16/9"
    objectFit="cover"
   />
  ```

  ##### html

  ```html
  <s-image
    src="https://cdn.shopify.com/static/sample-product/House-Plant1.png"
    srcSet="https://cdn.shopify.com/static/sample-product/House-Plant1.png 400w,
            https://cdn.shopify.com/static/sample-product/House-Plant1.png 800w"
    sizes="(max-width: 600px) 100vw, (max-width: 1200px) 50vw, 400px"
    alt="Product detail"
    aspectRatio="16/9"
    objectFit="cover"
  ></s-image>
  ```

- #### With border styling

  ##### Description

  Demonstrates how to apply border styling to an image, including width, style, color, and radius, using border-related properties.

  ##### jsx

  ```jsx
  <s-box inlineSize="300px">
    <s-image
      src="https://cdn.shopify.com/static/sample-product/House-Plant1.png"
      alt="Product thumbnail"
      borderWidth="large"
      borderStyle="solid"
      borderColor="strong"
      borderRadius="large"
      objectFit="cover"
      aspectRatio="1/1"
     />
  </s-box>
  ```

  ##### html

  ```html
  <s-box inlineSize="300px">
    <s-image
      src="https://cdn.shopify.com/static/sample-product/House-Plant1.png"
      alt="Product thumbnail"
      borderWidth="large"
      borderStyle="solid"
      borderColor="strong"
      borderRadius="large"
      objectFit="cover"
      aspectRatio="1/1"
    ></s-image>
  </s-box>
  ```

- #### Decorative image

  ##### Description

  Shows how to mark an image as decorative, which will make screen readers ignore the image by setting an empty alt text and presentation role.

  ##### jsx

  ```jsx
  <s-image
    src="https://cdn.shopify.com/static/sample-product/House-Plant1.png"
    alt=""
    accessibilityRole="presentation"
    objectFit="cover"
   />
  ```

  ##### html

  ```html
  <s-image
    src="https://cdn.shopify.com/static/sample-product/House-Plant1.png"
    alt=""
    accessibilityRole="presentation"
    objectFit="cover"
  ></s-image>
  ```

- #### Auto-sized image

  ##### Description

  Demonstrates an image with auto-sizing, which allows the image to adjust its size based on its container's width.

  ##### jsx

  ```jsx
  <s-image
    src="https://cdn.shopify.com/static/sample-product/House-Plant1.png"
    alt="Product image"
    inlineSize="auto"
   />
  ```

  ##### html

  ```html
  <s-image
    src="https://cdn.shopify.com/static/sample-product/House-Plant1.png"
    alt="Product image"
    inlineSize="auto"
  ></s-image>
  ```

- #### Within layout components

  ##### Description

  Shows how to use images within a grid layout, creating a consistent grid of images with equal size, aspect ratio, and styling.

  ##### jsx

  ```jsx
  <s-grid gridTemplateColumns="repeat(3, 150px)" gap="base" alignItems="center">
    <s-image
      src="https://cdn.shopify.com/static/sample-product/House-Plant1.png"
      alt="Main view"
      aspectRatio="1/1"
      objectFit="cover"
      borderRadius="base"
      inlineSize="fill"
     />
    <s-image
      src="https://cdn.shopify.com/static/sample-product/House-Plant1.png"
      alt="Side view"
      aspectRatio="1/1"
      objectFit="cover"
      borderRadius="base"
      inlineSize="fill"
     />
    <s-image
      src="https://cdn.shopify.com/static/sample-product/House-Plant1.png"
      alt="Detail view"
      aspectRatio="1/1"
      objectFit="cover"
      borderRadius="base"
      inlineSize="fill"
     />
  </s-grid>
  ```

  ##### html

  ```html
  <s-grid gridTemplateColumns="repeat(3, 150px)" gap="base" alignItems="center">
    <s-image
      src="https://cdn.shopify.com/static/sample-product/House-Plant1.png"
      alt="Main view"
      aspectRatio="1/1"
      objectFit="cover"
      borderRadius="base"
      inlineSize="fill"
    ></s-image>
    <s-image
      src="https://cdn.shopify.com/static/sample-product/House-Plant1.png"
      alt="Side view"
      aspectRatio="1/1"
      objectFit="cover"
      borderRadius="base"
      inlineSize="fill"
    ></s-image>
    <s-image
      src="https://cdn.shopify.com/static/sample-product/House-Plant1.png"
      alt="Detail view"
      aspectRatio="1/1"
      objectFit="cover"
      borderRadius="base"
      inlineSize="fill"
    ></s-image>
  </s-grid>
  ```

## Useful for

- Adding illustrations and photos.

## Best practices

- Use high-resolution, optimized images
- Use intentionally to add clarity and guide users

## Content guidelines

Alt text should be accurate, concise, and descriptive:

- Indicate it's an image: "Image of", "Photo of"
- Focus on description: "Image of a woman with curly brown hair smiling"

</page>

<page>
---
title: Thumbnail
description: 'Display a small preview image representing content, products, or media.'
api_name: app-home
source_url:
  html: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/media-and-visuals/thumbnail
  md: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/media-and-visuals/thumbnail.md
---

# Thumbnail

Display a small preview image representing content, products, or media.

## Properties

- **alt**

  **string**

  **Default: \`''\`**

  An alternative text description that describe the image for the reader to understand what it is about. It is extremely useful for both users using assistive technology and sighted users. A well written description provides people with visual impairments the ability to participate in consuming non-text content. When a screen readers encounters an `s-image`, the description is read and announced aloud. If an image fails to load, potentially due to a poor connection, the `alt` is displayed on screen instead. This has the benefit of letting a sighted buyer know an image was meant to load here, but as an alternative, they’re still able to consume the text content. Read [considerations when writing alternative text](https://www.shopify.com/ca/blog/image-alt-text#4) to learn more.

- **size**

  **"small" | "small-200" | "small-100" | "base" | "large" | "large-100"**

  **Default: 'base'**

  Adjusts the size the product thumbnail image.

- **src**

  **string**

  The image source (either a remote URL or a local file resource).

  When the image is loading or no `src` is provided, a placeholder will be rendered.

## Events

Learn more about [registering events](https://shopify.dev/docs/api/app-home/using-polaris-components#event-handling).

- **error**

  **OnErrorEventHandler**

- **load**

  **CallbackEventListener\<typeof tagName> | null**

### CallbackEventListener

```ts
(EventListener & {
      (event: CallbackEvent<T>): void;
    }) | null
```

### CallbackEvent

```ts
Event & {
  currentTarget: HTMLElementTagNameMap[T];
}
```

Examples

### Examples

- #### Code

  ##### jsx

  ```jsx
  <s-thumbnail
    alt="White sneakers"
    src="https://cdn.shopify.com/static/images/polaris/thumbnail-wc_src.jpg"
   />
  ```

  ##### html

  ```html
  <s-thumbnail
    alt="White sneakers"
    src="https://cdn.shopify.com/static/images/polaris/thumbnail-wc_src.jpg"
  ></s-thumbnail>
  ```

- #### Basic usage

  ##### Description

  Demonstrates a basic thumbnail component with a product image, showing the default base size and an alt text for accessibility.

  ##### jsx

  ```jsx
  <s-thumbnail
    src="https://cdn.shopify.com/static/sample-product/House-Plant1.png"
    alt="Product preview"
    size="base"
   />
  ```

  ##### html

  ```html
  <s-thumbnail
    src="https://cdn.shopify.com/static/sample-product/House-Plant1.png"
    alt="Product preview"
    size="base"
  ></s-thumbnail>
  ```

- #### Empty state

  ##### Description

  Shows the thumbnail component in an empty state, displaying a placeholder icon when no image source is provided.

  ##### jsx

  ```jsx
  <s-thumbnail alt="No image available" size="base" />
  ```

  ##### html

  ```html
  <s-thumbnail alt="No image available" size="base"></s-thumbnail>
  ```

- #### Different sizes

  ##### Description

  Illustrates the various size options for the thumbnail component, showcasing small-200, base, and large sizes in a stack layout.

  ##### jsx

  ```jsx
  <s-stack gap="large-100">
    <s-thumbnail
      src="https://cdn.shopify.com/static/sample-product/House-Plant1.png"
      alt="Small thumbnail"
      size="small-200"
     />
    <s-thumbnail
      src="https://cdn.shopify.com/static/sample-product/House-Plant1.png"
      alt="Base thumbnail"
      size="base"
     />
    <s-thumbnail
      src="https://cdn.shopify.com/static/sample-product/House-Plant1.png"
      alt="Large thumbnail"
      size="large"
     />
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack gap="large-100">
    <s-thumbnail
      src="https://cdn.shopify.com/static/sample-product/House-Plant1.png"
      alt="Small thumbnail"
      size="small-200"
    ></s-thumbnail>
    <s-thumbnail
      src="https://cdn.shopify.com/static/sample-product/House-Plant1.png"
      alt="Base thumbnail"
      size="base"
    ></s-thumbnail>
    <s-thumbnail
      src="https://cdn.shopify.com/static/sample-product/House-Plant1.png"
      alt="Large thumbnail"
      size="large"
    ></s-thumbnail>
  </s-stack>
  ```

- #### With event handling

  ##### Description

  Demonstrates how event handlers like onload or onerror can be attached to the thumbnail component via JavaScript to handle image loading states.

  ##### jsx

  ```jsx
  const [loading, setLoading] = useState(true)

  return (
    <s-stack direction="inline" gap="base" alignItems="center">
      <s-thumbnail
        src="https://cdn.shopify.com/static/sample-product/House-Plant1.png"
        alt="Product"
        size="small-200"
        onLoad={() => setLoading(false)}
      />
      <s-paragraph>{loading ? 'Loading...' : 'Image loaded'}</s-paragraph>
   </s-stack>
  )
  ```

  ##### html

  ```html
    <s-stack direction="inline" gap="base">
      <s-thumbnail
        src="https://cdn.shopify.com/static/sample-product/House-Plant1.png"
        alt="Product"
        size="small-200"
        onLoad="setLoading(false)"
      />
      <s-paragraph>Image loaded</s-paragraph>
   </s-stack>
  ```

## Useful for

- Identifying items visually in lists, tables, or cards
- Seeing a preview of images before uploading or publishing
- Distinguishing between similar items by their appearance
- Confirming the correct item is selected

## Best practices

- `small-200`: use in very small areas
- `small`: use in small areas
- `base`: use as the default size
- `large`: use when thumbnail is a focal point

## Content guidelines

Alternative text should be accurate, concise, and descriptive:

- Use "Image of", "Photo of" prefix
- Be primary visual content: "Image of a woman with curly brown hair smiling"
- Include relevant emotions: "Image of a woman laughing with her hand on her face"

</page>

<page>
---
title: Modal
description: >-
  Displays content in an overlay. Use to create a distraction-free experience
  such as a confirmation dialog or a settings panel.
api_name: app-home
source_url:
  html: 'https://shopify.dev/docs/api/app-home/polaris-web-components/overlays/modal'
  md: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/overlays/modal.md
---

# Modal

Displays content in an overlay. Use to create a distraction-free experience such as a confirmation dialog or a settings panel.

## Properties

- **accessibilityLabel**

  **string**

  A label that describes the purpose of the modal. When set, it will be announced to users using assistive technologies and will provide them with more context.

  This overrides the `heading` prop for screen readers.

- **heading**

  **string**

  A title that describes the content of the Modal.

- **hideOverlay**

  **() => void**

  Method to hide an overlay.

- **padding**

  **"base" | "none"**

  **Default: 'base'**

  Adjust the padding around the Modal content.

  `base`: applies padding that is appropriate for the element.

  `none`: removes all padding from the element. This can be useful when elements inside the Modal need to span to the edge of the Modal. For example, a full-width image. In this case, rely on `Box` with a padding of 'base' to bring back the desired padding for the rest of the content.

- **showOverlay**

  **() => void**

  Method to show an overlay.

- **size**

  **"small" | "small-100" | "base" | "large" | "large-100"**

  **Default: 'base'**

  Adjust the size of the Modal.

- **toggleOverlay**

  **() => void**

  Method to toggle the visiblity of an overlay.

## Events

Learn more about [registering events](https://shopify.dev/docs/api/app-home/using-polaris-components#event-handling).

- **afterhide**

  **CallbackEventListener\<TTagName> | null**

- **aftershow**

  **CallbackEventListener\<TTagName> | null**

- **hide**

  **CallbackEventListener\<TTagName> | null**

- **show**

  **CallbackEventListener\<TTagName> | null**

### CallbackEventListener

```ts
(EventListener & {
      (event: CallbackEvent<T>): void;
    }) | null
```

### CallbackEvent

```ts
Event & {
  currentTarget: HTMLElementTagNameMap[T];
}
```

## Slots

- **children**

  **HTMLElement**

  The content of the Modal.

- **primary-action**

  **HTMLElement**

  The primary action to perform.

  Only a `Button` with a variant of `primary` is allowed.

- **secondary-actions**

  **HTMLElement**

  The secondary actions to perform.

  Only `Button` elements with a variant of `secondary` or `auto` are allowed.

Examples

### Examples

- #### Code

  ##### jsx

  ```jsx
  <>
    <s-button commandFor="modal">Open</s-button>

    <s-modal id="modal" heading="Details">
      <s-paragraph>Displaying more details here.</s-paragraph>

      <s-button slot="secondary-actions" commandFor="modal" command="--hide">
        Close
      </s-button>
      <s-button
        slot="primary-action"
        variant="primary"
        commandFor="modal"
        command="--hide"
      >
        Save
      </s-button>
    </s-modal>
  </>
  ```

  ##### html

  ```html
  <s-button commandFor="modal">Open</s-button>

  <s-modal id="modal" heading="Details">
    <s-paragraph>Displaying more details here.</s-paragraph>

    <s-button slot="secondary-actions" commandFor="modal" command="--hide">
      Close
    </s-button>
    <s-button
      slot="primary-action"
      variant="primary"
      commandFor="modal"
      command="--hide"
    >
      Save
    </s-button>
  </s-modal>
  ```

- #### Basic modal

  ##### Description

  Simple modal with heading and basic content for displaying information. Click the button to open the modal.

  ##### jsx

  ```jsx
  <>
    <s-button commandFor="info-modal" command="--show">
      Show product info
    </s-button>

    <s-modal id="info-modal" heading="Product information">
      <s-text>
        This product is currently out of stock and cannot be ordered.
      </s-text>

      <s-button slot="secondary-actions" commandFor="info-modal" command="--hide">
        Close
      </s-button>
    </s-modal>
  </>
  ```

  ##### html

  ```html
  <s-button commandFor="info-modal" command="--show">Show product info</s-button>

  <s-modal id="info-modal" heading="Product information">
    <s-text>This product is currently out of stock and cannot be ordered.</s-text>

    <s-button slot="secondary-actions" commandFor="info-modal" command="--hide">
      Close
    </s-button>
  </s-modal>
  ```

- #### Modal with actions

  ##### Description

  Modal with primary and secondary action buttons. Click the button to open the confirmation modal.

  ##### jsx

  ```jsx
  <s-stack gap="base">
    <s-button tone="critical" commandFor="delete-modal" command="--show">
      Delete product
    </s-button>

    <s-modal id="delete-modal" heading="Delete product?">
      <s-stack gap="base">
        <s-text>Are you sure you want to delete "Winter jacket"?</s-text>
        <s-text tone="caution">This action cannot be undone.</s-text>
      </s-stack>

      <s-button
        slot="primary-action"
        variant="primary"
        tone="critical"
        commandFor="delete-modal"
        command="--hide"
      >
        Delete product
      </s-button>
      <s-button
        slot="secondary-actions"
        variant="secondary"
        commandFor="delete-modal"
        command="--hide"
      >
        Cancel
      </s-button>
    </s-modal>
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack gap="base">
    <s-button tone="critical" commandFor="delete-modal" command="--show">
      Delete product
    </s-button>

    <s-modal id="delete-modal" heading="Delete product?">
      <s-stack gap="base">
        <s-text>Are you sure you want to delete "Winter jacket"?</s-text>
        <s-text tone="caution">This action cannot be undone.</s-text>
      </s-stack>

      <s-button
        slot="primary-action"
        variant="primary"
        tone="critical"
        commandFor="delete-modal"
        command="--hide"
      >
        Delete product
      </s-button>
      <s-button
        slot="secondary-actions"
        variant="secondary"
        commandFor="delete-modal"
        command="--hide"
      >
        Cancel
      </s-button>
    </s-modal>
  </s-stack>
  ```

- #### Modal with form fields

  ##### Description

  Modal containing form fields demonstrating how to structure input fields within a modal. Click the button to open the modal.

  ##### jsx

  ```jsx
  <s-stack gap="base">
    <s-button variant="primary" commandFor="edit-modal" command="--show">
      Edit customer
    </s-button>

    <s-modal id="edit-modal" heading="Edit customer information" size="large">
      <s-stack gap="base">
        <s-text-field
          label="Customer name"
          name="name"
          value="Sarah Johnson"
         />

        <s-email-field
          label="Email address"
          name="email"
          value="sarah@example.com"
         />

        <s-text-field
          label="Phone number"
          name="phone"
          value="+1 555-0123"
         />

        <s-select label="Customer group" name="group">
          <s-option value="retail">Retail</s-option>
          <s-option value="wholesale" selected>
            Wholesale
          </s-option>
          <s-option value="vip">VIP</s-option>
        </s-select>
      </s-stack>

      <s-button
        slot="primary-action"
        variant="primary"
        commandFor="edit-modal"
        command="--hide"
      >
        Save changes
      </s-button>
      <s-button
        slot="secondary-actions"
        variant="secondary"
        commandFor="edit-modal"
        command="--hide"
      >
        Cancel
      </s-button>
    </s-modal>
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack gap="base">
    <s-button variant="primary" commandFor="edit-modal" command="--show">
      Edit customer
    </s-button>

    <s-modal id="edit-modal" heading="Edit customer information" size="large">
      <s-stack gap="base">
        <s-text-field
          label="Customer name"
          name="name"
          value="Sarah Johnson"
        ></s-text-field>

        <s-email-field
          label="Email address"
          name="email"
          value="sarah@example.com"
        ></s-email-field>

        <s-text-field
          label="Phone number"
          name="phone"
          value="+1 555-0123"
        ></s-text-field>

        <s-select label="Customer group" name="group">
          <s-option value="retail">Retail</s-option>
          <s-option value="wholesale" selected>Wholesale</s-option>
          <s-option value="vip">VIP</s-option>
        </s-select>
      </s-stack>

      <s-button
        slot="primary-action"
        variant="primary"
        commandFor="edit-modal"
        command="--hide"
      >
        Save changes
      </s-button>
      <s-button
        slot="secondary-actions"
        variant="secondary"
        commandFor="edit-modal"
        command="--hide"
      >
        Cancel
      </s-button>
    </s-modal>
  </s-stack>
  ```

- #### Different modal sizes

  ##### Description

  Demonstrates various modal sizes for different content requirements. Click each button to see different modal sizes.

  ##### jsx

  ```jsx
  <s-stack gap="base">
    <s-stack direction="inline" gap="base">
      <s-button commandFor="small-modal" command="--show">
        Small modal
      </s-button>
      <s-button commandFor="large-modal" command="--show">
        Large modal
      </s-button>
    </s-stack>

    {/* Small modal for quick confirmations */}
    <s-modal id="small-modal" heading="Confirm action" size="small-100">
      <s-text>Are you sure you want to proceed?</s-text>
      <s-button
        slot="primary-action"
        variant="primary"
        commandFor="small-modal"
        command="--hide"
      >
        Confirm
      </s-button>
      <s-button
        slot="secondary-actions"
        variant="secondary"
        commandFor="small-modal"
        command="--hide"
      >
        Cancel
      </s-button>
    </s-modal>

    {/* Large modal for detailed content */}
    <s-modal id="large-modal" heading="Order details" size="large-100">
      <s-stack gap="base">
        <s-section>
          <s-heading>Order #1001</s-heading>
          <s-text>Placed on March 15, 2024</s-text>
        </s-section>

        <s-divider />

        <s-section>
          <s-heading>Items</s-heading>
          <s-stack gap="small">
            <s-text>Winter jacket - $89.99</s-text>
            <s-text>Wool scarf - $29.99</s-text>
            <s-text>Leather gloves - $45.99</s-text>
          </s-stack>
        </s-section>

        <s-divider />

        <s-text type="strong">Total: $165.97</s-text>
      </s-stack>

      <s-button
        slot="primary-action"
        variant="primary"
        commandFor="large-modal"
        command="--hide"
      >
        Close
      </s-button>
    </s-modal>
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack gap="base">
    <s-stack direction="inline" gap="base">
      <s-button commandFor="small-modal" command="--show">Small modal</s-button>
      <s-button commandFor="large-modal" command="--show">Large modal</s-button>
    </s-stack>

    <!-- Small modal for quick confirmations -->
    <s-modal id="small-modal" heading="Confirm action" size="small-100">
      <s-text>Are you sure you want to proceed?</s-text>
      <s-button
        slot="primary-action"
        variant="primary"
        commandFor="small-modal"
        command="--hide"
      >
        Confirm
      </s-button>
      <s-button
        slot="secondary-actions"
        variant="secondary"
        commandFor="small-modal"
        command="--hide"
      >
        Cancel
      </s-button>
    </s-modal>

    <!-- Large modal for detailed content -->
    <s-modal id="large-modal" heading="Order details" size="large-100">
      <s-stack gap="base">
        <s-section>
          <s-heading>Order #1001</s-heading>
          <s-text>Placed on March 15, 2024</s-text>
        </s-section>

        <s-divider></s-divider>

        <s-section>
          <s-heading>Items</s-heading>
          <s-stack gap="small">
            <s-text>Winter jacket - $89.99</s-text>
            <s-text>Wool scarf - $29.99</s-text>
            <s-text>Leather gloves - $45.99</s-text>
          </s-stack>
        </s-section>

        <s-divider></s-divider>

        <s-text type="strong">Total: $165.97</s-text>
      </s-stack>

      <s-button
        slot="primary-action"
        variant="primary"
        commandFor="large-modal"
        command="--hide"
      >
        Close
      </s-button>
    </s-modal>
  </s-stack>
  ```

- #### Modal without padding

  ##### Description

  Modal with no padding for full-width content. Click to view the modal.

  ##### jsx

  ```jsx
  <s-stack gap="base">
    <s-button commandFor="image-modal" command="--show">
      View product image
    </s-button>

    <s-modal id="image-modal" heading="Product image" padding="none">
      <s-box background="subdued" padding="base">
        <s-text>Image would display here with full width</s-text>
      </s-box>

      <s-button
        slot="secondary-actions"
        variant="secondary"
        commandFor="image-modal"
        command="--hide"
      >
        Close
      </s-button>
    </s-modal>
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack gap="base">
    <s-button commandFor="image-modal" command="--show">
      View product image
    </s-button>

    <s-modal id="image-modal" heading="Product image" padding="none">
      <s-box background="subdued" padding="base">
        <s-text>Image would display here with full width</s-text>
      </s-box>

      <s-button
        slot="secondary-actions"
        variant="secondary"
        commandFor="image-modal"
        command="--hide"
      >
        Close
      </s-button>
    </s-modal>
  </s-stack>
  ```

## Usage

Modals are closed by default and should be triggered by a button using the `commandFor` attribute. The button's `commandFor` value should match the modal's `id`.

## Useful for

- Focusing on a specific task or piece of information
- Completing a flow that needs dedicated attention
- Confirming a significant action before proceeding
- Viewing information that's only temporarily relevant

## Best practices

- Use for focused, specific tasks that require merchants to make a decision or acknowledge critical information
- Include a prominent and clear call to action
- Don't nest modals (avoid launching one modal from another)
- Have concise and descriptive title and button text
- Use thoughtfully and sparingly—don't create unnecessary interruptions
- Use as a last resort for important decisions, not for contextual tools or actions that could happen on the page directly

## Content guidelines

- Use 1-3 word titles in sentence case without punctuation
- Keep body content to 1-2 short sentences
- For destructive actions, explain the consequences
- Use clear action verbs for buttons (e.g., "Delete", "Edit") instead of vague language like "Yes" or "OK"

</page>

<page>
---
title: Popover
description: >-
  Popovers are used to display content in an overlay that can be triggered by a
  button.
api_name: app-home
source_url:
  html: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/overlays/popover
  md: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/overlays/popover.md
---

# Popover

Popovers are used to display content in an overlay that can be triggered by a button.

## Properties

- **blockSize**

  **SizeUnitsOrAuto**

  **Default: 'auto'**

  Adjust the block size.

- **inlineSize**

  **SizeUnitsOrAuto**

  **Default: 'auto'**

  Adjust the inline size.

- **maxBlockSize**

  **SizeUnitsOrNone**

  **Default: 'none'**

  Adjust the maximum block size.

- **maxInlineSize**

  **SizeUnitsOrNone**

  **Default: 'none'**

  Adjust the maximum inline size.

- **minBlockSize**

  **SizeUnits**

  **Default: '0'**

  Adjust the minimum block size.

- **minInlineSize**

  **SizeUnits**

  **Default: '0'**

  Adjust the minimum inline size.

### SizeUnitsOrAuto

```ts
SizeUnits | 'auto'
```

### SizeUnits

```ts
`${number}px` | `${number}%` | `0`
```

### SizeUnitsOrNone

```ts
SizeUnits | 'none'
```

## Events

Learn more about [registering events](https://shopify.dev/docs/api/app-home/using-polaris-components#event-handling).

- **afterhide**

  **CallbackEventListener\<TTagName> | null**

- **aftershow**

  **CallbackEventListener\<TTagName> | null**

- **aftertoggle**

  **CallbackEventListener\<TTagName> | null**

- **hide**

  **CallbackEventListener\<TTagName> | null**

- **show**

  **CallbackEventListener\<TTagName> | null**

- **toggle**

  **CallbackEventListener\<TTagName> | null**

### CallbackEventListener

```ts
(EventListener & {
      (event: CallbackEvent<T>): void;
    }) | null
```

### CallbackEvent

```ts
Event & {
  currentTarget: HTMLElementTagNameMap[T];
}
```

## Slots

- **children**

  **HTMLElement**

  The content of the Popover.

Examples

### Examples

- #### Code

  ##### jsx

  ```jsx
  <>
    <s-button commandFor="product-options-popover">Product options</s-button>

    <s-popover id="product-options-popover">
      <s-stack direction="block">
        <s-button variant="tertiary">Import</s-button>
        <s-button variant="tertiary">Export</s-button>
      </s-stack>
    </s-popover>
  </>
  ```

  ##### html

  ```html
  <s-button commandFor="product-options-popover">Product options</s-button>

  <s-popover id="product-options-popover">
    <s-stack direction="block">
      <s-button variant="tertiary">Import</s-button>
      <s-button variant="tertiary">Export</s-button>
    </s-stack>
  </s-popover>
  ```

- #### Popover with notifications

  ##### Description

  Popover displaying admin notifications such as new orders, inventory alerts, and payment confirmations, demonstrating how popovers can show informational content without cluttering the main interface.

  ##### jsx

  ```jsx
  <>
    <s-button commandFor="notifications-popover" icon="notification">
      Notifications
    </s-button>

    <s-popover id="notifications-popover">
      <s-box padding="base">
        <s-stack gap="small-200">
          <s-stack gap="small">
            <s-heading>New order received</s-heading>
            <s-paragraph color="subdued">Order #1234 was placed 5 minutes ago</s-paragraph>
          </s-stack>

          <s-divider />

          <s-stack gap="small">
            <s-heading>Low inventory alert</s-heading>
            <s-paragraph color="subdued">3 products are running low on stock</s-paragraph>
          </s-stack>

          <s-divider />

          <s-stack gap="small">
            <s-heading>Payment processed</s-heading>
            <s-paragraph color="subdued">$250.00 payment confirmed for order #1230</s-paragraph>
          </s-stack>
        </s-stack>
      </s-box>
    </s-popover>
  </>
  ```

  ##### html

  ```html
  <s-button commandFor="notifications-popover" icon="notification">
    Notifications
  </s-button>

  <s-popover id="notifications-popover">
    <s-box padding="base">
      <s-stack gap="small-200">
        <s-stack gap="small">
          <s-heading>New order received</s-heading>
          <s-paragraph color="subdued">
            Order #1234 was placed 5 minutes ago
          </s-paragraph>
        </s-stack>

        <s-divider />

        <s-stack gap="small">
          <s-heading>Low inventory alert</s-heading>
          <s-paragraph color="subdued">
            3 products are running low on stock
          </s-paragraph>
        </s-stack>

        <s-divider />

        <s-stack gap="small">
          <s-heading>Payment processed</s-heading>
          <s-paragraph color="subdued">
            $250.00 payment confirmed for order #1230
          </s-paragraph>
        </s-stack>
      </s-stack>
    </s-box>
  </s-popover>
  ```

- #### Popover with choice list

  ##### Description

  Popover containing a choice list and action button demonstrating how popovers can be used for settings and configuration interfaces.

  ##### jsx

  ```jsx
  <>
    <s-button commandFor="table-settings-popover" icon="settings">
      Columns
    </s-button>

    <s-popover id="table-settings-popover">
      <s-box padding="base">
        <s-stack gap="small-200">
          <s-stack gap="small">
            <s-heading>Choose columns to display</s-heading>
            <s-choice-list label="Select columns to display">
              <s-choice value="sku" selected>
                Sku
              </s-choice>
              <s-choice value="inventory" selected>
                Inventory
              </s-choice>
              <s-choice value="price" selected>
                Price
              </s-choice>
              <s-choice value="vendor">Vendor</s-choice>
              <s-choice value="type">Product type</s-choice>
            </s-choice-list>
          </s-stack>
          <s-button variant="primary">Apply changes</s-button>
        </s-stack>
      </s-box>
    </s-popover>
  </>
  ```

  ##### html

  ```html
  <s-button commandFor="table-settings-popover" disclosure="true" icon="settings">
    Columns
  </s-button>

  <s-popover id="table-settings-popover">
    <s-box padding="base">
      <s-stack gap="small-200">
        <s-stack gap="small">
          <s-heading>Choose columns to display</s-heading>
          <s-choice-list label="Select columns to display">
            <s-choice value="sku" selected>Sku</s-choice>
            <s-choice value="inventory" selected>Inventory</s-choice>
            <s-choice value="price" selected>Price</s-choice>
            <s-choice value="vendor">Vendor</s-choice>
            <s-choice value="type">Product type</s-choice>
          </s-choice-list>
        </s-stack>
        <s-button variant="primary">Apply changes</s-button>
      </s-stack>
    </s-box>
  </s-popover>
  ```

- #### Popover with inventory details

  ##### Description

  Popover displaying detailed inventory information using Box padding instead of Section, demonstrating an alternative layout approach for data-focused content.

  ##### jsx

  ```jsx
  <>
    <s-button commandFor="stock-popover" icon="info">
      Stock details
    </s-button>

    <s-popover id="stock-popover">
      <s-box padding="base">
        <s-stack gap="small">
          <s-stack gap="small-200">
            <s-stack direction="inline" justifyContent="space-between">
              <s-text color="subdued">Available</s-text>
              <s-text>127 units</s-text>
            </s-stack>

            <s-stack direction="inline" justifyContent="space-between">
              <s-text color="subdued">Reserved</s-text>
              <s-text>15 units</s-text>
            </s-stack>

            <s-stack direction="inline" justifyContent="space-between">
              <s-text color="subdued">In transit</s-text>
              <s-text>50 units</s-text>
            </s-stack>
          </s-stack>

          <s-divider />

          <s-stack direction="inline" justifyContent="space-between">
            <s-text>Total stock</s-text>
            <s-text>192 units</s-text>
          </s-stack>

          <s-button variant="secondary">View full inventory report</s-button>
        </s-stack>
      </s-box>
    </s-popover>
  </>
  ```

  ##### html

  ```html
  <s-button commandFor="stock-popover" icon="info">
    Stock details
  </s-button>

  <s-popover id="stock-popover">
    <s-box padding="base">
      <s-stack gap="small">
        <s-stack gap="small-200">
          <s-stack direction="inline" justifyContent="space-between">
            <s-text color="subdued">Available</s-text>
            <s-text>127 units</s-text>
          </s-stack>

          <s-stack direction="inline" justifyContent="space-between">
            <s-text color="subdued">Reserved</s-text>
            <s-text>15 units</s-text>
          </s-stack>

          <s-stack direction="inline" justifyContent="space-between">
            <s-text color="subdued">In transit</s-text>
            <s-text>50 units</s-text>
          </s-stack>
        </s-stack>

        <s-divider />

        <s-stack direction="inline" justifyContent="space-between">
          <s-text>Total stock</s-text>
          <s-text>192 units</s-text>
        </s-stack>

        <s-button variant="secondary">View full inventory report</s-button>
      </s-stack>
    </s-box>
  </s-popover>
  ```

## Usage

Popovers are closed by default and should be triggered by a button using the `commandFor` attribute. The button's `commandFor` value should match the popover's `id`. The popover's position is determined by the button that triggers it.

## Best practices

- Use for secondary or less important information and actions since they're hidden until triggered
- Contain actions that share a relationship to each other
- Be triggered by a clearly labeled default or tertiary button

## Content guidelines

- Use clear action verbs in the {verb}+{noun} format (e.g., "Create order", "Edit HTML")
- Avoid unnecessary words like "the", "an", or "a"

</page>

<page>
---
title: Page
description: ' Use `s-page` as the main container for placing content in your app. Page comes with preset layouts and automatically adds spacing between elements.'
api_name: app-home
source_url:
  html: 'https://shopify.dev/docs/api/app-home/polaris-web-components/structure/page'
  md: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/structure/page.md
---

# Page

Use `s-page` as the main container for placing content in your app. Page comes with preset layouts and automatically adds spacing between elements.

## Properties

Use as the outer wrapper of a page

- **heading**

  **string**

  The main page heading

- **inlineSize**

  **"small" | "base" | "large"**

  **Default: 'base'**

  The inline size of the page

  - `base` corresponds to a set default inline size
  - `large` full width with whitespace

## Slots

- **aside**

  **HTMLElement**

  The content to display in the aside section of the page.

  This slot is only rendered when `inlineSize` is "base".

- **breadcrumb-actions**

  **HTMLElement**

  Navigations back actions for the page.

  Only accepts `Link` components.

- **children**

  **HTMLElement**

  The content of the Page.

- **primary-action**

  **HTMLElement**

  The primary action for the page.

  Only accepts a single `Button` component with a `variant` of `primary`.

- **secondary-actions**

  **HTMLElement**

  Secondary actions for the page.

  Only accepts `ButtonGroup` and `Button` components with a `variant` of `secondary` or `auto`.

Examples

### Examples

- ####

  ##### jsx

  ```jsx
  <s-page heading="Products">
    <s-section>
      <s-text>Hello World</s-text>
    </s-section>
  </s-page>
  ```

  ##### html

  ```html
  <s-page heading="Products">
    <s-section>
      <s-text>Hello World</s-text>
    </s-section>
  </s-page>
  ```

- #### Page with heading

  ##### Description

  Shows a page with a clear heading and descriptive text, illustrating how to use the page component with a title.

  ##### jsx

  ```jsx
  <s-page heading="Product catalog" inlineSize="base">
    <s-section>
      <s-text>Manage your product catalog and inventory.</s-text>
    </s-section>
  </s-page>
  ```

  ##### html

  ```html
  <s-page heading="Product catalog" inline-size="base">
    <s-section>
      <s-text>Manage your product catalog and inventory.</s-text>
    </s-section>
  </s-page>
  ```

- #### Small inline size for focused content

  ##### Description

  Illustrates a page with a small inline size, ideal for focused, compact content like settings or forms with minimal information.

  ##### jsx

  ```jsx
  <s-page heading="Store settings" inlineSize="small">
    <s-section>
      <s-stack gap="base">
        <s-text>Configure your basic store preferences.</s-text>
        <s-text-field label="Store name" />
        <s-button variant="primary">Save</s-button>
      </s-stack>
    </s-section>
  </s-page>
  ```

  ##### html

  ```html
  <s-page heading="Store settings" inline-size="small">
    <s-section>
      <s-stack gap="base">
        <s-text>Configure your basic store preferences.</s-text>
        <s-text-field label="Store name"></s-text-field>
        <s-button variant="primary">Save</s-button>
      </s-stack>
    </s-section>
  </s-page>
  ```

- #### Large inline size for wide content

  ##### Description

  Demonstrates a page with a large inline size, perfect for displaying broader content like analytics or dashboards with multiple information sections.

  ##### jsx

  ```jsx
  <s-page heading="Store analytics" inlineSize="large">
    <s-section>
      <s-stack gap="base">
        <s-text>Monitor your store performance across all channels.</s-text>
        <s-grid>
          <s-grid-item>
            <s-box
              padding="base"
              background="base"
              borderWidth="base"
              borderColor="base"
              borderRadius="base"
            >
              <s-heading>Sales</s-heading>
              <s-text type="strong">$12,456</s-text>
            </s-box>
          </s-grid-item>
          <s-grid-item>
            <s-box
              padding="base"
              background="base"
              borderWidth="base"
              borderColor="base"
              borderRadius="base"
            >
              <s-heading>Orders</s-heading>
              <s-text type="strong">145</s-text>
            </s-box>
          </s-grid-item>
        </s-grid>
      </s-stack>
    </s-section>
  </s-page>
  ```

  ##### html

  ```html
  <s-page heading="Store analytics" inline-size="large">
    <s-section>
      <s-stack gap="base">
        <s-text>Monitor your store performance across all channels.</s-text>
        <s-grid>
          <s-grid-item>
            <s-box
              padding="base"
              background="base"
              borderWidth="base"
              borderColor="base"
              borderRadius="base"
            >
              <s-heading>Sales</s-heading>
              <s-text type="strong">$12,456</s-text>
            </s-box>
          </s-grid-item>
          <s-grid-item>
            <s-box
              padding="base"
              background="base"
              borderWidth="base"
              borderColor="base"
              borderRadius="base"
            >
              <s-heading>Orders</s-heading>
              <s-text type="strong">145</s-text>
            </s-box>
          </s-grid-item>
        </s-grid>
      </s-stack>
    </s-section>
  </s-page>
  ```

- #### Page with breadcrumbs and title

  ##### Description

  Shows a page with breadcrumb navigation and a descriptive heading, helping users understand their location in the navigation hierarchy.

  ##### jsx

  ```jsx
  <s-page heading="Edit Product" inlineSize="base">
    <s-link slot="breadcrumb-actions" href="/products">Products</s-link>
    <s-link slot="breadcrumb-actions" href="/products/123">Acme Widget</s-link>
    <s-section>
      <s-text>Update your product information and settings.</s-text>
    </s-section>
  </s-page>
  ```

  ##### html

  ```html
  <s-page heading="Edit Product" inline-size="base">
    <s-link slot="breadcrumb-actions" href="/products">Products</s-link>
    <s-link slot="breadcrumb-actions" href="/products/123">Acme Widget</s-link>
    <s-section>
      <s-text>Update your product information and settings.</s-text>
    </s-section>
  </s-page>
  ```

- #### Page with primary and secondary actions

  ##### Description

  Demonstrates a page with a primary action button and secondary action buttons, showing how to provide main and related actions alongside the page heading.

  ##### jsx

  ```jsx
  <s-page heading="Products" inlineSize="base">
    <s-button slot="secondary-actions">Preview</s-button>
    <s-button slot="secondary-actions">Duplicate</s-button>
    <s-button slot="primary-action" variant="primary">Save</s-button>
    <s-section>
      <s-text>Manage your products and organize your catalog.</s-text>
    </s-section>
  </s-page>
  ```

  ##### html

  ```html
  <s-page heading="Products" inline-size="base">
    <s-button slot="secondary-actions">Preview</s-button>
    <s-button slot="secondary-actions">Duplicate</s-button>
    <s-button slot="primary-action" variant="primary">Save</s-button>
    <s-section>
      <s-text>Manage your products and organize your catalog.</s-text>
    </s-section>
  </s-page>
  ```

## Best practices

- Always provide a title that describes the current page
- Include breadcrumbs when the page is part of a flow
- Include page actions in the header only if they are relevant to the entire page
- Include no more than one primary action and 3 secondary actions per page
- Don't include any actions at the bottom of the page

## Content guidelines

- Use sentence case and avoid unnecessary words
- Don't include punctuation like periods or exclamation marks
- Page titles should clearly communicate the page purpose
- Page actions should use a verb or verb + noun phrase (e.g., "Create store", "Edit product")

</page>

<page>
---
title: Chip
description: >-
  Represents a set of user-supplied keywords that help label, organize, and
  categorize objects. Used to categorize or highlight content attributes. They
  are often displayed near the content they classify, enhancing discoverability
  by allowing users to identify items with similar properties.
api_name: app-home
source_url:
  html: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/typography-and-content/chip
  md: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/typography-and-content/chip.md
---

# Chip

Represents a set of user-supplied keywords that help label, organize, and categorize objects. Used to categorize or highlight content attributes. They are often displayed near the content they classify, enhancing discoverability by allowing users to identify items with similar properties.

## Properties

- **accessibilityLabel**

  **string**

  A label that describes the purpose or contents of the Chip. It will be read to users using assistive technologies such as screen readers.

- **color**

  **ColorKeyword**

  **Default: 'base'**

  Modify the color to be more or less intense.

### ColorKeyword

```ts
'subdued' | 'base' | 'strong'
```

## Slots

- **children**

  **HTMLElement**

  The content of the Chip.

- **graphic**

  **HTMLElement**

  The graphic to display in the chip.

  Only accepts `Icon` components.

Examples

### Examples

- #### Code

  ##### jsx

  ```jsx
  <s-chip>Chip</s-chip>
  ```

  ##### html

  ```html
  <s-chip>Chip</s-chip>
  ```

- #### Basic usage

  ##### Description

  Simple chip displaying product status without an icon.

  ##### jsx

  ```jsx
  <s-chip color="base" accessibilityLabel="Product status indicator">
    Active
  </s-chip>
  ```

  ##### html

  ```html
  <s-chip color="base" accessibilityLabel="Product status indicator">
    Active
  </s-chip>
  ```

- #### With icon graphic

  ##### Description

  Chip enhanced with an icon to provide visual context for the category.

  ##### jsx

  ```jsx
  <s-chip color="strong" accessibilityLabel="Product category">
    <s-icon slot="graphic" type="catalog-product" size="small" />
    Electronics
  </s-chip>
  ```

  ##### html

  ```html
  <s-chip color="strong" accessibilityLabel="Product category">
    <s-icon slot="graphic" type="catalog-product" size="small"></s-icon>
    Electronics
  </s-chip>
  ```

- #### Color variants

  ##### Description

  Demonstrates all three color variants for different levels of visual emphasis.

  ##### jsx

  ```jsx
  <s-stack direction="inline" gap="base">
    <s-chip color="subdued" accessibilityLabel="Secondary information">
      Draft
    </s-chip>
    <s-chip color="base" accessibilityLabel="Standard information">
      Published
    </s-chip>
    <s-chip color="strong" accessibilityLabel="Important status">
      <s-icon slot="graphic" type="check" size="small" />
      Verified
    </s-chip>
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack direction="inline" gap="base">
    <s-chip color="subdued" accessibilityLabel="Secondary information">
      Draft
    </s-chip>
    <s-chip color="base" accessibilityLabel="Standard information">
      Published
    </s-chip>
    <s-chip color="strong" accessibilityLabel="Important status">
      <s-icon slot="graphic" type="check" size="small"></s-icon>
      Verified
    </s-chip>
  </s-stack>
  ```

- #### Product status

  ##### Description

  Common status indicators demonstrating chips in real-world product management scenarios.

  ##### jsx

  ```jsx
  <s-stack direction="inline" gap="base">
    <s-chip color="base" accessibilityLabel="Product status">
      Active
    </s-chip>
    <s-chip color="subdued" accessibilityLabel="Product status">
      Draft
    </s-chip>
    <s-chip color="strong" accessibilityLabel="Product status">
      <s-icon slot="graphic" type="check" size="small" />
      Published
    </s-chip>
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack direction="inline" gap="base">
    <s-chip color="base" accessibilityLabel="Product status">Active</s-chip>
    <s-chip color="subdued" accessibilityLabel="Product status">Draft</s-chip>
    <s-chip color="strong" accessibilityLabel="Product status">
      <s-icon slot="graphic" type="check" size="small"></s-icon>
      Published
    </s-chip>
  </s-stack>
  ```

- #### Text truncation

  ##### Description

  Demonstrates automatic text truncation for long content within a constrained width.

  ##### jsx

  ```jsx
  <s-box maxInlineSize="200px">
    <s-stack gap="base">
      <s-chip color="base" accessibilityLabel="Long product name">
        This is a very long product name that will be truncated with ellipsis when
        it exceeds the container width
      </s-chip>
      <s-chip color="strong" accessibilityLabel="Long category name">
        <s-icon slot="graphic" type="catalog-product" size="small" />
        Electronics and computer accessories category with extended description
      </s-chip>
    </s-stack>
  </s-box>
  ```

  ##### html

  ```html
  <s-box maxInlineSize="200px">
    <s-stack gap="base">
      <s-chip color="base" accessibilityLabel="Long product name">
        This is a very long product name that will be truncated with ellipsis when
        it exceeds the container width
      </s-chip>
      <s-chip color="strong" accessibilityLabel="Long category name">
        <s-icon slot="graphic" type="catalog-product" size="small"></s-icon>
        Electronics and computer accessories category with extended description
      </s-chip>
    </s-stack>
  </s-box>
  ```

## Useful for

- Labeling, organizing, and categorizing objects
- Highlighting content attributes
- Enhancing discoverability by identifying items with similar properties

## Best practices

- `subdued`: use for secondary or less important information
- `base`: use as default color
- `strong`: use for important or verified status
- Text truncates automatically, keep labels short to avoid truncation
- Chips are static indicators, not interactive or dismissible. For interactive chips, use ClickableChip
- Add icons to `graphic` slot to provide visual context
- Display chips near the content they classify

## Content guidelines

Chip labels should:

- Be short and concise to avoid truncation
- Use `accessibilityLabel` to describe purpose for screen readers
- Common status labels: `Active`, `Draft`, `Published`, `Verified`
- Common category labels: `Product type`, `Collection`, `Tag name`

</page>

<page>
---
title: Heading
description: >-
  Renders hierarchical titles to communicate the structure and organization of
  page content. Heading levels adjust automatically based on nesting within
  parent Section components, ensuring a meaningful and accessible page outline.
api_name: app-home
source_url:
  html: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/typography-and-content/heading
  md: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/typography-and-content/heading.md
---

# Heading

Renders hierarchical titles to communicate the structure and organization of page content. Heading levels adjust automatically based on nesting within parent Section components, ensuring a meaningful and accessible page outline.

## Properties

- **accessibilityRole**

  **"none" | "presentation" | "heading"**

  **Default: 'heading'**

  Sets the semantic meaning of the component’s content. When set, the role will be used by assistive technologies to help users navigate the page.

  - `heading`: defines the element as a heading to a page or section.
  - `presentation`: the heading level will be stripped, and will prevent the element’s implicit ARIA semantics from being exposed to the accessibility tree.
  - `none`: a synonym for the `presentation` role.

- **accessibilityVisibility**

  **"visible" | "hidden" | "exclusive"**

  **Default: 'visible'**

  Changes the visibility of the element.

  - `visible`: the element is visible to all users.
  - `hidden`: the element is removed from the accessibility tree but remains visible.
  - `exclusive`: the element is visually hidden but remains in the accessibility tree.

- **lineClamp**

  **number**

  **Default: Infinity - no truncation is applied**

  Truncates the text content to the specified number of lines.

## Slots

- **children**

  **HTMLElement**

  The content of the Heading.

Examples

### Examples

- #### Code

  ##### jsx

  ```jsx
  <s-heading>Online store dashboard</s-heading>
  ```

  ##### html

  ```html
  <s-heading>Online store dashboard</s-heading>
  ```

- #### Basic heading

  ##### Description

  Standard heading for section titles and page content organization that creates a simple, clean title for a content section.

  ##### jsx

  ```jsx
  <s-heading>Product details</s-heading>
  ```

  ##### html

  ```html
  <s-heading>Product details</s-heading>
  ```

- #### Heading with line clamping

  ##### Description

  Truncated heading that limits text to a specified number of lines, using ellipsis to indicate additional content for long product names or constrained layouts.

  ##### jsx

  ```jsx
  <s-box inlineSize="200px">
    <s-heading lineClamp={2}>
      Premium organic cotton t-shirt with sustainable manufacturing practices
    </s-heading>
  </s-box>
  ```

  ##### html

  ```html
  <s-box inlineSize="200px">
    <s-heading lineClamp="2">
      Premium organic cotton t-shirt with sustainable manufacturing practices
    </s-heading>
  </s-box>
  ```

- #### Heading with custom accessibility

  ##### Description

  Heading configured with custom ARIA roles and visibility settings to meet specific accessibility requirements or design constraints.

  ##### jsx

  ```jsx
  <s-heading accessibilityRole="presentation" accessibilityVisibility="hidden">
    Sale badge
  </s-heading>
  ```

  ##### html

  ```html
  <s-heading accessibilityRole="presentation" accessibilityVisibility="hidden">
    Sale badge
  </s-heading>
  ```

- #### Heading within section hierarchy

  ##### Description

  Demonstrates nested heading structure that automatically adjusts heading levels (h2, h3, h4) based on the current section depth, ensuring proper semantic document structure.

  ##### jsx

  ```jsx
  <s-section>
    <s-heading>Order information</s-heading>
    {/* Renders as h2 */}
    <s-section>
      <s-heading>Shipping details</s-heading>
      {/* Renders as h3 */}
      <s-section>
        <s-heading>Tracking updates</s-heading>
        {/* Renders as h4 */}
      </s-section>
    </s-section>
  </s-section>
  ```

  ##### html

  ```html
  <s-section>
    <s-heading>Order information</s-heading>
    <!-- Renders as h2 -->
    <s-section>
      <s-heading>Shipping details</s-heading>
      <!-- Renders as h3 -->
      <s-section>
        <s-heading>Tracking updates</s-heading>
        <!-- Renders as h4 -->
      </s-section>
    </s-section>
  </s-section>
  ```

## Useful for

- Creating titles and subtitles for your content that are consistent across your app.
- Helping users with visual impairments navigate through content effectively using assistive technologies like screen readers.

## Considerations

- The level of the heading is automatically determined by how deeply it's nested inside other components, starting from h2.
- Default to using the `heading` property in `s-section`. The `s-heading` component should only be used if you need to implement a custom layout for your heading in the UI.

## Best practices

- Use short headings to make your content scannable.
- Use plain and clear terms.
- Don't use jargon or technical language.
- Don't use different terms to describe the same thing.
- Don't duplicate content.

</page>

<page>
---
title: Paragraph
description: >-
  Displays a block of text and can contain inline elements such as buttons,
  links, or emphasized text. Use to present standalone blocks of content as
  opposed to inline text.
api_name: app-home
source_url:
  html: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/typography-and-content/paragraph
  md: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/typography-and-content/paragraph.md
---

# Paragraph

Displays a block of text and can contain inline elements such as buttons, links, or emphasized text. Use to present standalone blocks of content as opposed to inline text.

## Properties

- **accessibilityVisibility**

  **"visible" | "hidden" | "exclusive"**

  **Default: 'visible'**

  Changes the visibility of the element.

  - `visible`: the element is visible to all users.
  - `hidden`: the element is removed from the accessibility tree but remains visible.
  - `exclusive`: the element is visually hidden but remains in the accessibility tree.

- **color**

  **"base" | "subdued"**

  **Default: 'base'**

  Modify the color to be more or less intense.

- **dir**

  **"" | "auto" | "ltr" | "rtl"**

  **Default: ''**

  Indicates the directionality of the element’s text.

  - `ltr`: languages written from left to right (e.g. English)
  - `rtl`: languages written from right to left (e.g. Arabic)
  - `auto`: the user agent determines the direction based on the content
  - `''`: direction is inherited from parent elements (equivalent to not setting the attribute)

- **fontVariantNumeric**

  **"auto" | "normal" | "tabular-nums"**

  **Default: 'auto' - inherit from the parent element**

  Set the numeric properties of the font.

- **lineClamp**

  **number**

  **Default: Infinity - no truncation is applied**

  Truncates the text content to the specified number of lines.

- **tone**

  **"info" | "success" | "warning" | "critical" | "auto" | "neutral" | "caution"**

  **Default: 'auto'**

  Sets the tone of the component, based on the intention of the information being conveyed.

## Slots

- **children**

  **HTMLElement**

  The content of the Paragraph.

Examples

### Examples

- #### Code

  ##### jsx

  ```jsx
  <s-paragraph>
    Shopify POS is the easiest way to sell your products in person. Available for
    iPad, iPhone, and Android.
  </s-paragraph>
  ```

  ##### html

  ```html
  <s-paragraph>
    Shopify POS is the easiest way to sell your products in person. Available for
    iPad, iPhone, and Android.
  </s-paragraph>
  ```

- #### Basic Usage

  ##### Description

  Demonstrates a simple paragraph with default styling, showing how to use the paragraph component for standard text content.

  ##### jsx

  ```jsx
  <s-paragraph>
    Track inventory across all your retail locations in real-time.
  </s-paragraph>
  ```

  ##### html

  ```html
  <s-paragraph>
    Track inventory across all your retail locations in real-time.
  </s-paragraph>
  ```

- #### With Tone and Color

  ##### Description

  Illustrates how to apply different tones and color variations to convey different types of information, such as informational and success messages.

  ##### jsx

  ```jsx
  <s-section>
    <s-paragraph tone="info" color="base">
      Your order will be processed within 2-3 business days.
    </s-paragraph>

    <s-paragraph tone="success" color="subdued">
      Payment successfully processed.
    </s-paragraph>
  </s-section>
  ```

  ##### html

  ```html
  <s-section>
    <s-paragraph tone="info" color="base">
      Your order will be processed within 2-3 business days.
    </s-paragraph>

    <s-paragraph tone="success" color="subdued">
      Payment successfully processed.
    </s-paragraph>
  </s-section>
  ```

- #### Line Clamping

  ##### Description

  Shows how to limit the number of lines displayed using the lineClamp prop, which truncates long text with an ellipsis after the specified number of lines.

  ##### jsx

  ```jsx
  <s-box inlineSize="300px">
    <s-paragraph lineClamp={1}>
      Premium organic cotton t-shirt featuring sustainable manufacturing
      processes, ethically sourced materials, and carbon-neutral shipping.
      Available in multiple colors and sizes with customization options for your
      brand.
    </s-paragraph>
  </s-box>
  ```

  ##### html

  ```html
  <s-box inlineSize="300px">
    <s-paragraph lineClamp="1">
      Premium organic cotton t-shirt featuring sustainable manufacturing
      processes, ethically sourced materials, and carbon-neutral shipping.
      Available in multiple colors and sizes with customization options for your
      brand.
    </s-paragraph>
  </s-box>
  ```

- #### Tabular Numbers

  ##### Description

  Demonstrates the use of tabular numbers with fontVariantNumeric, ensuring consistent alignment and readability for numerical data.

  ##### jsx

  ```jsx
  <s-paragraph fontVariantNumeric="tabular-nums">
    Orders: 1,234 Revenue: $45,678.90 Customers: 890
  </s-paragraph>
  ```

  ##### html

  ```html
  <s-paragraph fontVariantNumeric="tabular-nums">
    Orders: 1,234 Revenue: $45,678.90 Customers: 890
  </s-paragraph>
  ```

- #### RTL Support

  ##### Description

  Illustrates right-to-left (RTL) text rendering, showing how the paragraph component supports internationalization and different text directions.

  ##### jsx

  ```jsx
  <s-paragraph dir="rtl">
    محتوى النص باللغة العربية
  </s-paragraph>
  ```

  ##### html

  ```html
  <s-paragraph dir="rtl">
    محتوى النص باللغة العربية
  </s-paragraph>
  ```

- #### Screen Reader Text

  ##### Description

  Shows how to use the accessibilityVisibility prop to create text that is exclusively available to screen readers, improving accessibility for assistive technologies.

  ##### jsx

  ```jsx
  <s-paragraph accessibilityVisibility="exclusive">
    Table sorted by date, newest first.
  </s-paragraph>
  ```

  ##### html

  ```html
  <s-paragraph accessibilityVisibility="exclusive">
    Table sorted by date, newest first.
  </s-paragraph>
  ```

- #### Admin UI Patterns

  ##### Description

  Showcases various tone and color combinations for different administrative messages, illustrating how paragraph can communicate different types of information in a user interface.

  ##### jsx

  ```jsx
  <s-section>
    <s-paragraph tone="success" color="base">
      Payment successfully processed and order confirmed.
    </s-paragraph>

    <s-paragraph tone="warning" color="base">
      Inventory levels are running low for this product.
    </s-paragraph>

    <s-paragraph tone="critical" color="base">
      This order requires immediate attention due to shipping delays.
    </s-paragraph>

    <s-paragraph tone="info" color="base">
      Customer requested gift wrapping for this order.
    </s-paragraph>

    <s-paragraph tone="caution" color="base">
      Review shipping address before processing.
    </s-paragraph>
  </s-section>
  ```

  ##### html

  ```html
  <s-section>
    <s-paragraph tone="success" color="base">
      Payment successfully processed and order confirmed.
    </s-paragraph>

    <s-paragraph tone="warning" color="base">
      Inventory levels are running low for this product.
    </s-paragraph>

    <s-paragraph tone="critical" color="base">
      This order requires immediate attention due to shipping delays.
    </s-paragraph>

    <s-paragraph tone="info" color="base">
      Customer requested gift wrapping for this order.
    </s-paragraph>

    <s-paragraph tone="caution" color="base">
      Review shipping address before processing.
    </s-paragraph>
  </s-section>
  ```

## Useful for

- Displaying text content in a paragraph format.
- Grouping elements with the same style. For instance, icons inside a paragraph will automatically adopt the paragraph's tone.

## Best practices

- Use short paragraphs to make your content scannable.
- Use plain and clear terms.
- Don't use jargon or technical language.
- Don't use different terms to describe the same thing.
- Don't duplicate content.

</page>

<page>
---
title: Text
description: >-
  Displays inline text with specific visual styles or tones. Use to emphasize or
  differentiate words or phrases within a Paragraph or other block-level
  components.
api_name: app-home
source_url:
  html: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/typography-and-content/text
  md: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/typography-and-content/text.md
---

# Text

Displays inline text with specific visual styles or tones. Use to emphasize or differentiate words or phrases within a Paragraph or other block-level components.

## Properties

- **accessibilityVisibility**

  **"visible" | "hidden" | "exclusive"**

  **Default: 'visible'**

  Changes the visibility of the element.

  - `visible`: the element is visible to all users.
  - `hidden`: the element is removed from the accessibility tree but remains visible.
  - `exclusive`: the element is visually hidden but remains in the accessibility tree.

- **color**

  **"base" | "subdued"**

  **Default: 'base'**

  Modify the color to be more or less intense.

- **dir**

  **"" | "auto" | "ltr" | "rtl"**

  **Default: ''**

  Indicates the directionality of the element’s text.

  - `ltr`: languages written from left to right (e.g. English)
  - `rtl`: languages written from right to left (e.g. Arabic)
  - `auto`: the user agent determines the direction based on the content
  - `''`: direction is inherited from parent elements (equivalent to not setting the attribute)

- **fontVariantNumeric**

  **"auto" | "normal" | "tabular-nums"**

  **Default: 'auto' - inherit from the parent element**

  Set the numeric properties of the font.

- **interestFor**

  **string**

  ID of a component that should respond to interest (e.g. hover and focus) on this component.

- **tone**

  **"info" | "success" | "warning" | "critical" | "auto" | "neutral" | "caution"**

  **Default: 'auto'**

  Sets the tone of the component, based on the intention of the information being conveyed.

- **type**

  **"strong" | "generic" | "address" | "redundant"**

  **Default: 'generic'**

  Provide semantic meaning and default styling to the text.

  Other presentation properties on Text override the default styling.

## Slots

- **children**

  **HTMLElement**

  The content of the Text.

Examples

### Examples

- #### Code

  ##### jsx

  ```jsx
  <s-paragraph>
    <s-text type="strong">Name: </s-text>
    <s-text>Jane Doe</s-text>
  </s-paragraph>
  ```

  ##### html

  ```html
  <s-paragraph>
    <s-text type="strong">Name: </s-text>
    <s-text>Jane Doe</s-text>
  </s-paragraph>
  ```

- #### Basic Usage

  ##### Description

  Standard text content for general interface messaging and descriptions.

  ##### jsx

  ```jsx
  <s-text>
    Manage your products and inventory from one dashboard.
  </s-text>
  ```

  ##### html

  ```html
  <s-text>
    Manage your products and inventory from one dashboard.
  </s-text>
  ```

- #### Strong Text

  ##### Description

  Emphasized text for important messages and call-to-actions.

  ##### jsx

  ```jsx
  <s-text type="strong">
    Free shipping on orders over $50
  </s-text>
  ```

  ##### html

  ```html
  <s-text type="strong">
    Free shipping on orders over $50
  </s-text>
  ```

- #### Semantic Address

  ##### Description

  Structured address text with proper semantic meaning for screen readers.

  ##### jsx

  ```jsx
  <s-text type="address">
    123 Commerce Street, Toronto, ON M5V 2H1
  </s-text>
  ```

  ##### html

  ```html
  <s-text type="address">
    123 Commerce Street, Toronto, ON M5V 2H1
  </s-text>
  ```

- #### Tabular Numbers

  ##### Description

  Monospace number formatting for consistent alignment in tables and financial data.

  ##### jsx

  ```jsx
  <s-text fontVariantNumeric="tabular-nums">
    $1,234.56
  </s-text>
  ```

  ##### html

  ```html
  <s-text fontVariantNumeric="tabular-nums">
    $1,234.56
  </s-text>
  ```

- #### Status Tones

  ##### Description

  Color-coded text indicating different status states and semantic meanings.

  ##### jsx

  ```jsx
  <s-stack gap="small">
    <s-text tone="success">Order fulfilled</s-text>
    <s-text tone="critical">Payment failed</s-text>
    <s-text tone="warning">Low inventory</s-text>
  </s-stack>
  ```

  ##### html

  ```html
  <s-stack gap="small">
    <s-text tone="success">Order fulfilled</s-text>
    <s-text tone="critical">Payment failed</s-text>
    <s-text tone="warning">Low inventory</s-text>
  </s-stack>
  ```

- #### Accessibility Hidden Text

  ##### Description

  Text visible only to screen readers for providing additional context.

  ##### jsx

  ```jsx
  <s-text accessibilityVisibility="exclusive">
    Product prices include tax
  </s-text>
  ```

  ##### html

  ```html
  <s-text accessibilityVisibility="exclusive">
    Product prices include tax
  </s-text>
  ```

- #### Right-to-Left Text

  ##### Description

  Text direction support for RTL languages like Arabic and Hebrew.

  ##### jsx

  ```jsx
  <s-text dir="rtl">
    محتوى النص باللغة العربية
  </s-text>
  ```

  ##### html

  ```html
  <s-text dir="rtl">
    محتوى النص باللغة العربية
  </s-text>
  ```

- #### Subdued Color

  ##### Description

  Lower contrast text for secondary information and timestamps.

  ##### jsx

  ```jsx
  <s-text color="subdued">
    Last updated 2 hours ago
  </s-text>
  ```

  ##### html

  ```html
  <s-text color="subdued">
    Last updated 2 hours ago
  </s-text>
  ```

- #### Interest For Association

  ##### Description

  Text element associated with tooltips using the \`interestFor\` attribute to show additional information on hover or focus.

  ##### jsx

  ```jsx
  <>
    <s-tooltip id="sku-tooltip">
      SKU must be unique across all products and cannot be changed after creation
    </s-tooltip>
    <s-text color="subdued" interestFor="sku-tooltip">
      What is a product SKU?
    </s-text>
  </>
  ```

  ##### html

  ```html
  <s-tooltip id="sku-tooltip">
    SKU must be unique across all products and cannot be changed after creation
  </s-tooltip>
  <s-text color="subdued" interestFor="sku-tooltip">
    What is a product SKU?
  </s-text>
  ```

## Useful for

- Adding inline text elements such as labels or line errors.
- Applying different visual tones and text styles to specific words or phrases within a `s-paragraph`, such as a `strong` type or `critical` tone.

## Best practices

- Text elements display inline and will flow on the same line when placed next to each other. To stack multiple text elements vertically, wrap them in a Stack container or use multiple `s-paragraph` components.
- Use plain and clear terms.
- Don't use jargon or technical language.
- Don't use different terms to describe the same thing.
- Don't duplicate content.

</page>

<page>
---
title: Tooltip
description: >-
  Displays helpful information in a small overlay when users hover or focus on
  an element. Use to provide additional context without cluttering the
  interface.
api_name: app-home
source_url:
  html: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/typography-and-content/tooltip
  md: >-
    https://shopify.dev/docs/api/app-home/polaris-web-components/typography-and-content/tooltip.md
---

# Tooltip

Displays helpful information in a small overlay when users hover or focus on an element. Use to provide additional context without cluttering the interface.

## Slots

- **children**

  **HTMLElement**

  The content of the Tooltip.

  Only accepts `Text`, `Paragraph` components, and raw `textContent`.

## Slots

- **children**

  **HTMLElement**

  The content of the Tooltip.

  Only accepts `Text`, `Paragraph` components, and raw `textContent`.

Examples

### Examples

- #### Code

  ##### jsx

  ```jsx
  <>
    <s-tooltip id="bold-tooltip">Bold</s-tooltip>
    <s-button interestFor="bold-tooltip" accessibilityLabel="Bold">
      B
    </s-button>
  </>
  ```

  ##### html

  ```html
  <s-tooltip id="bold-tooltip">Bold</s-tooltip>
  <s-button interestFor="bold-tooltip" accessibilityLabel="Bold">B</s-button>
  ```

- #### Basic Usage

  ##### Description

  Demonstrates a simple tooltip that provides additional context for a text element when hovered or focused.

  ##### jsx

  ```jsx
  <>
    <s-tooltip id="shipping-status-tooltip">
      <s-text>This order has shipping labels.</s-text>
    </s-tooltip>
    <s-text interestFor="shipping-status-tooltip">Shipping status</s-text>
  </>
  ```

  ##### html

  ```html
  <s-tooltip id="shipping-status-tooltip">
    <s-text>This order has shipping labels.</s-text>
  </s-tooltip>
  <s-text interestFor="shipping-status-tooltip">Shipping status</s-text>
  ```

- #### With Icon Button

  ##### Description

  Shows how to add a tooltip to an icon button, providing a clear explanation of the button's action when hovered.

  ##### jsx

  ```jsx
  <>
    <s-tooltip id="delete-button-tooltip">
      <s-text>Delete item permanently</s-text>
    </s-tooltip>
    <s-button interestFor="delete-button-tooltip">
      <s-icon tone="neutral" type="info" />
    </s-button>
  </>
  ```

  ##### html

  ```html
  <s-tooltip id="delete-button-tooltip">
    <s-text>Delete item permanently</s-text>
  </s-tooltip>
  <s-button interestFor="delete-button-tooltip">
    <s-icon tone="neutral" type="info"></s-icon>
  </s-button>
  ```

## Usage

Tooltips only render on devices with a pointer and do not display on mobile devices.

## Best practices

- Use for additional, non-essential context only
- Provide for icon-only buttons or buttons with keyboard shortcuts
- Keep content concise and in sentence case
- Don't use for critical information, errors, or blocking messages
- Don't contain any links or buttons
- Use sparingly. If you need many tooltips, clarify the design and language instead

</page>

<page>
---
title: Using Polaris web components
description: >
  Polaris web components are Shopify's UI toolkit for building interfaces that
  match the Shopify Admin design system. This toolkit provides a set of custom
  HTML elements (web components) that you can use to create consistent,
  accessible, and performant user interfaces for the Shopify App Home and UI
  Extensions.
api_name: app-home
source_url:
  html: 'https://shopify.dev/docs/api/app-home/using-polaris-components'
  md: 'https://shopify.dev/docs/api/app-home/using-polaris-components.md'
---

# Using Polaris web components

Polaris web components are Shopify's UI toolkit for building interfaces that match the Shopify Admin design system. This toolkit provides a set of custom HTML elements (web components) that you can use to create consistent, accessible, and performant user interfaces for the Shopify App Home and UI Extensions.

***

## Availability

You can start using Polaris web components in any framework right away. Simply add the script tag to your app's HTML head.

## Example

##### HTML

```html
<head>
  <meta name="shopify-api-key" content="%SHOPIFY_API_KEY%" />
  <script src="https://cdn.shopify.com/shopifycloud/app-bridge.js"></script>
  <script src="https://cdn.shopify.com/shopifycloud/polaris.js"></script>
</head>
```

##### Remix

```tsx
import {useNavigate} from '@remix-run/react';

export default function App() {
  const navigate = useNavigate();

  useEffect(() => {
    const handleNavigate = (event) => {
      const href = event.target.getAttribute('href');
      if (href) navigate(href);
    };

    document.addEventListener('shopify:navigate', handleNavigate);
    return () => {
      document.removeEventListener('shopify:navigate', handleNavigate);
    };
  }, [navigate]);

  return (
    <html>
      <head>
        ...
        <script src="https://cdn.shopify.com/shopifycloud/polaris.js"></script>
      </head>
      ...
    </html>
  );
}
```

***

## Styling

Polaris web components come with built-in styling that follows Shopify's design system. The components will automatically apply the correct styling based on the properties you set and the context in which they are used. For example, headings automatically display at progressively less prominent sizes based on how many levels deep they are nested inside of sections.

In general, you should not need to use custom CSS to style Polaris web components. By building with only the components, you can ensure that your app will look and feel consistent with the Shopify Admin and its style will automatically adjust to match future updates.

## Example

## JSX

```jsx
<s-box
  padding="base"
  background="subdued"
  border="base"
  borderRadius="base"
>
  Content
</s-box>
```

***

## Opinionated Layout

To ensure the white space between components stays aligned with the Admin Design Language use components with opinionated spacing like `s-page` and `s-section`.

### s-page component

The `s-page` adds the global padding, background color, and internal layout for the Admin. It includes opinionated spacing between `s-section` and `s-banner`.

Two sizes:

- `large` which is full width used for tables and data visualization
- `base` which is narrow and used for forms with a sidebar

Can add a sidebar with the `aside` slot

### s-section component

The `s-section` component provides structured content areas with proper spacing. It provides default vertical white space to children

- `s-stack` and `s-grid` are not necessary as children unless building a complex layout
- Nesting `s-section` elements changes the heading level and the appearance of the section.
- The top level `s-section` renders a card appearance

## Example

```html
<s-page>
  <s-banner tone="critical" heading="Media upload failed">
    File extension doesn't match the format of the file.
  </s-banner>
  <s-section>
    <s-text-field label="Title"></s-text-field>
    <s-text-area label="Description"> </s-text-area>
  </s-section>


  <s-section heading="Status" slot="aside">
    <s-select>
      <s-option value="active">Active</s-option>
      <s-option value="draft">Draft</s-option>
    </s-select>
  </s-section>
</s-page>
```

***

## Custom layout

When you need to build custom layouts you can use `s-stack`, `s-grid` and `s-box`. You should always try to use `s-section` and `s-page` components first to ensure your white space stays aligned with the Admin Design Language.

- `s-stack` and `s-grid` do not include space between children by default. To apply the default white space between children use `gap="base"`
- Try to avoid adding vertical spacing with custom layouts. Use the `s-section` and `s-page` default white space instead.
- When `s-stack` is `display="inline"` it will automatically wrap children to a new line when space is limited.
- `s-grid` will allow children to overflow unless template rows/columns are properly set.
- Order is important for shorthand properties, e.g. border takes `size-keyword`, `color-keyword`, `style-keyword`

***

## Scale

Our components use a middle-out scale for multiple properties like `padding`, `size` and `gap`.

Our scale moves from the middle out:

- `small-300` is smaller than `small-100`
- `large-300` is bigger than `large-100`
- `small-100` and `large-100` have aliases of `small` and `large`
- `base` is the default value

## Example

```ts
export type Scale =
  | 'small-300'
  | 'small-200'
  | 'small-100'
  | 'small' // alias of small-100
  | 'base'
  | 'large' // alias of large-100
  | 'large-100'
  | 'large-200'
  | 'large-300';
```

***

## Responsive values

Some properties accept responsive values, which enables you to change the value of the property depending on a parent block size.

##### Syntax

The syntax for a responsive value generally follows the ternary operator syntax. For example, `@container (inline-size < 500px) small, large` means that the value will be `small` if the container is less than 500px wide, and `large` if the container is 500px or wider. The syntax rules are:

1. Begin the value with `@container`
2. Optionally add a name to target a specific container
3. Use the `inline-size` keyword inside of parentheses to query the inline-size of the container. This is the condition that will be evaluated to determine which value to use.
4. Set the value if that condition is true
5. Set the value to be used if the condition is false.

##### Using s-query-container

When using responsive values, you must also place the `<s-query-container>` component in the location you want to query the inline-size.

By default, the responsive value will query against the closest parent; to look up a specific parent, this component also accepts a `queryname` attribute which adds a name to the container. Then add that name after `@container` in your responsive value to target it.

##### Values with reserved characters

Some values could contain reserved characters used in the responsive value syntax, such as `()` or `,`. To use these values, escape them by wrapping them in quotes.

##### Advanced patterns

The syntax is flexible enough to support advanced patterns such as compound conditions, and|or conditions, and nested conditions.

## HTML

```html
<s-box padding="@container (inline-size < 500px) small, large">Hello</s-box>
```

## Pseudocode

##### Basic example

```html
<s-page>
  <s-query-container>
    <s-box padding="@container (inline-size < 500px) small, large">Hello</s-box>
  </s-query-container>
</s-page>
```

##### Named example

```html
<s-query-container containername="outer">
  <s-page>
    <s-query-container>
      <s-box padding="@container outer (inline-size < 500px) small, large">
        Hello
      </s-box>
    </s-query-container>
  </s-page>
</s-query-container>
```

## Escaped characters

```html
<s-query-container>
  <s-grid
    gridtemplatecolumns="@container (inline-size < 500px) 'repeat(2, 1fr)', 'repeat(3, 1fr)'"
  >
    ...
  </s-grid>
</s-query-container>
```

## Advanced patterns

##### Compound

```html
<s-query-container>
  <s-box padding="@container (300px < inline-size < 500px) small, large">
    The padding will be "small" when the container is between 300px and 500px
    wide. Otherwise it will be "large".
  </s-box>
</s-query-container>
```

##### And|or

```html
<s-query-container>
  <s-box
    padding="@container (inline-size < 500px) or (inline-size > 1000px) small, large"
  >
    This padding will be "small" when the container is less than 500px wide, or
    when the container is greater than 1000px wide. Otherwise it will be
    "large".
  </s-box>
</s-query-container>
```

##### Nested

```html
<s-query-container>
  <s-box
    padding="@container (inline-size < 500px) small, (500px <= inline-size < 1000px) base, large"
  >
    This padding will be "small" when the container is less than 500px wide,
    "base" when the container is between 500px and 1000px wide, and "large" when
    the container is greater than 1000px wide.
  </s-box>
</s-query-container>
```

***

## Interactive elements

`s-clickable` `s-button` and `s-link` render as anchor elements when they have a `href` and render as a button element when they have an `onClick` without a `href`. The HTML specification states that interactive elements cannot have interactive children.

`s-clickable` is an escape hatch for when `s-link` and `s-button` are not able to implement a specific design. You should always try to use `s-link` and `s-button` first.

Interactive components with `target="auto"` automatically use `_self` for internal links and `_blank` for external URLs. This behavior ensures a consistent navigation experience for users without requiring developers to manually set the correct target for each link.

***

## Variant tone and color

The `tone` is used to apply a group of color design tokens to the component such as `critical` `success` or `info`.

The `color` adjusts the intensity of the `tone` making it more `subdued` or `strong`.

The `variant` is used to change how the component is rendered to match the design language. This is different for each component.

## Example

```html
<s-button tone="critical" variant="primary"> Primary Critical Button </s-button>


<s-badge tone="success" color="strong"> Success Strong Badge </s-badge>
```

***

## Using with React (App Home)

When building in the App Home with the Shopify Remix template, you'll be using React. Here's how to use Polaris web components in your React components:

## Example

## JSX

```jsx
import {useState} from 'react';


function ProductForm() {
  const [productName, setProductName] = useState('');


  const handleSubmit = (event) => {
    event.preventDefault();
    console.log('Product name:', productName);
  };


  return (
    <s-section heading="Add Product">
      <form onSubmit={handleSubmit}>
        <s-stack gap="base">
          <s-text-field
            label="Product name"
            value={productName}
            onChange={(e) => setProductName(e.currentTarget.value)}
            required
          />
          <s-button variant="primary" type="submit">
            Save product
          </s-button>
        </s-stack>
      </form>
    </s-section>
  );
}
```

***

## Using with Preact (UI Extensions)

For UI Extensions, Shopify provides Preact as the framework of choice. Using Polaris web components with Preact is very similar to using them with React:

## Example

## JSX

```jsx
export function ProductExtension() {
  return (
    <s-box padding="base">
      <s-stack gap="base">
        <s-text>Enable special pricing</s-text>
        <s-checkbox
          onChange={() => console.log('Checkbox toggled')}
        />
        <s-number-field
          label="Discount percentage"
          suffix="%"
          min="0"
          max="100"
        />
      </s-stack>
    </s-box>
  );
}
```

***

## Properties vs Attributes

Polaris web components follow the same property and attribute patterns as standard HTML elements. Understanding this distinction is important for using the components effectively.

##### Key Concepts

1. **Attributes** are HTML attributes that appear in the HTML markup.
2. **Properties** are JavaScript object properties accessed directly on the DOM element.
3. Most attributes in Polaris web components are reflected as properties, with a few exceptions like `value` and `checked` which follow HTML's standard behavior.

##### How JSX Props Are Applied

When using Polaris web components in JSX (React or Preact), the framework determines how to apply your props based on whether the element has a matching property name.

If the element has a property with the exact same name as your prop, the value is set as a property. Otherwise, it's applied as an attribute. Here's how this works in pseudocode:

##### Examples

For Polaris web components, you can generally just use the property names as documented, and everything will work as expected.

## Pseudocode

## JavaScript

```javascript
if (propName in element) {
  // Set as a property
  element[propName] = propValue;
} else {
  // Set as an attribute
  element.setAttribute(propName, propValue);
}
```

## Examples

## JSX

```jsx
// This works as expected - the "gap" property accepts string values
<s-stack gap="base">...</s-stack>


// This also works - the "checked" property accepts boolean values
<s-checkbox checked={true}>...</s-checkbox>


// Complex values like objects and arrays are set as properties
<s-data-table items={[{ id: 1, name: 'Product' }]}>...</s-data-table>
```

***

## Event Handling

Polaris web components use standard DOM events, making them work seamlessly with your preferred framework. You can attach event handlers using the same patterns as with native HTML elements.

##### Basic Event Handling

Event handlers in Polaris components work just like standard HTML elements. In frameworks, use the familiar camelCase syntax (like `onClick` in React). In plain HTML, use lowercase attributes or `addEventListener`.

##### Form Input Events

Polaris form components support two primary event types for tracking input changes:

- **onInput**: Fires immediately on every keystroke or value change
- **onChange**: Fires when the field loses focus or Enter is pressed

Choose the appropriate event based on your needs:

- Use `onInput` for real-time validation or character counting
- Use `onChange` for validation after a user completes their input

##### Focus Management

Track when users interact with form elements using these events:

- **onFocus**: Fires when an element receives focus
- **onBlur**: Fires when an element loses focus

##### Form Values and Types

Important details about form values in Polaris web components:

- All form elements return string values in their events, even numeric inputs
- Multi-select components (like `s-choice-list`) use a `values` prop (array of strings)
- Access values in event handlers via `event.currentTarget.value`

##### Controlled vs. Uncontrolled Components

Polaris components can be used in two ways:

**Uncontrolled (simpler)**: Component manages its own internal state - use `defaultValue` prop

**Controlled (more powerful)**: Your code manages the component's state - use `value` prop

Use controlled components when you need to:

- Validate input as the user types
- Format or transform input values
- Synchronize multiple inputs

##### Technical Details

Under the hood, Polaris web components handle event registration consistently across frameworks:

- In React 18+, Polaris components properly register events via `addEventListener` instead of setting attributes
- Event names are automatically converted to lowercase (`onClick` becomes `click`)
- All event handlers receive standard DOM events as their first argument

For example, when you write `<s-button onClick={handleClick}>`, the component:

1. Sees that `"onclick" in element` is `true`
2. Registers your handler via `addEventListener('click', handler)`
3. Passes the event object to your handler when clicked

## Basic Event Handling Examples

##### JSX

```jsx
<s-button onClick={() => console.log('Clicked!')}>
  Inline Handler
</s-button>

<s-button onClick={(event) => {
  console.log('Event details:', event.type);
  console.log('Target:', event.currentTarget);
}}>
  With Event Object
</s-button>
```

##### HTML

```html
<s-button onclick="console.log('Button clicked!')">
  Click me
</s-button>

<s-button id="eventButton">
  Click me (addEventListener)
</s-button>

<script>
  const eventButton = document.getElementById('eventButton');

  eventButton.addEventListener('click', () => {
    console.log('Button clicked via addEventListener!');
  });
</script>
```

## Form Input Events Examples

##### JSX

```jsx
// OnInput fires on every keystroke
<s-text-field
  label="Email"
  name="email"
  onInput={(e) => console.log('Typing:', e.currentValue.value)}
/>

// OnChange fires on blur or Enter press
<s-text-field
  label="Name"
  name="name"
  onChange={(e) => console.log('Value committed:', e.currentValue.value)}
/>

// Using both together
<s-search-field
  label="Search"
  name="search"
  onInput={(e) => console.log('Real-time:', e.currentTarget.value)}
  onChange={(e) => console.log('Final value:', e.currentTarget.value)}
/>
```

##### HTML

```html
<s-text-field
  label="Email"
  name="email"
  oninput="console.log('Typing:', this.value)"
></s-text-field>

<s-text-field
  label="Name"
  name="name"
  onchange="console.log('Value committed:', this.value)"
></s-text-field>

<!-- Using addEventListener -->
<s-search-field
  label="Search"
  name="search"
></s-search-field>

<script>
  const field = document.querySelector('s-search-field');

  field.addEventListener('input', (e) => {
    console.log('Real-time:', e.currentTarget.value);
  });

  field.addEventListener('change', (e) => {
    console.log('Final value:', e.currentTarget.value);
  });
</script>
```

## Focus Management Examples

##### JSX

```jsx
<s-search-field
  label="Search"
  name="search"
  onFocus={() => console.log('Field focused')}
  onBlur={() => console.log('Field blurred')}
/>

<s-text-field
  label="Name"
  name="name"
  details="Tab to next field to trigger blur"
  onFocus={(e) => {
    e.currentTarget.setAttribute('details', 'Field is active!')
  }}
  onBlur={(e) => {
    e.currentTarget.setAttribute('details', 'Tab to next field to trigger blur')
  }}
/>
```

##### HTML

```html
<s-search-field
  label="Search"
  name="search"
  onfocus="console.log('Field focused')"
  onblur="console.log('Field blurred')"
></s-search-field>

<s-text-field
  label="Name"
  name="name"
  details="Tab to next field to trigger blur"
></s-text-field>

<script>
  const field = document.querySelector('s-text-field');

  field.addEventListener('focus', () => {
    field.setAttribute('details', 'Field is active!');
  });

  field.addEventListener('blur', () => {
    field.setAttribute('details', 'Tab to next field to trigger blur');
  });
</script>
```

## Form Values and Types Examples

##### JSX

```jsx
// Number field example - values are strings
<s-number-field
  label="Quantity"
  name="quantity"
  onChange={(e) => {
    // e.currentTarget.value is a string, convert if needed
    const quantity = Number(e.currentTarget.value);
    console.log('Quantity as number:', quantity);
  }}
/>

// Multi-select example - values is an array of strings
<s-choice-list
  name="colors"
  label="Colors"
  multiple
  onChange={(e) => {
    // e.currentTarget.values is an array of strings
    console.log('Selected colors:', e.currentTarget.values);
  }}
>
  <s-choice label="Red" value="red" />
  <s-choice label="Blue" value="blue" />
  <s-choice label="Green" value="green" />
</s-choice-list>
```

##### HTML

```html
<!-- Number field example - values are strings -->
<s-number-field
  label="Quantity"
  name="quantity"
  onchange="console.log('Value type:', typeof this.value, 'Value:', this.value)"
></s-number-field>

<!-- Multi-select example - values is an array of strings -->
<s-choice-list name="colors" label="Colors" multiple>
  <s-choice value="red">Red</s-choice>
  <s-choice value="blue">Blue</s-choice>
  <s-choice value="green">Green</s-choice>
</s-choice-list>

<script>
  const choiceList = document.querySelector('s-choice-list');

  choiceList.addEventListener('change', (e) => {
    // e.currentTarget.values is an array of strings
    console.log('Selected colors:', e.currentTarget.values);
  });
</script>
```

## Controlled vs. Uncontrolled Components Examples

##### JSX

```jsx
// Uncontrolled component - internal state
<s-text-field
  label="Comment"
  name="comment"
  defaultValue="Initial value"
  onChange={(e) => console.log('New value:', e.currentTarget.value)}
/>

// Controlled component - external state
// In a real component, 'name' would be from framework state
const name = "John Doe";

<s-text-field
  label="Name"
  name="name"
  value={name}
  onChange={(e) => {
    console.log('Would update state:', e.currentTarget.value)
  }}
/>
```

##### HTML

```html
<!-- Uncontrolled component - internal state -->
<s-text-field
  label="Comment"
  name="comment"
  value="Initial value"
  onchange="console.log('New value:', this.value)"
></s-text-field>

<!-- Controlled component - external state -->
<s-text-field
  id="nameField"
  label="Name"
  name="name"
  value="John Doe"
></s-text-field>

<button onclick="updateName()">
  Change Name
</button>

<script>
  const nameField = document.getElementById('nameField');

  // Listen for changes to update our "state"
  nameField.addEventListener('input', (e) => {
    console.log('Value changed:', e.currentTarget.value);
  });

  // Manually update the component value (controlled)
  function updateName() {
    nameField.value = "Jane Smith";
  }
</script>
```

## Complete Examples

##### JSX

```jsx
<s-button onClick={() => console.log('Clicked!')}>
  Click me
</s-button>

<s-text-field
  label="Email"
  name="email"
  onChange={(e) => console.log('Value changed:', e.currentTarget.value)}
  onFocus={() => console.log('Field focused')}
  onBlur={() => console.log('Field blurred')}
/>
```

##### HTML

```html
<s-button onclick="console.log('Clicked!')">
  Click me
</s-button>

<s-text-field
  label="Email"
  name="email"
  onchange="console.log('Value changed:', e.currentTarget.value)"
  onfocus="console.log('Field focused')"
  onblur="console.log('Field blurred')"
></s-text-field>

<!-- or -->

<script>
  const textField = document.querySelector('s-text-field');

  textField.addEventListener('change', (e) => {
    console.log('Value changed:', e.currentTarget.value);
  });
</script>
```

***

## Slots

Slots allow you to insert custom content into specific areas of Polaris web components. Use the `slot` attribute to specify where your content should appear within a component.

Key points:

- Named slots (e.g., `slot="title"`) place content in designated areas
- Multiple elements can share the same slot name
- Elements without a slot attribute go into the default (unnamed) slot

## Examples

##### Banner

```jsx
<s-banner heading="Order created" status="success">
  The order has been created successfully.
  <s-button slot="secondary-actions">
    View order
  </s-button>
  <s-button slot="secondary-actions">
    Download invoice
  </s-button>
</s-banner>
```

##### Page

```jsx
<s-page>
  <s-section>
    Main content
  </s-section>

  <div slot="aside">
    <s-section>
      Aside content
    </s-section>
  </div>
</s-page>
```

***

## Working with Forms

Polaris web components work seamlessly with standard HTML forms:

### Form Behavior

The form components will automatically participate in form submission and validation.

## Example

## JSX

```jsx
<form onSubmit={handleSubmit}>
  <s-stack gap="base">
    <s-text-field
      name="email"
      label="Email"
      type="email"
      required
    />
    <s-password-field
      name="password"
      label="Password"
      required
    />
    <s-button type="submit" variant="primary">
      Sign in
    </s-button>
  </s-stack>
</form>
```

***

## Accessibility

Polaris web components are built with accessibility in mind. They:

- Use semantic HTML under the hood
- Support keyboard navigation
- Include proper ARIA attributes
- Manage focus appropriately
- Provide appropriate color contrast
- Log warnings when component properties are missing and required for accessibility

To ensure your application remains accessible, follow these best practices:

1. Always use the `label` and `error` properties for form elements
2. Use appropriate heading levels with `s-heading` or the `heading` property
3. Ensure sufficient color contrast
4. Test keyboard navigation
5. Use `labelAccessibilityVisibility` to hide labels and keep them visible to assistive technologies
6. Use `accessibilityRole` to specify the `aria-role` of the component

## Example

## JSX

```jsx
// Good - provides a label
<s-text-field label="Email address" />


// Bad - missing a label
<s-text-field placeholder="Enter email" />
```

***

## Troubleshooting

Common issues and debugging tips for using Polaris web components.

##### Common Issues

1. **Properties not updating**: Ensure you're using the property name as documented, not a different casing or naming convention.

2. **Event handlers not firing**: Check that you're using the correct event name (e.g., `onClick` for click events).

3. **Form values not being submitted**: Make sure your form elements have `name` attributes.

##### Debugging Tips

1. Inspect the element in your browser's developer tools to see the current property and attribute values.

2. Use `console.log` to verify that event handlers are being called and receiving the expected event objects.

3. Check for any errors in the browser console that might indicate issues with your component usage.

***

</page>
