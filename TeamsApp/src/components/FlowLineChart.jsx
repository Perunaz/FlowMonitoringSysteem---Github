import React from 'react';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts';

// Helper function to round timestamps to the nearest interval (e.g., minute)
const roundTimestamp = (timestamp, intervalInSeconds) => {
    const intervalInMillis = intervalInSeconds * 1000;
    return Math.round(timestamp / intervalInMillis) * intervalInMillis;
};

// Function to add smaller randomness to y-values per line
const addLineRandomness = (data) => {
    const lineRandomness = Math.random() * 0.1 - 0.05; // Adjusted randomness range to ±0.05
    return data.map(entry => ({
        ...entry,
        status: entry.status + lineRandomness // Adding smaller randomness to status values per line
    }));
};

const FlowLineChart = ({ flows }) => {
    if (!flows || flows.length === 0) {
        return <div>No data available</div>;
    }

    const lineColors = [
        "#1f77b4", // Dark blue
        "#ff7f0e", // Orange
        "#2ca02c", // Green
        "#9467bd", // Purple
        "#d62728", // Red
        "#8c564b", // Brown
        "#e377c2", // Pink
        "#7f7f7f", // Gray
        "#bcbd22", // Olive
        "#17becf"  // Teal
    ];

    const getStatusValue = (status) => {
        if (status === "Succeeded") {
            return 1;
        } else {
            return 0; // Default to 0 if status is not "Succeeded"
        }
    };

    const intervalInSeconds = 60; // Define the interval for rounding (e.g., 60 seconds)

    // Convert status values to numbers and convert time to timestamps
    const transformedData = flows.map(flow => {
        const roundedData = {};

        flow.data.forEach(entry => {
            const roundedTimestamp = roundTimestamp(new Date(entry.originalTime).getTime(), intervalInSeconds);
            if (!roundedData[roundedTimestamp]) {
                roundedData[roundedTimestamp] = { ...entry, timestamp: roundedTimestamp, count: 0 };
            }
            roundedData[roundedTimestamp].count += 1;
            roundedData[roundedTimestamp].status = getStatusValue(entry.status);
        });

        return {
            name: flow.name,
            data: Object.values(roundedData)
                .sort((a, b) => a.timestamp - b.timestamp) // Sorting by timestamp
        };
    });

    // Apply smaller randomness per line
    const randomizedData = transformedData.map(flow => ({
        ...flow,
        data: addLineRandomness(flow.data)
    }));

    const formatStatusValue = (value) => {
        const roundedValue = Math.round(value);
        return roundedValue === 0 ? "Failed" : "Succeeded";
    };

    return (
        <ResponsiveContainer width="100%" height="100%" maxHeight={500}>
            <LineChart margin={{ top: 20, right: 30, left: 20, bottom: 20 }}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis
                    dataKey="timestamp"
                    type="number"
                    domain={['auto', 'auto']}
                    tickFormatter={(tick) => new Date(tick).toLocaleTimeString()} // Convert timestamp back to readable time
                />
                <YAxis
                    domain={[-0.1, 1.1]}
                    ticks={[0, 1]}
                    tickFormatter={(value) => value === 0 ? "Failed" : "Succeeded"} // Custom tick formatter
                />
                <Tooltip
                    labelFormatter={(label) => new Date(label).toLocaleTimeString()} // Convert timestamp in tooltip
                    formatter={(value) => formatStatusValue(value)} // Convert status value in tooltip
                    isAnimationActive={false} // Disable tooltip animation
                    contentStyle={{ pointerEvents: 'none' }} // Prevent tooltip from blocking cursor
                />
                {randomizedData.map((s, index) => (
                    <Line
                        key={s.name}
                        type="monotone"
                        dataKey="status"
                        data={s.data || []}
                        name={s.name}
                        stroke={lineColors[index % lineColors.length]}
                    />
                ))}
            </LineChart>
        </ResponsiveContainer>
    );
}

export default FlowLineChart;
