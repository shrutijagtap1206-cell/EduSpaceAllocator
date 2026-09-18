import { useEffect, useMemo, useState } from 'react';
import api from '../services/api';

export default function Courses() {
  const [items, setItems] = useState([]);
  const [search, setSearch] = useState('');
  const [category, setCategory] = useState('all');
  const [level, setLevel] = useState('all');
  const [sortBy, setSortBy] = useState('id');

  useEffect(() => {
    api.get('/Courses')
      .then((response) => setItems(response.data))
      .catch(console.error);
  }, []);

  const categories = [...new Set(items.map((x) => x.category))];
  const levels = [...new Set(items.map((x) => x.skillLevel))];

  const filtered = useMemo(() => {
    let result = items.filter((item) => {
      const text = `${item.courseName} ${item.category}`.toLowerCase();

      return (
        text.includes(search.toLowerCase()) &&
        (category === 'all' || item.category === category) &&
        (level === 'all' || item.skillLevel === level)
      );
    });

    result = [...result].sort((a, b) => {
      if (sortBy === 'capacity') return b.requiredCapacity - a.requiredCapacity;
      if (sortBy === 'duration') return b.durationHours - a.durationHours;
      return a.courseId - b.courseId;
    });

    return result;
  }, [items, search, category, level, sortBy]);

  return (
    <div>
      <div className="page-heading">
        <div>
          <h2>Courses</h2>
          <p className="page-subtitle">
            Course catalog and space requirements.
          </p>
        </div>
      </div>

      <div className="filter-bar">
        <input
          className="search-input"
          placeholder="Search course or category..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />

        <select value={category} onChange={(e) => setCategory(e.target.value)}>
          <option value="all">All Categories</option>
          {categories.map((item) => (
            <option key={item} value={item}>{item}</option>
          ))}
        </select>

        <select value={level} onChange={(e) => setLevel(e.target.value)}>
          <option value="all">All Levels</option>
          {levels.map((item) => (
            <option key={item} value={item}>{item}</option>
          ))}
        </select>

        <select value={sortBy} onChange={(e) => setSortBy(e.target.value)}>
          <option value="id">Sort: ID</option>
          <option value="capacity">Sort: Capacity</option>
          <option value="duration">Sort: Duration</option>
        </select>
      </div>

      <div className="result-count">
        Showing {filtered.length} of {items.length} courses
      </div>

      <div className="card">
        <table className="data-table">
          <thead>
            <tr>
              <th>ID</th>
              <th>Course</th>
              <th>Category</th>
              <th>Capacity</th>
              <th>Hours</th>
              <th>Level</th>
            </tr>
          </thead>

          <tbody>
            {filtered.map((item) => (
              <tr key={item.courseId}>
                <td>{item.courseId}</td>
                <td>{item.courseName}</td>
                <td>{item.category}</td>
                <td>{item.requiredCapacity}</td>
                <td>{item.durationHours}</td>
                <td>{item.skillLevel}</td>
              </tr>
            ))}

            {filtered.length === 0 && (
              <tr>
                <td colSpan="6">No courses match the selected filters.</td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
