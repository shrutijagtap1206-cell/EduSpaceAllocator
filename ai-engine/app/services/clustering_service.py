from sklearn.cluster import KMeans
from sklearn.preprocessing import StandardScaler
import pandas as pd


INCOME_SCORE = {
    "Low": 0,
    "Medium": 1,
    "High": 2
}


def run_kmeans(communities: list[dict], n_clusters: int = 3):
    if not communities:
        return []

    if len(communities) < n_clusters:
        raise ValueError(
            f"At least {n_clusters} communities are required."
        )

    df = pd.DataFrame(communities)

    required_columns = [
        "population",
        "studentCount",
        "literacyRate",
        "incomeGroup"
    ]

    missing = [
        column for column in required_columns
        if column not in df.columns
    ]

    if missing:
        raise ValueError(
            f"Missing required fields: {', '.join(missing)}"
        )

    df["incomeScore"] = (
        df["incomeGroup"]
        .astype(str)
        .str.strip()
        .str.title()
        .map(INCOME_SCORE)
    )

    if df["incomeScore"].isna().any():
        raise ValueError(
            "IncomeGroup must be Low, Medium, or High."
        )

    features = df[
        [
            "population",
            "studentCount",
            "literacyRate",
            "incomeScore"
        ]
    ]

    scaler = StandardScaler()
    scaled_features = scaler.fit_transform(features)

    model = KMeans(
        n_clusters=n_clusters,
        random_state=42,
        n_init=10
    )

    df["cluster"] = model.fit_predict(scaled_features)

    cluster_stats = (
        df.groupby("cluster")[
            [
                "studentCount",
                "literacyRate",
                "incomeScore"
            ]
        ]
        .mean()
    )

    cluster_stats["demandIndex"] = (
        cluster_stats["studentCount"].rank(pct=True)
        + (1 - cluster_stats["literacyRate"].rank(pct=True))
        + (1 - cluster_stats["incomeScore"].rank(pct=True))
    )

    ordered_clusters = (
        cluster_stats["demandIndex"]
        .sort_values(ascending=False)
        .index
        .tolist()
    )

    labels = {}

    if len(ordered_clusters) >= 1:
        labels[ordered_clusters[0]] = "High Demand"

    if len(ordered_clusters) >= 2:
        labels[ordered_clusters[-1]] = "Emerging Demand"

    for cluster_id in ordered_clusters:
        if cluster_id not in labels:
            labels[cluster_id] = "Medium Demand"

    df["clusterLabel"] = df["cluster"].map(labels)

    return df.to_dict(orient="records")
