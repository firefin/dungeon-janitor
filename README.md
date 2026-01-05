# Group 12 for CEN4090L Project

## Dungeon Janitor

Play as a janitor who's hired to clean up dungeons after previous adventurer's adventuring. But beware; not always are the monsters fully dead. Sometimes, other monsters may show up and see what happened and be quite irritated at seeing their kin's remains splayed across the floor.



# The Ability Framework

### Architecture Overview

[Document showing flow](/DungeonJanitorCEN4090L/ABILITIES.md)

## Some title

### Core Components

| Component | Responsibility |
|-----------|----------------|
| `AbilityManager` | Handles input, cooldowns, mana validation, and fires events |
| `AbilityUser` |  Executes abilities (projectiles, effects, targeting) |
| `AbilityDefinition` | ScriptableObject defining ability properties |
| `Player` | ScriptableObject storing equipped abilities and player stats |
| `ResourceManager` | Manages mana/resource costs and regeneration |
| `HealthComponent` | Manages health, damage, healing, and death |
| `EffectSO` | Abstract ScriptableObject base for effect logic |
| `EffectDefinition` | Serializable class defining effect parameters |
| `Projectile` | Runtime projectile behavior for projectile-type abilities |

### How It Works

- **Input** - Player presses ability key(s) (1, 2, or 3)
- **Validation** - `AbilityManager` checks cooldown and mana
- **Resource Spend** - Mana is deducted via `ResourceManager`
- **Event** - `OnAbilityRequested` event is fired with the `AbilityDefinition`
- **Execution** - `AbilityUser` receives event and executes based on targeting type
- **Effects** - `EffectDefinition` effects are applied to targets via `IEffect.Apply()`

### Targeting Types

Defined in `TargetingType` enum:

| Type | Description |
|------|-------------|
| `Self` | Applies effects directly to the caster |
| `Projectile` | Spawns a projectile that travels forward and applies effects on hit |
| `RaycastSingle` | Instant hit on first target in line of sight |
| `AreaOfEffect` | Hits all targets within a radius using `Physics.OverlapSphere` |

## The Effects System

#### Effect Types

Defined in `EffectType` enum:

| Type | Description |
|------|-------------|
| `Damage` | Deals damage to target |
| `Heal` | Restores health to target |
| `Buff` | Applies positive status effect |
| `Debuff` | Applies negative status effect |



- **EffectDefinition** -  A serializable class containing effect parameters:

- **EffectSO** - Base abstract class for creating custom effects:

- **IEffect** - Implement this interface to create custom effect logic:

---

## Health System

### *HealthComponent*

Manages entity health with events for UI/feedback:

| Property/Method | Description |
|-----------------|-------------|
| `CurrentHealth` | Current health value |
| `MaxHealth` | Maximum health value |
| `IsDead` | Whether entity is dead |
| `TakeDamage(int)` | Apply damage to entity |
| `Heal(int)` | Restore health to entity |
| `FullHeal()` | Restore to max health |

### Events

- `OnHealthChanged` - Fired when health changes
- `OnDamaged` - Fired when taking damage
- `OnHealed` - Fired when healed
- `OnDeath` - Fired when health reaches 0

---

## Resource Management

### *ResourceManager*

Handles mana (or other resources) with regeneration:

| Property | Description |
|----------|-------------|
| `CurrentAmount` | Current resource amount |
| `Max` | Maximum resource capacity |
| `RegenRate` | Regeneration per second |
| `HasSufficientResources(float)` | Check if enough resources |
| `SpendResource(float)` | Deduct resources |
| `AddResource(float)` | Add resources |

### Events

- `OnResourceChanged` - Fired when resource amount changes
- `OnResourceSpent` - Fired when resources are deducted
- `OnResourceGained` - Fired when resources are added

---

## Projectile System

### Projectile Component

Attached to projectile prefabs, handles movement and collision:

| Property | Description |
|----------|-------------|
| `Caster` | GameObject that fired the projectile |
| `Speed` | Movement speed |
| `Lifetime` | Time before auto-destroy |
| `Effects` | Effects to apply on hit |
| `HitLayers` | LayerMask for valid targets |

---

## Creating a New Ability

1. Right-click in Project → **Create > Scriptable Objects > AbilityDefinition**
2. Configure properties:
   - **Basic Info**: Name, description, icon
   - **Cooldown & Resources**: Cooldown time, mana cost
   - **Targeting**: Type, range, hit layers
   - **Effects**: Add `EffectDefinition` entries
   - **Projectile** (if applicable): Prefab, speed
3. Assign to a slot in the `Player` ScriptableObject's `equippedAbilities` array

---

## Creating a Custom Effect

1. Create a new class extending `EffectSO`:
2. Implement the `IEffect` interface:
3. Create the ScriptableObject asset and assign to an `AbilityDefinition`

---

## Events (for UI/Feedback)

### AbilityManager Events

- `OnAbilityRequested` - Fired when an ability is activated
- `OnAbilityExecuted` - Fired when an ability is successfully executed
- `OnAbilityCanceled` - Fired when an ability is canceled

### AbilityUser Events

- `OnProjectileCreated` - Fired when a projectile is fired
- `OnEffectApplied` - Fired when an effect is applied to a target

### HealthComponent Events

- `OnHealthChanged` - Fired when health changes
- `OnDamaged` - Fired when taking damage
- `OnHealed` - Fired when healed
- `OnDeath` - Fired when health reaches 0
