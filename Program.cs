//*****************************************************************************
//** 1334. Find the City With the Smallest Number of Neighbors at a Threshold**
//** Distance  leetcode                                                      **
//*****************************************************************************
//** Commented out the Bellman-Ford algorithm (slower) and used Dijkstra's   **
//*****************************************************************************


/*
int* bellmanFord(int n, int** edges, int edgesSize, int src) {
    int* dist = (int*)malloc(n * sizeof(int));
    
    // Initialize distances from src to all other vertices as infinite
    for (int i = 0; i < n; i++) {
        dist[i] = INT_MAX;
    }
    dist[src] = 0;
    
    // Relax all edges |V| - 1 times.
    for (int i = 1; i <= n - 1; i++) {
        for (int j = 0; j < edgesSize; j++) {
            int u = edges[j][0];
            int v = edges[j][1];
            int weight = edges[j][2];
            if (dist[u] != INT_MAX && dist[u] + weight < dist[v]) {
                dist[v] = dist[u] + weight;
            }
            if (dist[v] != INT_MAX && dist[v] + weight < dist[u]) {
                dist[u] = dist[v] + weight;
            }
        }
    }
    
    // Check for negative-weight cycles (not necessary in this problem)
    for (int i = 0; i < edgesSize; i++) {
        int u = edges[i][0];
        int v = edges[i][1];
        int weight = edges[i][2];
        if (dist[u] != INT_MAX && dist[u] + weight < dist[v]) {
            printf("Graph contains negative weight cycle\n");
            return NULL;
        }
    }
    
    return dist;
}

int findTheCity(int n, int** edges, int edgesSize, int* edgesColSize, int distanceThreshold) {
    int minReachableCities = INT_MAX;
    int resultCity = -1;

    for (int i = 0; i < n; i++) {
        int* distances = bellmanFord(n, edges, edgesSize, i);
        if (distances == NULL) {
            return -1; // Negative weight cycle detected
        }
        
        int reachableCities = 0;
        for (int j = 0; j < n; j++) {
            if (i != j && distances[j] <= distanceThreshold) {
                reachableCities++;
            }
        }

        if (reachableCities <= minReachableCities) {
            minReachableCities = reachableCities;
            resultCity = i;
        }

        free(distances);
    }

    return resultCity;
}
*/

typedef struct {
    int city;
    int distance;
} HeapNode;

typedef struct {
    HeapNode *nodes;
    int size;
    int capacity;
} MinHeap;

MinHeap* createMinHeap(int capacity) {
    MinHeap* minHeap = (MinHeap*)malloc(sizeof(MinHeap));
    minHeap->nodes = (HeapNode*)malloc(capacity * sizeof(HeapNode));
    minHeap->size = 0;
    minHeap->capacity = capacity;
    return minHeap;
}

void swapHeapNode(HeapNode* a, HeapNode* b) {
    HeapNode temp = *a;
    *a = *b;
    *b = temp;
}

void heapify(MinHeap* minHeap, int idx) {
    int smallest = idx;
    int left = 2 * idx + 1;
    int right = 2 * idx + 2;

    if (left < minHeap->size && minHeap->nodes[left].distance < minHeap->nodes[smallest].distance)
        smallest = left;

    if (right < minHeap->size && minHeap->nodes[right].distance < minHeap->nodes[smallest].distance)
        smallest = right;

    if (smallest != idx) {
        swapHeapNode(&minHeap->nodes[smallest], &minHeap->nodes[idx]);
        heapify(minHeap, smallest);
    }
}

int isEmpty(MinHeap* minHeap) {
    return minHeap->size == 0;
}

HeapNode extractMin(MinHeap* minHeap) {
    if (isEmpty(minHeap))
        return (HeapNode){-1, INT_MAX};

    HeapNode root = minHeap->nodes[0];
    minHeap->nodes[0] = minHeap->nodes[minHeap->size - 1];
    minHeap->size--;
    heapify(minHeap, 0);

    return root;
}

void decreaseKey(MinHeap* minHeap, int city, int distance) {
    int i;
    for (i = 0; i < minHeap->size; i++) {
        if (minHeap->nodes[i].city == city) {
            minHeap->nodes[i].distance = distance;
            break;
        }
    }
    while (i && minHeap->nodes[i].distance < minHeap->nodes[(i - 1) / 2].distance) {
        swapHeapNode(&minHeap->nodes[i], &minHeap->nodes[(i - 1) / 2]);
        i = (i - 1) / 2;
    }
}

int* dijkstra(int n, int** graph, int src, int distanceThreshold) {
    MinHeap* minHeap = createMinHeap(n);
    int* dist = (int*)malloc(n * sizeof(int));

    for (int v = 0; v < n; ++v) {
        dist[v] = INT_MAX;
        minHeap->nodes[v].city = v;
        minHeap->nodes[v].distance = INT_MAX;
    }

    minHeap->nodes[src].city = src;
    minHeap->nodes[src].distance = 0;
    dist[src] = 0;
    minHeap->size = n;
    decreaseKey(minHeap, src, 0);

    while (!isEmpty(minHeap)) {
        HeapNode minHeapNode = extractMin(minHeap);
        int u = minHeapNode.city;

        for (int v = 0; v < n; ++v) {
            if (graph[u][v] && dist[u] != INT_MAX && dist[u] + graph[u][v] < dist[v]) {
                dist[v] = dist[u] + graph[u][v];
                decreaseKey(minHeap, v, dist[v]);
            }
        }
    }

    free(minHeap->nodes);
    free(minHeap);

    return dist;
}

int findTheCity(int n, int** edges, int edgesSize, int* edgesColSize, int distanceThreshold) {
    int** graph = (int**)malloc(n * sizeof(int*));
    for (int i = 0; i < n; i++) {
        graph[i] = (int*)malloc(n * sizeof(int));
        for (int j = 0; j < n; j++) {
            graph[i][j] = 0;
        }
    }

    for (int i = 0; i < edgesSize; i++) {
        int u = edges[i][0];
        int v = edges[i][1];
        int w = edges[i][2];
        graph[u][v] = w;
        graph[v][u] = w;
    }

    int minReachableCities = INT_MAX;
    int resultCity = -1;

    for (int i = 0; i < n; i++) {
        int* distances = dijkstra(n, graph, i, distanceThreshold);
        int reachableCities = 0;
        for (int j = 0; j < n; j++) {
            if (i != j && distances[j] <= distanceThreshold) {
                reachableCities++;
            }
        }

        if (reachableCities <= minReachableCities) {
            minReachableCities = reachableCities;
            resultCity = i;
        }

        free(distances);
    }

    for (int i = 0; i < n; i++) {
        free(graph[i]);
    }
    free(graph);

    return resultCity;
}
