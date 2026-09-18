# 06. Machine Learning Engine: Demand Clustering

## 1. Role in the Pipeline
The Python AI engine operates as an internal intelligence service. Rather than forcing users to manually aggregate fragmented learning requests, the engine applies unsupervised machine learning to group contiguous community student demands into optimal geographic catchments.

---

## 2. K-Means Geospatial Clustering
Given demand coordinate pairs $X = \{(lat_i, lon_i)\}_{i=1}^n$ weighted by required student count $w_i$:

$$\arg\min_{\mathbf{S}} \sum_{i=1}^{k} \sum_{\mathbf{x} \in S_i} w_{\mathbf{x}} \|\mathbf{x} - \boldsymbol{\mu}_i\|^2$$

where $\boldsymbol{\mu}_i$ represents the weighted geographic centroid of cluster $S_i$.

### Advantages for Municipal Planning
1. **Identifies Infrastructure Deserts**: Detects dense student clusters located outside the 2 km buffer zone of any existing registered learning space.
2. **Prevents Spatial Cannibalization**: Ensures nearby NGOs do not compete for the same singular building while adjacent wards remain unserved.
3. **Consolidates Micro-Demands**: Aggregates small classes into viable batch sizes for larger institutional halls.
