# 07. Matching Algorithm: Bipartite Maximum Weight Matching

## 1. Algorithmic Pipeline Overview

```
Input Data (Spaces & Requests)
        ↓
Data Validation & Schema Integrity
        ↓
Community Demand Analysis
        ↓
Candidate Generation (Distance ≤ 5 km, Capacity ≥ Demand, Budget ≥ Rent)
        ↓
Bipartite Graph Construction G = (U, V, E)
        ↓
Maximum-Weight Matching (Kuhn-Munkres / Hungarian Algorithm)
        ↓
Optimization Composite Scoring
        ↓
Allocation Recommendation & Persistence
        ↓
GIS Map & Analytics Dashboard
```

---

## 2. Bipartite Graph Formulation
We construct an undirected bipartite graph $G = (U, V, E)$ where:
- $U$: Set of available learning space nodes $\{u_1, u_2, \dots, u_m\}$
- $V$: Set of active learning request nodes $\{v_1, v_2, \dots, v_n\}$
- $E$: Edge $(u_i, v_j)$ exists if and only if space $u_i$ is **feasible** for request $v_j$.

### Edge Weight Function
Each edge $(u, v) \in E$ is assigned a non-negative weight $W(u, v)$ derived directly from the multi-objective optimization score:
$$W(u, v) = \text{OptimizationScore}(u, v)$$

---

## 3. Matching Resolution
The matching problem is solved using the Maximum Weight Matching algorithm (via NetworkX / Kuhn-Munkres implementation):
$$\max_{M \subseteq E} \sum_{e \in M} w(e)$$
subject to:
$$\text{deg}_M(x) \le 1 \quad \forall x \in U \cup V$$

This mathematical formulation guarantees:
1. No learning space is double-allocated during the matching interval.
2. Every NGO is paired with at most one optimal facility.
3. Global utility (proximity, high capacity utilization, budget alignment) is maximized over greedy local choices.
