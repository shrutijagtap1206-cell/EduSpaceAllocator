from fastapi import FastAPI
from app.routes.clustering import router as clustering_router

app = FastAPI(
    title="EduSpace Allocator AI Engine",
    version="1.0.0"
)

app.include_router(clustering_router)


@app.get("/")
def root():
    return {
        "service": "EduSpace Allocator AI Engine",
        "status": "running"
    }
