# 02. Problem Statement

## 1. Context & Motivation
Urbanization has outpaced public social infrastructure in tier-1 and tier-2 Indian cities. Non-governmental organizations (NGOs) and community groups provide essential supplemental education, after-school tutoring, and adult digital skills to underserved children. However, they face systemic hurdles:

1. **High Commercial Rents**: Rental rates in urban centers exceed non-profit operational budgets.
2. **Transit & Safety Friction**: Long commute distances deter female students and younger children from attending evening classes.
3. **Severe Asymmetry of Information**: While NGOs search fruitlessly for rooms, commercial buildings, corporate auditoriums, library halls, and municipal offices stand dark and locked during evenings and weekends.
4. **Manual & Inefficient Scheduling**: Ad-hoc coordination relies on personal networks, leading to double-booking, mismatched class sizes, and sudden cancellations.

---

## 2. Quantitative Problem Definition
Given:
- A set of spaces $S = \{s_1, s_2, \dots, s_m\}$, each defined by location $(lat, lon)$, capacity $C(s)$, rental cost $R(s)$, and availability flag $A(s)$.
- A set of community learning requests $R = \{r_1, r_2, \dots, r_n\}$, each defined by preferred location $(lat, lon)$, required student capacity $K(r)$, budget $B(r)$, and organization $O(r)$.

Find an allocation matching $M \subseteq S \times R$ that:
$$\text{Maximize } \sum_{(s, r) \in M} \text{Score}(s, r)$$
subject to:
1. **Capacity Feasibility**: $C(s) \ge K(r)$ for all $(s, r) \in M$
2. **Budget Feasibility**: $B(r) \ge R(s)$ (or within acceptable tolerance)
3. **Distance Constraint**: $\text{dist}(s, r) \le D_{\max}$ (default 5 km)
4. **One-to-One Matching per Time Slot**: No space $s$ is assigned to more than one request $r$ simultaneously.

---

## 3. The EduSpace Allocator Solution
EduSpace Allocator automates this entire pipeline using a multi-criteria optimization function and bipartite maximum-weight matching, ensuring transparent, deterministic, and equitable resource distribution across municipal wards.
