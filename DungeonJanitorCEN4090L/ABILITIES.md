# Abilities System

A flexible, event-driven abilities system for Unity that supports various targeting types, effects, and resource management.

---

## Architecture Diagram

```mermaid
flowchart TB
    subgraph Input["Input Layer"]
        PI[PlayerInput]
        AM[AbilityManager]
    end

    subgraph Data["Data Layer (ScriptableObjects)"]
        P[Player SO]
        AD[AbilityDefinition SO]
        ESO[EffectSO]
    end

    subgraph Execution["Execution Layer"]
        AU[AbilityUser]
        PROJ[Projectile]
    end

    subgraph Effects["Effect System"]
        ED[EffectDefinition]
        IE[IEffect]
    end

    subgraph Health["Health & Resources"]
        HC[HealthComponent]
        RM[ResourceManager]
        IH[IHealth]
    end

    PI -->|"UseAbility1/2/3"| AM
    AM -->|"Reads equipped abilities"| P
    AM -->|"Checks mana"| RM
    AM -->|"OnAbilityRequested event"| AU
    
    P -->|"Stores"| AD
    AD -->|"Contains"| ED
    ED -->|"References"| ESO
    ESO -->|"Creates"| IE
    
    AU -->|"Spawns"| PROJ
    AU -->|"Executes"| IE
    PROJ -->|"On hit"| IE
    
    IE -->|"Damage/Heal"| HC
    HC -->|"Implements"| IH
    RM -->|"Syncs with"| P
```

---

## Component Relationships

```mermaid
classDiagram
    class AbilityManager {
        -Player playerData
        -ResourceManager manaManager
        -float[] abilityCooldowns
        +OnAbilityRequested : event
        +OnCooldownStarted : event
        +OnAbilityFailed : event
        -UseAbility(int index)
    }

    class AbilityUser {
        -Transform castPoint
        +ExecuteAbility(AbilityDefinition)
        -ExecuteProjectileAbility()
        -ExecuteRaycastSingleAbility()
        -ExecuteSelfAbility()
        -ExecuteAreaOfEffectAbility()
        -ApplyEffectsToTarget()
    }

    class AbilityDefinition {
        +string Name
        +float Cooldown
        +float ResourceCost
        +TargetingType Targeting
        +EffectDefinition[] Effects
        +GameObject ProjectilePrefab
    }

    class EffectDefinition {
        +EffectSO effect
        +EffectType effectType
        +float magnitude
        +float duration
        +float scalingFactor
        +GetScaledMagnitude() float
    }

    class EffectSO {
        <<abstract>>
        +CreateEffectInstance() IEffect
    }

    class IEffect {
        <<interface>>
        +Apply(caster, target, def, hitPoint)
    }

    class HealthComponent {
        +int CurrentHealth
        +int MaxHealth
        +bool IsDead
        +TakeDamage(int)
        +Heal(int)
        +OnDeath : event
    }

    class ResourceManager {
        +float CurrentAmount
        +float Max
        +HasSufficientResources(float) bool
        +SpendResource(float) bool
    }

    class Projectile {
        +GameObject Caster
        +float Speed
        +float Lifetime
        +EffectDefinition[] Effects
    }

    AbilityManager --> AbilityUser : fires event
    AbilityManager --> ResourceManager : checks/spends
    AbilityDefinition --> EffectDefinition : contains
    EffectDefinition --> EffectSO : references
    EffectSO --> IEffect : creates
    AbilityUser --> Projectile : spawns
    IEffect --> HealthComponent : modifies
```

---

## Event Flow

```mermaid
sequenceDiagram
    participant Player
    participant AM as AbilityManager
    participant RM as ResourceManager
    participant AU as AbilityUser
    participant AD as AbilityDefinition
    participant HC as HealthComponent

    Player->>AM: Press ability key (1/2/3)
    AM->>AM: Check cooldown
    AM->>RM: HasSufficientResources()
    RM-->>AM: true/false
    
    alt Has resources
        AM->>RM: SpendResource()
        AM->>AM: Start cooldown
        AM->>AU: OnAbilityRequested(ability)
        AU->>AD: Get targeting type
        
        alt Projectile
            AU->>AU: Spawn projectile
        else Self
            AU->>HC: ApplyEffects(caster)
        else Raycast
            AU->>HC: ApplyEffects(hit target)
        else AOE
            AU->>HC: ApplyEffects(all in radius)
        end
    else No resources
        AM->>AM: OnAbilityFailed event
    end
```

---

## Core Components

| Component | Responsibility |
|-----------|----------------|
| `AbilityManager` | Input handling, cooldowns, mana validation, fires events |
| `AbilityUser` | Executes abilities based on targeting type |
| `AbilityDefinition` | ScriptableObject defining ability properties |
| `EffectSO` | Abstract base for effect ScriptableObjects |
| `EffectDefinition` | Serializable effect parameters (magnitude, duration, etc.) |
| `IEffect` | Interface for runtime effect execution |
| `HealthComponent` | Health management, damage, healing, death |
| `ResourceManager` | Mana/resource management with regeneration |
| `Projectile` | Runtime projectile movement and collision |
| `Player` | ScriptableObject storing equipped abilities and stats |

---

## Targeting Types

| Type | Execution Method | Description |
|------|------------------|-------------|
| `Self` | `ExecuteSelfAbility()` | Applies effects to caster |
| `Projectile` | `ExecuteProjectileAbility()` | Spawns traveling projectile |
| `RaycastSingle` | `ExecuteRaycastSingleAbility()` | Instant hit via raycast |
| `AreaOfEffect` | `ExecuteAreaOfEffectAbility()` | Hits all in radius |

---

## Effect Types

| Type | Description |
|------|-------------|
| `Damage` | Reduces target health |
| `Heal` | Restores target health |
| `Buff` | Positive status effect |
| `Debuff` | Negative status effect |

---

## File Structure

```
Assets/Scripts/Abilities/
??? AbilityManager.cs       # Input, cooldowns, events
??? AbilityUser.cs          # Ability execution
??? AbilityDefinition.cs    # Ability SO + TargetingType enum
??? EffectSO.cs             # Abstract effect base
??? EffectDefinition.cs     # Effect parameters
??? EffectType.cs           # Effect type enum
??? IEffect.cs              # Effect interface
??? HealthComponent.cs      # Health + IHealth interface
??? ResourceManager.cs      # Resource management
??? Projectile.cs           # Projectile behavior
```

---

## Quick Reference

### Creating an Ability
1. Create `AbilityDefinition` asset
2. Set targeting type, cooldown, mana cost
3. Add effects via `EffectDefinition` array
4. Assign to `Player` SO's equipped abilities

### Creating a Custom Effect
1. Extend `EffectSO` ? implement `CreateEffectInstance()`
2. Create class implementing `IEffect` ? implement `Apply()`
3. Create SO asset, assign to ability's effects